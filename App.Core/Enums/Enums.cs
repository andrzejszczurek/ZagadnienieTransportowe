namespace App.Core.Enums
{
   /// <summary>
   /// Typ punktu w cyklu.
   /// </summary>
   public enum CyclePointType
   {
      Positive,
      Negative,
   }

   /// <summary>
   /// Typ bazy dla cyklu.
   /// </summary>
   public enum CycleBaseType
   {
      Maximizing,
      Minimizing
   }

   /// <summary>
   /// Rodzaj zadania do przeliczenia dla solvera.
   /// </summary>
   public enum JobType
   {
      TransportCosts,
      Profit
   }

   /// <summary>
   /// Rodzaj danych wejściowych.
   /// </summary>
   public enum InputType
   {
      Dostawca,
      Odbiorca
   }

}
