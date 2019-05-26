using App.Core.Enums;

namespace App.Core.Model
{ 

   public class InputData
   {
      public int Id { get; }

      public InputType Type { get; }

      public int Value { get; set; }

      public bool IsVirtual { get; set; }

      public int? Price { get; set; }


      public InputData(int a_id, InputType a_inputType, int a_value)
         : this(a_id, a_inputType, a_value, false)
      {

      }


      public InputData(int a_id, InputType a_inputType, int a_value, bool a_isVirtual)
         : this(a_id, a_inputType, a_value, null, a_isVirtual)
      {

      }


      public InputData(int a_id, InputType a_inputType, int a_value, int a_price)
         : this(a_id, a_inputType, a_value, a_price, false)
      {

      }


      public InputData(int a_id, InputType a_inputType, int a_value, int? a_price, bool a_isVirtual)
      {
         Id = a_id;
         Type = a_inputType;
         Value = a_value;
         IsVirtual = a_isVirtual;
         Price = a_price;
      }
   }
}
