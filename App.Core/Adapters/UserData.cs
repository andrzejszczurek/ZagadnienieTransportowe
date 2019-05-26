using App.Core.Model;
using System.Collections.Generic;

namespace App.Core.Adapters
{
   public class UserData
   {
      /// <summary>
      /// Siatka kosztów jednostkowych.
      /// </summary>
      public GridCell[][] UnitCostGrid { get; set; }

      /// <summary>
      /// Lista Dostawów.
      /// </summary>
      public IEnumerable<InputData> Deliverers { get; set; }

      /// <summary>
      /// Lista Odbiorców.
      /// </summary>
      public IEnumerable<InputData> Customers { get; set; }


      public UserData(GridCell[][] a_unitCosts, IEnumerable<InputData> a_deliverers, IEnumerable<InputData> a_customers)
      {
         UnitCostGrid = a_unitCosts;
         Deliverers = a_deliverers;
         Customers = a_customers;
      }
   }
}
