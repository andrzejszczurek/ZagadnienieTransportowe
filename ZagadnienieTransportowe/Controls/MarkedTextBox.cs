using System;
using System.Windows.Forms;

namespace ZagadnienieTransportowe.Controls
{
   public class MarkedTextBox : TextBox
   {
      public Guid UniqueId { get; set; }

      public MarkedTextBox()
         : base()
      {
         UniqueId = Guid.NewGuid();
      }
   }
}
