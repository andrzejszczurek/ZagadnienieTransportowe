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
         var macierzList = DataGrid.SelectMany(c => c.Where(cc => cc.IsVirtual =! true).Select(cell => cell)).ToList();
         var ml = macierzList.ToList();
         foreach (var cell in ml)
         {
            var maxCell = macierzList.First(c => c.Zysk == macierzList.Max(z => c.Zysk));
            macierzList.Remove(maxCell);

            var d = a_dostawcy.Single(dos => dos.Id == maxCell.Id[0]);
            var o = a_odbiorcy.Single(odb => odb.Id == maxCell.Id[1]);

            var aktualneZapotrzebowanie = o.Value;
            var aktualnaPodaz = d.Value;

            if (o.Value == 0)
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

            var vd = a_dostawcy.Single(ds => ds.IsVirtual);
            var vo = a_odbiorcy.Single(ob => ob.IsVirtual);
            for (int j = 0; j < vd.Id; j++)
            {
               var cc = DataGrid[j][DataGrid[j].Length];
               cc.Zysk = 0;
               cc.Przydzial = vd.Value == 0 ? null : (int?)vd.Value;
            }


            foreach (var c in DataGrid[vd.Id])
            {
               var odbiorca = a_odbiorcy.Single(odbio => odbio.Id == c.Id[1]);
               c.Zysk = 0;
               c.Przydzial = odbiorca.Value == 0 ? null : (int?)odbiorca.Value;
            }
         }
         #endregion [Impl]
      }


      protected override void CalculateWspolczynnikiAlfaAndBeta()
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
         throw new NotImplementedException();
      }


      public (InputData Dostawca, InputData Odbiorca) CreateVirtualInputData(IEnumerable<InputData> a_dostawcy, IEnumerable<InputData> a_odbiorcy)
      {
         var popyt = a_odbiorcy.Sum(o => o.Value);
         var podaz = a_dostawcy.Sum(o => o.Value);

         var virtualOdbiorca = new InputData(a_odbiorcy.Count(), InputType.Odbiorca, popyt, 0, true);
         var virtualDostawca = new InputData(a_odbiorcy.Count(), InputType.Odbiorca, podaz, 0, true);

         return (virtualDostawca, virtualOdbiorca);
      }

   }
}
