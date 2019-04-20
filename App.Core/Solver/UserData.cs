using System.Collections.Generic;

namespace App.Core.Solver
{
   public class UserData
   {
      /// <summary>
      /// Słownik kosztów jednostkowych podanych przez użytkownika.
      /// Klucz to pozycja na gridzie (y + x).
      /// </summary>
      public Dictionary<string, int> KosztyJednostkowe { get; set; }

      /// <summary>
      /// Słownik dostawców z wartościa podaży.
      /// Klucz to indeks dla pozycji na gridzie
      /// </summary>
      public Dictionary<string, int> Dostawcy { get; set; }

      /// <summary>
      /// Słownik odbiorców z wartością popytu.
      /// Klucz to indeks dla pozycji na gridzie
      /// </summary>
      public Dictionary<string, int> Odbiorcy { get; set; }
   }
}
