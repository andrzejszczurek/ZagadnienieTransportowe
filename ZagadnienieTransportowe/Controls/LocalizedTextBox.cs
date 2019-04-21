using System;
using System.Windows.Forms;

namespace ZagadnienieTransportowe.Controls
{
   public class LocalizedTextBox : TextBox
   {
      public Guid UniqueId { get; set; }

      public LocalizedTextBox()
         : base()
      {
         UniqueId = Guid.NewGuid();
      }
   }
}
