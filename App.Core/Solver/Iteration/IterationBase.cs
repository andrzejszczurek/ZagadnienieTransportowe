using App.Core.Enums;
using App.Core.Model;
using App.Core.Solver.CycleProvider;
using System.Collections.Generic;

namespace App.Core.Solver
{
   public abstract class IterationBase
   {
      public JobType JobType { get; set; }

      public GridCell[][] DataGrid { get; set; }

      public Cycle Cycle { get; protected set; }

      public int?[] Alpha { get; protected set; }

      public int?[] Beta { get; protected set; }

      public int Number { get; }

      public bool IsOptimal { get; protected set; }

      public (bool IsError, string Message) Error { get; protected set; }

      public int IterationResultValue { get; protected set; }

      public bool IsCorrect { get; protected set; }


      protected IterationBase(GridCell[][] a_grid, JobType a_jobType, int a_number)
      {
         DataGrid = a_grid;
         Number = a_number;
         JobType = a_jobType;
      }

      internal abstract void CalculatePrzydzial(IEnumerable<InputData> a_dostawcy, IEnumerable<InputData> a_odbiorcy);

      internal abstract void CalculateZysk(IEnumerable<InputData> a_dostawcy, IEnumerable<InputData> a_odbiorcy);

      internal abstract void CalculateWspolczynnikiAlfaAndBeta();

      internal abstract void CalculateDeltyNiebazowe();


      /// <summary>
      /// Przelicza koszty transportu lub zysk w zależności od rodzaju iteracji.
      /// </summary>
      public abstract void CalculateIterationResult();


      /// <summary>
      /// Wyznacza siatkę dla kolejnej iteracji.
      /// </summary>
      public abstract GridCell[][] CalculateNextIteration();


      /// <summary>
      /// Przelicza przydział, współczynniki alfa i beta, delty niebazowe oraz zoptymalizowany wynik dla siatki
      /// </summary>
      public void CalculateGridInit(IEnumerable<InputData> a_dostawcy, IEnumerable<InputData> a_odbiorcy)
      {
         if (JobType == JobType.Profit)
            CalculateZysk(a_dostawcy, a_odbiorcy);
         CalculatePrzydzial(a_dostawcy, a_odbiorcy);
         CalculateGrid(a_dostawcy, a_odbiorcy);
      }


      /// <summary>
      /// Przelicza współczynniki alfa i beta, delty niebazowe oraz koszty transportu dla siatki
      /// </summary>
      public void CalculateGrid(IEnumerable<InputData> a_dostawcy, IEnumerable<InputData> a_odbiorcy)
      {
         CalculateWspolczynnikiAlfaAndBeta();
         if (Error.IsError)
            return;
         CalculateDeltyNiebazowe();
         CalculateIterationResult();
      }

   }
}
