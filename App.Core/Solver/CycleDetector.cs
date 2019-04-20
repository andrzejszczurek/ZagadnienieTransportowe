using App.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace App.Core.Solver
{
   public class CycleDetector
   {
      private DataGridCell[][] m_grid;

      public string NegativeElementId { get; set; }

      public Cykl WyznaczonyCykl { get; set; }


      public CycleDetector(DataGridCell[][] a_grid)
      {
         m_grid = a_grid;
         NegativeElementId = FindNegativeElementId();
      }

      public CycleDetector Detect()
      {
         WyznaczonyCykl = FindCycle();
         return this;
      }


      private Cykl FindCycle()
      { 
         var startY = int.Parse(NegativeElementId[0].ToString());
         var startX = int.Parse(NegativeElementId[1].ToString());

         var cycles = new List<Cykl>();
         for (int i = 1; i < m_grid.Length; i++)
         {
            for (int j = 1; j < m_grid[i].Length; j++)
            {
               #region [Impl]
               var a = new Punkt(startY, startX, true);

               var b1 = new Punkt(startY, startX - i);
               var c1 = new Punkt(startY - j, startX - i);
               var d1 = new Punkt(startY - j, startX);
               var cykl1 = new Cykl(a, b1, c1, d1);
               if (cykl1.IsPositive())
                  cycles.Add(cykl1);

               var b2 = new Punkt(startY, startX - i);
               var c2 = new Punkt(startY + j, startX - i);
               var d2 = new Punkt(startY + j, startX);
               var cykl2 = new Cykl(a, b2, c2, d2);
               if (cykl2.IsPositive())
                  cycles.Add(cykl2);

               var b3 = new Punkt(startY, startX + i);
               var c3 = new Punkt(startY + j, startX + i);
               var d3 = new Punkt(startY + j, startX);
               var cykl3 = new Cykl(a, b3, c3, d3);
               if (cykl3.IsPositive())
                  cycles.Add(cykl3);

               var b4 = new Punkt(startY, startX + i);
               var c4 = new Punkt(startY - j, startX + i);
               var d4 = new Punkt(startY - j, startX);
               var cykl4 = new Cykl(a, b4, c4, d4);
               if (cykl4.IsPositive())
                  cycles.Add(cykl4);
               #endregion [Impl]
            }
         }

         var correctCycles = new List<Cykl>();
         for (int i = 0; i < cycles.Count; i++)
         {
            var cycleToExamine = cycles[i];
            if (IsCycleCorrect(cycleToExamine, NegativeElementId))
               correctCycles.Add(cycles[i]);
         }

         if (correctCycles.Count == 1)
            return correctCycles.Single();

         if (correctCycles.Count > 1)
            throw new Exception("Znaleziono więcej niż jeden prawidłowy cykl. Zweryfikuj dane wejściowe");

         return null;
      }


      private string FindNegativeElementId()
      {
         var minEl = m_grid.SelectMany(x => x.Select(v => new { v.Id, v.DeltaNiebazowa }))
                           .Where(x => x.DeltaNiebazowa != null)
                           .OrderBy(x => x.DeltaNiebazowa)
                           .First();
         if (minEl.DeltaNiebazowa > 0)
            return null; // co wtedy?

         return minEl.Id;
      }


      private bool IsCycleCorrect(Cykl a_cykl, string a_startPointId)
      {
         if (CheckCyklCoordinatesExistInGridScope(a_cykl))
            return CheckIsCyklOnlyOnDeltaNiebazowaNull(a_cykl, a_startPointId);
         return false;
      }


      private bool CheckCyklCoordinatesExistInGridScope(Cykl a_cykl)
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


      private bool CheckIsCyklOnlyOnDeltaNiebazowaNull(Cykl a_cykl, string a_startPointId)
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

         return el1.Przydział.Value < el2.Przydział.Value
                  ? el1.Przydział.Value
                  : el2.Przydział.Value;
      }

   }
}
