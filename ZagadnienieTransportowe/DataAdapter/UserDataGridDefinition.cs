using App.Core.Adapters;
using System.Collections.Generic;
using ZagadnienieTransportowe.Controls;

namespace ZagadnienieTransportowe.DataAdapter
{
   internal class UserDataGridDefinition : IUserDataGridDefinition
   {
      internal IDictionary<string, LocalizedTextBox> Cells;

      internal IDictionary<int, (LocalizedTextBox Popyt, LocalizedTextBox Cena)> Odbiorcy;

      internal IDictionary<int, (LocalizedTextBox Podaz, LocalizedTextBox Cena)> Dostawcy;
   }
}
