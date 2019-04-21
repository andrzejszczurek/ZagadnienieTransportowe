using App.Core.Model;
using System.Collections.Generic;

namespace App.Core.Solver
{
   public class UserData
   {
      /// <summary>
      /// Siatka kosztów jednostkowych.
      /// </summary>
      public GridCell[][] SiatkaKosztowJednostkowych { get; set; }

      /// <summary>
      /// Lista Dostawów.
      /// </summary>
      public List<InputData> Dostawcy { get; set; }

      /// <summary>
      /// Lista Odbiorców.
      /// </summary>
      public List<InputData> Odbiorcy { get; set; }


      public UserData(GridCell[][] a_koszty, List<InputData> a_dostawcy, List<InputData> a_odbiorcy)
      {
         SiatkaKosztowJednostkowych = a_koszty;
         Dostawcy = a_dostawcy;
         Odbiorcy = a_odbiorcy;
      }
   }
}
