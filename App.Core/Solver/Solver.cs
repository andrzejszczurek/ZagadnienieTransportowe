using App.Core.Adapters;
using App.Core.Enums;
using App.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace App.Core.Solver
{
   public class Solver
   {
      private readonly UserData m_userData;

      private IterationBase m_startIteration;

      private bool m_isInit;

      private int m_iterator;


      public List<InputData> Dostawcy { get; }

      public List<InputData> Odbiorcy { get; }

      public List<IterationBase> Iterations { get; }

      public string ErrorMessage { get; private set; }

      public int AttemptsLimit { get; set; }

      public bool OptimalSolutionFound { get; private set; }



      public Solver(UserData a_userData)
      {
         m_userData = a_userData;
         Dostawcy = a_userData is null ? new List<InputData>() : m_userData.Deliverers.ToList(); // todo: to powinny być kopia pełna 
         Odbiorcy = a_userData is null ? new List<InputData>() : m_userData.Customers.ToList(); // todo: to powinny być kopia pełna
         Iterations = new List<IterationBase>();
         m_isInit = false;
         m_iterator = 0;
         AttemptsLimit = 10;
      }


      /// <summary>
      /// Inicjalizuje początkową iterację dla zadanych dostawców i odbiorców.
      /// Jeżeli podczas tworzenia instancji solvera nie zostały przekazane dane wejściowe, 
      /// po wykonaniu Init należy dodać koszty.
      /// Po wykonaniu Init, brak możliwości definiowania dostawców i odbiorców.
      /// </summary>
      public void Init(JobType a_jobType)
      {
         IterationBase iteracjaStartowa = null;
         var isUserDataSet = !(m_userData is null);

         if (!isUserDataSet && (Dostawcy.Count < 1 || Odbiorcy.Count < 1))
            throw new Exception("Przed zainicjowaniem solvera należy dodać conajmniej jednego dostawcę oraz odbiorcę.");

         GridCell[][] grid = isUserDataSet
                     ? m_userData.UnitCostGrid
                     : Utility.CreateEmptyCellGrid(Dostawcy.Count, Odbiorcy.Count);

         if (a_jobType == JobType.TransportCosts)
         {
            var isBalaced = IsBalanced();
            if (!isBalaced)
               AddVirtualInputData();

            if (!isBalaced && isUserDataSet)
            {
               RecalculateDataGrid();
               grid = m_userData.UnitCostGrid;
            }
            iteracjaStartowa = new IterationTransportCosts(grid, JobType.TransportCosts, ++m_iterator);
         }

         if (a_jobType == JobType.Profit)
         {
            var (vDostawca, vOdbiorca) = IterationProfit.CreateVirtualInputData(Dostawcy, Odbiorcy);
            Dostawcy.Add(vDostawca);
            Odbiorcy.Add(vOdbiorca);
            RecalculateDataGrid();
            grid = m_userData.UnitCostGrid;
            iteracjaStartowa = new IterationProfit(grid, JobType.Profit, ++m_iterator);
         }

         m_startIteration = iteracjaStartowa;
         Iterations.Add(iteracjaStartowa);
         m_isInit = true;

      }


      private void RecalculateDataGrid()
      {
         var virtualGrid = Utility.CreateEmptyCellGrid(Dostawcy.Count, Odbiorcy.Count);
         for (int y = 0; y < m_userData.UnitCostGrid.Length; y++)
         {
            for (int x = 0; x < m_userData.UnitCostGrid[y].Length; x++)
            {
               var cell = m_userData.UnitCostGrid[y][x];
               var newCell = new GridCell(x, y);
               newCell.KosztyJednostkowe = cell.KosztyJednostkowe;
               virtualGrid[y][x] = newCell;
            }
         }

         var virtualOdbiorcaIndex = Odbiorcy.SingleOrDefault(e => e.IsVirtual)?.Id;
         var virtualDostawcaIndex = Dostawcy.SingleOrDefault(e => e.IsVirtual)?.Id;
         for (int y = 0; y < virtualGrid.Length; y++)
         {
            for (int x = 0; x < virtualGrid[0].Length; x++)
            {
               if (virtualOdbiorcaIndex == x)
                  virtualGrid[y][x].IsVirtual = true;

               if (virtualDostawcaIndex == y)
                  virtualGrid[y][x].IsVirtual = true;
            }
         }

         m_userData.UnitCostGrid = virtualGrid;
      }

      private void AddVirtualInputData()
      {
         var popyt = Odbiorcy.Sum(o => o.Value);
         var podaz = Dostawcy.Sum(o => o.Value);
         if (popyt > podaz)
         {
            Dostawcy.Add(new InputData(Dostawcy.Count, InputType.Dostawca, popyt - podaz, true));
         }
         else
            Odbiorcy.Add(new InputData(Odbiorcy.Count, InputType.Odbiorca, podaz - popyt, true));
      }


      /// <summary>
      /// Rozwiązuje zadanie w iteraciach.
      /// </summary>
      public void Resolve(JobType a_jobType)
      {
         CanByResolve();

         var attempt = 0;
         IterationBase iteracja = m_startIteration;
         int? aktulaneWartoscOptymalna = null;
         OptimalSolutionFound = false;

         iteracja.CalculateGridInit(Dostawcy, Odbiorcy);
         aktulaneWartoscOptymalna = iteracja.IterationResultValue;

         while (!OptimalSolutionFound)
         {
            attempt++;
            var nextStepGrid = iteracja.CalculateNextIteration();

            if (iteracja.Error.IsError)
            {
               ErrorMessage = iteracja.Error.Message;
               return;
            }

            if (iteracja.IsOptimal)
            {
               OptimalSolutionFound = true;
               continue;
            }

            iteracja = a_jobType == JobType.TransportCosts
                              ? new IterationTransportCosts(nextStepGrid, JobType.TransportCosts, ++m_iterator) as IterationBase
                              : new IterationProfit(nextStepGrid, JobType.Profit, ++m_iterator) as IterationBase;

            iteracja.CalculateIterationResult();
            var zoptymalizowaneWartosc = iteracja.IterationResultValue;

            var express = a_jobType == JobType.TransportCosts
                                    ? aktulaneWartoscOptymalna > zoptymalizowaneWartosc
                                    : aktulaneWartoscOptymalna < zoptymalizowaneWartosc;

            if (express)
            {
               aktulaneWartoscOptymalna = zoptymalizowaneWartosc;
               Iterations.Add(iteracja);
               iteracja.CalculateGrid(Dostawcy, Odbiorcy);

               if (iteracja.Error.IsError)
               {
                  ErrorMessage = iteracja.Error.Message;
                  return;
               }

               if (attempt >= AttemptsLimit)
               {
                  ErrorMessage = "Nie udało się wyznaczyć optymalnego rozwiązania w iteracjach. Zweryfikuj wprowadzone dane";
                  return;
               }
               continue;
            }
            OptimalSolutionFound = true;
         }
      }


      private bool IsBalanced()
      {
         var popyt = Odbiorcy.Sum(o => o.Value);
         var podaz = Dostawcy.Sum(o => o.Value);
         return popyt == podaz;
      }


      private void CanByResolve()
      {
         if (!m_isInit)
            throw new Exception("Solver nie został zainicjowany");
      }

      #region [Data preparetion]

      public void AddKosztyJednostkowe(int y, int x, int a_value)
      {
         m_startIteration.DataGrid[y][x].KosztyJednostkowe = a_value;
      }

      public void AddDostawca(int a_value)
      {
         if (m_isInit)
            throw new Exception("Nrak możliwości dodania dostawcy. Solver został już zainicjowany");

         var supp = new InputData(Dostawcy.Count, InputType.Dostawca, a_value);
         Dostawcy.Add(supp);
      }

      public void AddOdbiorca(int a_value)
      {
         if (m_isInit)
            throw new Exception("Nrak możliwości dodania odbiorcy. Solver został już zainicjowany");


         var supp = new InputData(Odbiorcy.Count, InputType.Odbiorca, a_value);
         Odbiorcy.Add(supp);
      }

      #endregion [Data preparetion]

   }
}
