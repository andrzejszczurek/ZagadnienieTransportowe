namespace App.Core.Model
{
   public class GridCell
   {
      public string Id { get; set; }

      public int? Przydzial { get; set; }

      public int KosztyJednostkowe { get; set; }

      public int? DeltaNiebazowa { get; set; }


      public GridCell(int x, int y)
      {
         Id = y.ToString() + x.ToString();
      }
   }
}
