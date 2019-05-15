using App.Core.Solver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.Model
{
   public class IterationZysk : IterationBase 
   {

      public IterationZysk(GridCell[][] a_grid, int a_number)
         : base(a_grid, a_number)
      {
      }


      internal override void CalculateZysk(IEnumerable<InputData> a_dostawcy, IEnumerable<InputData> a_odbiorcy)
      {
         for (int j = 0; j < DataGrid.Length; j++)
         {
            for (int i = 0; i < DataGrid[j].Length; i++)
            {
               var dostawca = a_dostawcy.Single(d => d.Id == j);
               var odbiorca = a_odbiorcy.Single(o => o.Id == i);
               var cell = DataGrid[j][i];

               var zysk = odbiorca.Price - dostawca.Price - cell.KosztyJednostkowe;
               cell.Zysk = zysk;
            }
         }
      }


      internal override void CalculatePrzydzial(IEnumerable<InputData> a_dostawcy, IEnumerable<InputData> a_odbiorcy)
      {
         #region [Impl]
         var macierzList = DataGrid.SelectMany(c => c.Select(cell => cell)).Where(cc => cc.IsVirtual != true).ToList();
         var ml = macierzList.ToList();
         foreach (var cell in ml)
         {
            var maxZysk = macierzList.Where(z => z.Zysk != null).Max(z => z.Zysk);
            var maxCell = macierzList.First(c => c.Zysk == maxZysk);
            macierzList.Remove(maxCell);

            var d = a_dostawcy.Where(dos => dos.IsVirtual != true).Single(dos => dos.Id == int.Parse(maxCell.Id[0].ToString()));
            var o = a_odbiorcy.Where(dos => dos.IsVirtual != true).Single(odb => odb.Id == int.Parse(maxCell.Id[1].ToString()));

            var aktualneZapotrzebowanie = o.Value;
            var aktualnaPodaz = d.Value;

            if (o.Value == 0)
            {
               maxCell.Przydzial = null;
               continue;
            }

            if (d.Value == 0)
            {
               maxCell.Przydzial = null;
               continue;
            }

            if (aktualneZapotrzebowanie >= aktualnaPodaz)
            {
               maxCell.Przydzial = aktualnaPodaz;
               d.Value = 0;
               o.Value = o.Value - aktualnaPodaz;
               continue;
            }
            if (aktualneZapotrzebowanie < aktualnaPodaz)
            {
               maxCell.Przydzial = aktualneZapotrzebowanie;
               d.Value = d.Value - aktualneZapotrzebowanie;
               o.Value = o.Value - cell.Przydzial ?? 0;
               continue;
            }
         }

         var vd = a_dostawcy.Single(ds => ds.IsVirtual);
         var vo = a_odbiorcy.Single(ob => ob.IsVirtual);
         for (int j = 0; j < vd.Id; j++)
         {
            var cc = DataGrid[j][DataGrid[j].Length - 1];
            cc.Zysk = 0;

            var podazDos = a_dostawcy.Single(d => d.Id == j).Value;
            cc.Przydzial = podazDos == 0 ? null : (int?)podazDos;
            vo.Value =  vo.Value - podazDos;
         }

         foreach (var c in DataGrid[vd.Id])
         {
            var odbiorca = a_odbiorcy.Single(odbio => odbio.Id == int.Parse(c.Id[1].ToString()));
            c.Zysk = 0;
            c.Przydzial = odbiorca.Value == 0 ? null : (int?)odbiorca.Value;
         }

         #endregion [Impl]
      }


      internal override void CalculateWspolczynnikiAlfaAndBeta()
      {
         #region [Impl]
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
                     Beta[b] = DataGrid[a][b].Zysk - Alfa[a];
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
                     Alfa[a] = DataGrid[a][b].Zysk - Beta[b];
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
         #endregion [Impl]
      }


      internal override void CalculateDeltyNiebazowe()
      {
         for (int y = 0; y < DataGrid.Length; y++)
         {
            for (int x = 0; x < DataGrid[y].Length; x++)
            {
               var cell = DataGrid[y][x];
               if (cell.Przydzial != null)
                  cell.DeltaNiebazowa = null;
               else
                  cell.DeltaNiebazowa = cell.Zysk - Alfa[y] - Beta[x];
            }
         }
      }


      public override void CalculateIterationResult()
      {
         int zyski = 0;
         for (int j = 0; j < DataGrid.Length; j++)
         {
            for (int i = 0; i < DataGrid[j].Length; i++)
            {
               var cell = DataGrid[j][i];
               zyski += cell.Zysk * cell.Przydzial ?? 0;
            }
         }
         IterationResultValue = zyski;
      }


      public override GridCell[][] CalculateNextIteration()
      {
         var cycleDetector = new CycleDetector(DataGrid, CycleDetector.CycleType.Positive).Detect();

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
               nextIterationGrid[j][i].Zysk = DataGrid[j][i].Zysk;
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


      public static (InputData Dostawca, InputData Odbiorca) CreateVirtualInputData(IEnumerable<InputData> a_dostawcy, IEnumerable<InputData> a_odbiorcy)
      {
         var popyt = a_dostawcy.Sum(o => o.Value);
         var podaz = a_odbiorcy.Sum(o => o.Value);

         var virtualOdbiorca = new InputData(a_odbiorcy.Count(), InputType.Odbiorca, popyt, 0, true);
         var virtualDostawca = new InputData(a_dostawcy.Count(), InputType.Odbiorca, podaz, 0, true);

         return (virtualDostawca, virtualOdbiorca);
      }

   }
}
