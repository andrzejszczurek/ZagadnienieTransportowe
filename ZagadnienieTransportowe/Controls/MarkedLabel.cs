using System.Windows.Forms;

namespace ZagadnienieTransportowe.Controls
{
   public class LocalizedLabel : Label
   {
      public enum LocalizatorType
      {
         OdbiorcaInfo,
         DostawcaInfo,
         TabelaOptymalizacji,
         TabelaPrzydzial
      }

      public string GridPosition { get; set; }

      public LocalizatorType Localizator { get; set; }

      public LocalizedLabel()
      {

      }

   }
}
