using App.Core.Solver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace App.Core.Model
{
   public class IterationKosztyTransportu : IterationBase
   {

      public IterationKosztyTransportu(GridCell[][] a_grid, int a_number)
         : base(a_grid, a_number)
      {
      }

      public override void CalculateIterationResult()
      {
         int koszty = 0;
         for (int j = 0; j < DataGrid.Length; j++)
         {
            for (int i = 0; i < DataGrid[j].Length; i++)
            {
               var cell = DataGrid[j][i];
               koszty += cell.KosztyJednostkowe * cell.Przydzial ?? 0;
            }
         }
         IterationResultValue = koszty;
      }


      public override GridCell[][] CalculateNextIteration()
      {
         var cycleDetector = new CycleDetector(DataGrid).Detect();

         if (cycleDetector.Error.IsError)
         {
            Error = cycleDetector.Error;
            return null;
         }

         IsOptimal = cycleDetector.IsOptimal;
         if (cycleDetector.IsOptimal)
         {
            IsCorrect = true;
            return null;
         }

         var cycle = cycleDetector.WyznaczonyCykl;
         Cykl = cycle;
         var przydzial = cycleDetector.FindPrzydzialDoOptymalizacji();

         var nextIterationGrid = Utility.CreateEmptyCellGrid(DataGrid.Length, DataGrid[0].Length);
         var cyclePoints = cycle.ToPointsList();

         var poz_el = cycle.Start.Id;
         var punktyCyklu = cyclePoints.ToList();
         punktyCyklu.Remove(punktyCyklu.Single(e => e.Id == poz_el));
         var p1_negatywny = punktyCyklu.Single(x => x.Y == int.Parse(poz_el[0].ToString()));
         punktyCyklu.Remove(p1_negatywny);
         var p2_negatywny = punktyCyklu.Single(x => x.X == int.Parse(poz_el[1].ToString()));
         punktyCyklu.Remove(p2_negatywny);
         var p2_pozytywny = punktyCyklu.First();

         for (int j = 0; j < DataGrid.Length; j++)
         {
            for (int i = 0; i < DataGrid[j].Length; i++)
            {
               nextIterationGrid[j][i].KosztyJednostkowe = DataGrid[j][i].KosztyJednostkowe;
               nextIterationGrid[j][i].IsVirtual = DataGrid[j][i].IsVirtual;

               var wsp = cyclePoints.SingleOrDefault(e => e.Id == DataGrid[j][i].Id);
               if (wsp is null)
                  nextIterationGrid[j][i].Przydzial = DataGrid[j][i].Przydzial;
               else
               {
                  var isEx = true;
                  if (p1_negatywny.Id == wsp.Id || p2_negatywny.Id == wsp.Id)
                  {
                     if (DataGrid[j][i].Przydzial is null)
                     {
                        DataGrid[j][i].Przydzial = 0;
                        isEx = false;
                     }

                     nextIterationGrid[j][i].Przydzial = DataGrid[j][i].Przydzial - przydzial;
                  }
                  else
                  {
                     if (DataGrid[j][i].Przydzial is null)
                     {
                        DataGrid[j][i].Przydzial = 0;
                        isEx = false;
                     }

                     nextIterationGrid[j][i].Przydzial = DataGrid[j][i].Przydzial + przydzial;
                  }

                  if (nextIterationGrid[j][i].Przydzial == 0 && isEx)
                     nextIterationGrid[j][i].Przydzial = null;
               }
            }
         }
         IsCorrect = true;
         return nextIterationGrid;
      }


      internal override void CalculatePrzydzial(IEnumerable<InputData> a_dostawcy, IEnumerable<InputData> a_odbiorcy)
      {
         for (int y = 0; y < DataGrid.Length; y++)
         {
            for (int x = 0; x < DataGrid[y].Length; x++)
            {
               var cell = DataGrid[y][x];
               var d = a_dostawcy.Single(dos => dos.Id == y);
               var o = a_odbiorcy.Single(odb => odb.Id == x);

               var aktualneZapotrzebowanie = o.Value;
               var aktualnaPodaz = d.Value;

               if (o.Value == 0) // popyt odbiorcy
               {
                  cell.Przydzial = null;
                  continue;
               }

               if (d.Value == 0)
               {
                  cell.Przydzial = null;
                  continue;
               }

               if (aktualneZapotrzebowanie >= aktualnaPodaz)
               {
                  cell.Przydzial = aktualnaPodaz;
                  d.Value = 0;
                  o.Value = o.Value - aktualnaPodaz;
                  continue;
               }
               if (aktualneZapotrzebowanie < aktualnaPodaz)
               {
                  cell.Przydzial = aktualneZapotrzebowanie;
                  d.Value = d.Value - aktualneZapotrzebowanie;
                  o.Value = o.Value - cell.Przydzial ?? 0;
                  continue;
               }
            }
         }
      }



      protected override void CalculateDeltyNiebazowe()
      {
         for (int y = 0; y < DataGrid.Length; y++)
         {
            for (int x = 0; x < DataGrid[y].Length; x++)
            {
               var cell = DataGrid[y][x];
               if (cell.Przydzial != null)
                  cell.DeltaNiebazowa = null;
               else
                  cell.DeltaNiebazowa = cell.KosztyJednostkowe - Alfa[y] - Beta[x];
            }
         }
      }

      protected override void CalculateWspolczynnikiAlfaAndBeta()
      {
         Alfa = new int?[DataGrid.Length];
         Beta = new int?[DataGrid[0].Length];
         Alfa[0] = 0;
         var isCalculated = false;
         var iteration = 0;
         while (!isCalculated)
         {
            for (int b = 0; b < Beta.Length; b++)
            {
               if (Beta[b] != null)
                  continue;

               for (int a = 0; a < Alfa.Length; a++)
               {
                  if (Alfa[a] != null && DataGrid[a][b].Przydzial != null)
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
                  if (Beta[b] != null && DataGrid[a][b].Przydzial != null)
                  {
                     Alfa[a] = DataGrid[a][b].KosztyJednostkowe - Beta[b];
                     break;
                  }
               }
            }

            if (Alfa.All(a => a != null) && Beta.All(b => b != null))
               isCalculated = true;

            if (iteration++ > 10)
            {
               Error = (true, "Nie udało się wyliczyć wspólczynników alfa i beta");
               break;
            }
         }
      }


      internal override void CalculateZysk(IEnumerable<InputData> a_dostawcy, IEnumerable<InputData> a_odbiorcy)
      {
         throw new NotImplementedException("Dla tego rodzaju iteracji nie jest to wymagane przeliczenie.");
      }

   }
}
