namespace App.Core.Model
{
   public enum InputType
   {
      Dostawca,
      Odbiorca
   }


   public class InputData
   {
      public int Id { get; }

      public InputType Type { get; }

      public int Value { get; set; }


      public InputData(int a_id, InputType a_inputType, int a_value)
      {
         Id = a_id;
         Type = a_inputType;
         Value = a_value;
      }
   }
}
