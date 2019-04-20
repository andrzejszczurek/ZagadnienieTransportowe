using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.Model
{
   public class DataGridCell
   {
      public string Id { get; set; }

      public int? Przydział { get; set; }

      public int KosztyJednostkowe { get; set; }

      public int? DeltaNiebazowa { get; set; }

      public bool? IsInCycle { get; set; }

      public DataGridCell(int x, int y)
      {
         Id = y.ToString() + x.ToString();
      }
   }
}
