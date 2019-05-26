using App.Core.Solver;

namespace App.Core.Adapters
{
   public interface IUserDataAdapter<T> where T: IUserDataGridDefinition
   {
      /// <summary>
      /// Tworzy dane wejściowe dla solvera na podstawie danych z kontrolek.
      /// </summary>
      UserData Adapt(T a_dataGrid);
   }
}
