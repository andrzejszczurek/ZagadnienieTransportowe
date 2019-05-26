using App.Core;
using App.Core.Adapters;
using App.Core.Enums;
using App.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ZagadnienieTransportowe.DataAdapter
{
   internal class UserDataAdapter : IUserDataAdapter<UserDataGridDefinition>
   {

      public UserData Adapt(UserDataGridDefinition a_data)
      {
         var cells = CreateGridCell(a_data);
         var dos = CreateDostawcy(a_data);
         var odb = CreateOdbiorcy(a_data);

         return new UserData(cells, dos, odb);
      }


      private GridCell[][] CreateGridCell(UserDataGridDefinition a_data)
      {
         var us_grid = Utility.CreateEmptyCellGrid(a_data.Dostawcy.Count(), a_data.Odbiorcy.Count());
         foreach (var cell in a_data.Cells)
         {
            var y = int.Parse(cell.Key[0].ToString());
            var x = int.Parse(cell.Key[1].ToString());

            if (us_grid[y][x].Id != cell.Key)
               throw new Exception($"Podczas przygotowania danych wystąpił bład. Oczekiwane Id '{cell.Key}', pobrane '{us_grid[y][x]?.Id ?? "-"}'");

            us_grid[y][x].KosztyJednostkowe = int.Parse(cell.Value.Text);
         }
         return us_grid;
      }


      private IEnumerable<InputData> CreateOdbiorcy(UserDataGridDefinition a_data)
      {
         var us_odbiorcy = new List<InputData>();
         foreach (var o in a_data.Odbiorcy)
         {
            var data = new InputData(o.Key, InputType.Odbiorca, int.Parse(o.Value.Popyt.Text), int.Parse(o.Value.Cena.Text));
            us_odbiorcy.Add(data);
         }
         return us_odbiorcy;
      }


      private IEnumerable<InputData> CreateDostawcy(UserDataGridDefinition a_data)
      {
         var us_dostawcy = new List<InputData>();
         foreach (var d in a_data.Dostawcy)
         {
            var data = new InputData(d.Key, InputType.Dostawca, int.Parse(d.Value.Podaz.Text), int.Parse(d.Value.Cena.Text));
            us_dostawcy.Add(data);
         }
         return us_dostawcy;
      }
   }
}
