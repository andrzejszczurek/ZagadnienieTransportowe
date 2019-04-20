using App.Core.Solver;
using System.Collections.Generic;
using System.Linq;

namespace App.Core.Model
{
   public class Iteration
   {
      public DataGridCell[][] DataGrid { get; set; }

      public int?[] Alfa { get; set; }

      public int?[] Beta { get; set; }

      public int KosztyTransportu { get; set; }

      public Iteration(DataGridCell[][] a_grid)
      {
         DataGrid = a_grid;
      }

      public void CalculateGrid(IEnumerable<InputData> a_dostawcy, IEnumerable<InputData> a_odbiorcy)
      {
         CalculatePrzydzial(a_dostawcy, a_odbiorcy);
         CalculateWspolczynnikiAlfaAndBeta(a_dostawcy, a_odbiorcy);
         CalculateDeltyNiebazowe();
         KosztyTransportu = CalculateKosztyTransportu();
      }

      public void CalculatePrzydzial(IEnumerable<InputData> a_dostawcy, IEnumerable<InputData> a_odbiorcy)
      {
         for (int y = 0; y < DataGrid.Length; y++)
         {
            for (int x = 0; x < DataGrid[y].Length; x++)
            {
               var cell = DataGrid[y][x];
               var d = a_dostawcy.Single(dos => dos.Id == y+1);
               var o = a_odbiorcy.Single(odb => odb.Id == x+1);

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

      public void CalculateWspolczynnikiAlfaAndBeta(IEnumerable<InputData> a_dostawcy, IEnumerable<InputData> a_odbiorcy)
      {
         Alfa = new int?[a_dostawcy.Count()];
         Beta = new int?[a_odbiorcy.Count()];
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

      public int CalculateKosztyTransportu()
      {
         int koszty = 0;
         for (int j = 0; j < DataGrid.Length; j++)
         {
            for (int i = 0; i < DataGrid[j].Length; i++)
            {
               var cell = DataGrid[j][i];
               koszty += cell.KosztyJednostkowe * cell.Przydział ?? 0;
            }
         }
         return koszty;
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

      
      public DataGridCell[][] CalculateNextIteration()
      {
         var cycleDetector = new CycleDetector(DataGrid).Detect();
         var cycle = cycleDetector.WyznaczonyCykl;
         var przydzial = cycleDetector.FindPrzydzialDoOptymalizacji();

         var nextIterationGrid = Utility.CreateEmptyGrid(DataGrid.Length, DataGrid[0].Length);
         var cyclePoints = cycle.ToPointsList();

         var neg_el = cycle.Start.Id;
         var punktyCyklu = cyclePoints.ToList();
         punktyCyklu.Remove(punktyCyklu.Single(e => e.Id == neg_el));
         var p1_negatywny = punktyCyklu.Single(x => x.Y == int.Parse(neg_el[0].ToString()));
         punktyCyklu.Remove(p1_negatywny);
         var p2_negatywny = punktyCyklu.Single(x => x.X == int.Parse(neg_el[1].ToString()));
         punktyCyklu.Remove(p2_negatywny);
         var p2_pozytywny = punktyCyklu.First();

         for (int j = 0; j < DataGrid.Length; j++)
         {
            for (int i = 0; i < DataGrid[j].Length; i++)
            {
               nextIterationGrid[j][i].KosztyJednostkowe = DataGrid[j][i].KosztyJednostkowe;

               var wsp = cyclePoints.SingleOrDefault(e => e.Id == DataGrid[j][i].Id);
               if (wsp is null)
                  nextIterationGrid[j][i].Przydział = DataGrid[j][i].Przydział;
               else
               {
                  var isEx = true;
                  if (p1_negatywny.Id == wsp.Id || p2_negatywny.Id == wsp.Id)
                  {
                     if (DataGrid[j][i].Przydział is null)
                     {
                        DataGrid[j][i].Przydział = 0;
                        isEx = false;
                     }

                     nextIterationGrid[j][i].Przydział = DataGrid[j][i].Przydział - przydzial;
                  }
                  else
                  {
                     if (DataGrid[j][i].Przydział is null)
                     {
                        DataGrid[j][i].Przydział = 0;
                        isEx = false;
                     }

                     nextIterationGrid[j][i].Przydział = DataGrid[j][i].Przydział + przydzial;
                  }

                  if (nextIterationGrid[j][i].Przydział == 0 && isEx)
                     nextIterationGrid[j][i].Przydział = null;
               }
            }
         }
         return nextIterationGrid;
      }

   }
}
