using App.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace App.Core.Solver
{
   public class CycleDetector
   {
      public enum CycleType
      {
         Positive,
         Nagative
      }

      public string CycleElementId { get; private set; }

      public Cycle WyznaczonyCykl { get; private set; }

      public bool IsOptimal { get; private set; }

      public (bool IsError, string Message) Error { get; private set; }

      private GridCell[][] m_grid;


      public CycleDetector(GridCell[][] a_grid, CycleType a_cycleType)
      {
         m_grid = a_grid;
         CycleElementId = a_cycleType == CycleType.Nagative 
                              ? FindLeastElementId() 
                              : FindMaxElementId();
         if (Error.IsError)
            return;
         IsOptimal = CycleElementId is null ? true : false;
      }


      /// <summary>
      /// Wykrywa cykl dla zadanej siatki optymalizacji.
      /// </summary>
      public CycleDetector Detect()
      {
         if (!IsOptimal && !Error.IsError)
         {
            WyznaczonyCykl = FindCycle();
            MarkPointsType();
         }
         return this;
      }

      private Cycle FindCycle()
      { 
         var startY = int.Parse(CycleElementId[0].ToString());
         var startX = int.Parse(CycleElementId[1].ToString());
         var cycles = new List<Cycle>();

         var maxLengthX = m_grid[0][m_grid[0].Length - 1].IsVirtual ? m_grid[0].Length - 1 : m_grid[0].Length;
         var maxLengthY = m_grid[m_grid.Length - 1][0].IsVirtual ? m_grid.Length - 1 : m_grid.Length;

         for (int i = 1; i < m_grid.Length; i++)
         {
            if (maxLengthX == i)
               continue;

            for (int j = 1; j < m_grid[i].Length; j++)
            {
               if (maxLengthY == j)
                  continue;

               #region [Impl]
               var a = new CyclePoint(startY, startX, true);

               var b1 = new CyclePoint(startY, startX - i);
               var c1 = new CyclePoint(startY - j, startX - i);
               var d1 = new CyclePoint(startY - j, startX);
               var cykl1 = new Cycle(a, b1, c1, d1);
               if (cykl1.IsInGridScope())
                  cycles.Add(cykl1);

               var b2 = new CyclePoint(startY, startX - i);
               var c2 = new CyclePoint(startY + j, startX - i);
               var d2 = new CyclePoint(startY + j, startX);
               var cykl2 = new Cycle(a, b2, c2, d2);
               if (cykl2.IsInGridScope())
                  cycles.Add(cykl2);

               var b3 = new CyclePoint(startY, startX + i);
               var c3 = new CyclePoint(startY + j, startX + i);
               var d3 = new CyclePoint(startY + j, startX);
               var cykl3 = new Cycle(a, b3, c3, d3);
               if (cykl3.IsInGridScope())
                  cycles.Add(cykl3);

               var b4 = new CyclePoint(startY, startX + i);
               var c4 = new CyclePoint(startY - j, startX + i);
               var d4 = new CyclePoint(startY - j, startX);
               var cykl4 = new Cycle(a, b4, c4, d4);
               if (cykl4.IsInGridScope())
                  cycles.Add(cykl4);
               #endregion [Impl]
            }
         }

         var correctCycles = new List<Cycle>();
         for (int i = 0; i < cycles.Count; i++)
         {
            var cycleToExamine = cycles[i];
            if (IsCycleCorrect(cycleToExamine, CycleElementId))
               correctCycles.Add(cycles[i]);
         }

         if (correctCycles.Count == 1)
            return correctCycles.Single();

         if (correctCycles.Count > 1)
            throw new Exception("Znaleziono więcej niż jeden prawidłowy cykl. Zweryfikuj dane wejściowe");

         return null;
      }


      private string FindMaxElementId()
      {
         var maxEl = m_grid.SelectMany(x => x.Select(v => new { v.Id, v.DeltaNiebazowa }))
                           .Where(x => x.DeltaNiebazowa != null )
                           .OrderByDescending(x => x.DeltaNiebazowa)
                           .FirstOrDefault();

         if (maxEl is null)
         {
            Error = (true, "Nie udało się określić punktu początkowego dla cyklu. Zweryfikuj dane wejściowe.");
            return null;
         }

         if (maxEl.DeltaNiebazowa <= 0)
            return null;

         return maxEl.Id;
      }

      private string FindLeastElementId()
      {
         var minEl = m_grid.SelectMany(x => x.Select(v => new { v.Id, v.DeltaNiebazowa, v.IsVirtual }))
                           .Where(x => x.DeltaNiebazowa != null && x.IsVirtual != true)
                           .OrderBy(x => x.DeltaNiebazowa)
                           .FirstOrDefault();
         if (minEl is null)
         {
            Error = (true, "Nie udało się określić punktu początkowego dla cyklu. Zweryfikuj dane wejściowe.");
            return null;
         }

         if (minEl.DeltaNiebazowa >= 0)
            return null;

         return minEl.Id;
      }


      private bool IsCycleCorrect(Cycle a_cykl, string a_startPointId)
      {
         if (CheckCyklCoordinatesExistInGridScope(a_cykl))
            return CheckIsCyklOnlyOnDeltaNiebazowaNull(a_cykl, a_startPointId);
         return false;
      }


      private bool CheckCyklCoordinatesExistInGridScope(Cycle a_cykl)
      {
         var flatList = m_grid.SelectMany(c => c.Select(cor => cor.Id));
         var isPointAInGrid = flatList.Contains(a_cykl.A.Id);
         var isPointBInGrid = flatList.Contains(a_cykl.B.Id);
         var isPointCInGrid = flatList.Contains(a_cykl.C.Id);
         var isPointDInGrid = flatList.Contains(a_cykl.D.Id);

         if (isPointAInGrid && isPointBInGrid && isPointCInGrid && isPointDInGrid)
            return true;

         return false;
      }


      private bool CheckIsCyklOnlyOnDeltaNiebazowaNull(Cycle a_cykl, string a_startPointId)
      {
         var flatList = m_grid.SelectMany(e => e.Select(cor => new { cor.Id, cor.DeltaNiebazowa }));

         var a = flatList.Single(e => e.Id == a_cykl.A.Id);
         var b = flatList.Single(e => e.Id == a_cykl.B.Id);
         var c = flatList.Single(e => e.Id == a_cykl.C.Id);
         var d = flatList.Single(e => e.Id == a_cykl.D.Id);

         var isPointAOnDeltaNull = a.Id == a_startPointId ? true : a.DeltaNiebazowa is null;
         var isPointBOnDeltaNull = b.Id == a_startPointId ? true : b.DeltaNiebazowa is null;
         var isPointCOnDeltaNull = c.Id == a_startPointId ? true : c.DeltaNiebazowa is null;
         var isPointDOnDeltaNull = d.Id == a_startPointId ? true : d.DeltaNiebazowa is null;

         if (isPointAOnDeltaNull && isPointBOnDeltaNull && isPointCOnDeltaNull && isPointDOnDeltaNull)
            return true;

         return false;
      }


      public int FindPrzydzialDoOptymalizacji()
      {
         var startPositionId = WyznaczonyCykl.Start.Id;
         var points = WyznaczonyCykl.ToPointsList();

         var sp = points.Single(x => x.Id == startPositionId);
         points.Remove(sp);

         var startX = startPositionId[1];
         var p1 = points.Single(x => x.X == int.Parse(startX.ToString()));
         var startY = startPositionId[0];
         var p2 = points.Single(x => x.Y == int.Parse(startY.ToString()));

         var el1 = m_grid[p1.Y][p1.X];
         var el2 = m_grid[p2.Y][p2.X];

         return el1.Przydzial.Value < el2.Przydzial.Value
                  ? el1.Przydzial.Value
                  : el2.Przydzial.Value;
      }

      private void MarkPointsType()
      {
         var cyclePoints = WyznaczonyCykl.ToPointsList();

         var poz_el = WyznaczonyCykl.Start;
         var punktyCyklu = cyclePoints.ToList();
         punktyCyklu.Remove(punktyCyklu.Single(e => e.Id == poz_el.Id));
         var p1_negatywny = punktyCyklu.Single(x => x.Y == int.Parse(poz_el.Id[0].ToString()));
         punktyCyklu.Remove(p1_negatywny);
         var p2_negatywny = punktyCyklu.Single(x => x.X == int.Parse(poz_el.Id[1].ToString()));
         punktyCyklu.Remove(p2_negatywny);
         var p2_pozytywny = punktyCyklu.First();

         poz_el.Type = CyclePoint.CyclePointType.CyklDodatni;
         p1_negatywny.Type = CyclePoint.CyclePointType.CyklUjemny;
         p2_negatywny.Type = CyclePoint.CyclePointType.CyklUjemny;
         p2_pozytywny.Type = CyclePoint.CyclePointType.CyklDodatni;
      }

   }
}
