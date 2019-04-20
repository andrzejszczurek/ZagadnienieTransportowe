using System;
using System.Collections.Generic;
using System.Linq;
using static App.Core.Model.Cykl;

namespace App.Core.Model
{
   public class IterationGrid
   {

      public List<InputData> Dostawcy { get; set; }

      public List<InputData> Odbiorcy { get; set; }

      public DataGridCell[][] DataGrid { get; set; }

      public int?[] Alfa { get; set; }

      public int?[] Beta { get; set; }

      public IterationGrid()
      {
         Dostawcy = new List<InputData>();
         Odbiorcy = new List<InputData>();
      }

      internal DataGridCell[][] InitDataGrid()
      {
         for (int y = 0; y < DataGrid.Length; y++)
         {
            for (int x = 0; x < DataGrid[y].Length; x++)
            {
               DataGrid[y][x] = new DataGridCell(x, y);
            }
         }
         return DataGrid;
      }

      public void AddDostawca(int a_value)
      {
         var supp = new InputData(Dostawcy.Count + 1, InputType.Dostawca, a_value);
         Dostawcy.Add(supp);
      }

      public void AddOdbiorca(int a_value)
      {
         var supp = new InputData(Odbiorcy.Count + 1, InputType.Odbiorca, a_value);
         Odbiorcy.Add(supp);
      }

      public void CreateGrid()
      {
         DataGrid = Extenssions.Init2DimmArray<DataGridCell>(Odbiorcy.Count, Dostawcy.Count);
         InitDataGrid();

      }

      public void AddKosztyJednostkowe(int y, int x, int a_value)
      {
         DataGrid[y][x].KosztyJednostkowe = a_value;
      }

      public void CalculatePrzydzial()
      {
         for (int y = 0; y < DataGrid.Length; y++)
         {
            for (int x = 0; x < DataGrid[y].Length; x++)
            {
               var cell = DataGrid[y][x];
               var d = Dostawcy.Single(dos => dos.Id == y+1);
               var o = Odbiorcy.Single(odb => odb.Id == x+1);

               var aktualneZapotrzebowanie = o.Value;
               var aktualnaPodaz = d.Value;

               if (o.Value == 0)
               {
                  cell.Przydział = null;
                  continue;
               }

               if (d.Value == 0)
               {
                  cell.Przydział = null;
                  continue;
               }

               if (aktualneZapotrzebowanie >= aktualnaPodaz)
               {
                  cell.Przydział = aktualnaPodaz;
                  d.Value = 0;
                  o.Value = o.Value - aktualnaPodaz;
                  continue;
               }
               if (aktualneZapotrzebowanie < aktualnaPodaz)
               {
                  cell.Przydział = aktualneZapotrzebowanie;
                  d.Value = d.Value - aktualneZapotrzebowanie;
                  o.Value = o.Value - cell.Przydział ?? 0;
                  continue;
               }
            }
         }
      }

      public void CalculateWspolczynnikiAlfaAndBeta()
      {
         Alfa = new int?[Dostawcy.Count];
         Beta = new int?[Odbiorcy.Count];
         Alfa[0] = 0;
         var isCalculated = false;
         while (!isCalculated)
         {
            for (int b = 0; b < Beta.Length; b++)
            {
               if (Beta[b] != null)
                  continue;

               for (int a = 0; a < Alfa.Length; a++)
               {
                  if (Alfa[a] != null && DataGrid[a][b].Przydział != null)
                  {
                     Beta[b] = DataGrid[a][b].KosztyJednostkowe - Alfa[a];
                     break;
                  } 
               }
            }

            for (int a = 0; a < Alfa.Length; a++)
            {
               if (Alfa[a] != null)
                  continue;

               for (int b = 0; b < Beta.Length; b++)
               {
                  if (Beta[b] != null && DataGrid[a][b].Przydział != null)
                  {
                     Alfa[a] = DataGrid[a][b].KosztyJednostkowe - Beta[b];
                     break;
                  }
               }
            }

            if (Alfa.All(a => a != null) && Beta.All(b => b != null))
               isCalculated = true;
         }
      }

      public void CalculateDeltyNiebazowe()
      {
         for (int y = 0; y < DataGrid.Length; y++)
         {
            for (int x = 0; x < DataGrid[y].Length; x++)
            {
               var cell = DataGrid[y][x];
               if (cell.Przydział != null)
                  cell.DeltaNiebazowa = null;
               else
                  cell.DeltaNiebazowa = cell.KosztyJednostkowe - Alfa[y] - Beta[x];
            }
         }
      }

      public Cykl FindCycle()
      {
         var startPositionId = FindNegativeElementIdIfExist();
         var startY = int.Parse(startPositionId[0].ToString());
         var startX = int.Parse(startPositionId[1].ToString());

         var cycles = new List<Cykl>();

         for (int i = 1; i < Dostawcy.Count; i++)
         {
            for (int j = 1; j < Odbiorcy.Count; j++)
            {
               var a = new Punkt(startY, startX);

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
            }
         }

         var correctCycles = new List<Cykl>();

         for (int i = 0; i < cycles.Count; i++)
         {
            var cycleToExamine = cycles[i];
            if (IsCycleCorrect(cycleToExamine, startPositionId))
               correctCycles.Add(cycles[i]);
         }

         if (correctCycles.Count == 1)
            return correctCycles.First();

         if (correctCycles.Count > 1)
            throw new Exception("Znaleziono więcej niż jeden prawidłowy cykl. Zweryfikuj dane wejściowe");

         return null;
      }

      private string FindNegativeElementIdIfExist()
      {
         var minEl = DataGrid.SelectMany(x => x.Select(v => new { v.Id, v.DeltaNiebazowa }))
                           .Where(x => x.DeltaNiebazowa != null)
                           .OrderBy(x => x.DeltaNiebazowa)
                           .First();
         if (minEl.DeltaNiebazowa > 0)
            return null;

         return minEl.Id;
      }

      public bool IsCycleCorrect(Cykl a_cykl, string a_startPointId)
      {
         if (CheckCyklCoordinatesExistInGridScope(a_cykl))
            return CheckIsCyklOnlyOnDeltaNiebazowaNull(a_cykl, a_startPointId);
         return false;
      }

      public bool CheckCyklCoordinatesExistInGridScope(Cykl a_cykl)
      {
         var flatList = DataGrid.SelectMany(c => c.Select(cor => cor.Id));
         var isPointAInGrid = flatList.Contains(a_cykl.A.Id);
         var isPointBInGrid = flatList.Contains(a_cykl.B.Id);
         var isPointCInGrid = flatList.Contains(a_cykl.C.Id);
         var isPointDInGrid = flatList.Contains(a_cykl.D.Id);

         if (isPointAInGrid && isPointBInGrid && isPointCInGrid && isPointDInGrid)
            return true;

         return false;
      }

      public bool CheckIsCyklOnlyOnDeltaNiebazowaNull(Cykl a_cykl, string a_startPointId)
      {
         var flatList = DataGrid.SelectMany(e => e.Select(cor => new { cor.Id, cor.DeltaNiebazowa }));

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
   }
}
