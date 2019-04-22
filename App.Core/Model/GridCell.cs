namespace App.Core.Model
{
   public class GridCell
   {
      public string Id { get; }

      public int? Przydzial { get; set; }

      public int KosztyJednostkowe { get; set; }

      public int? DeltaNiebazowa { get; set; }

      public bool IsVirtual { get; set; }


      public GridCell(int x, int y)
      {
         Id = y.ToString() + x.ToString();
         KosztyJednostkowe = 0;
      }
   }
}
