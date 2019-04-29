using App.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace App.Core.Solver
{
   public class Solver
   {
      private readonly UserData m_userData;

      public List<InputData> Dostawcy { get; }

      public List<InputData> Odbiorcy { get; }

      public List<IterationKosztyTransportu> Iteracje { get; }

      public string ErrorMessage { get; private set; }

      public int AttemptsLimit { get; set; }

      public bool OptimalSolutionFound { get; private set; }

      private IterationKosztyTransportu m_iteracjaStartowa;

      private bool m_isInit;

      private int m_iterator;


      public Solver(UserData a_userData)
      {
         m_userData = a_userData;
         Dostawcy = a_userData is null ? new List<InputData>() : m_userData.Dostawcy; // todo: to powinny być kopia pełna 
         Odbiorcy = a_userData is null ? new List<InputData>() : m_userData.Odbiorcy; // todo: to powinny być kopia pełna
         Iteracje = new List<IterationKosztyTransportu>();
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
      public void Init()
      {
         var isUserDataSet = !(m_userData is null);

         if (!isUserDataSet && (Dostawcy.Count < 1 || Odbiorcy.Count < 1))
            throw new Exception("Przed zainicjowaniem solvera należy dodać conajmniej jednego dostawce oraz odbiorcę.");


         var isBalaced = IsBalanced();
         if (!isBalaced)
         {
            AddVirtualInputData();
            //OptimalSolutionFound = false;
            //ErrorMessage = "Zadanie nie jest zbilansowane. Brak wsparcia dla tego typu danych.";
            //return;
         }
         GridCell[][] grid = isUserDataSet
                              ? m_userData.SiatkaKosztowJednostkowych
                              : Utility.CreateEmptyCellGrid(Dostawcy.Count, Odbiorcy.Count);

         if (!isBalaced && isUserDataSet)
         {
            RecalculateDataGrid();
            grid = m_userData.SiatkaKosztowJednostkowych;
         }

         var iteracjaStartowa = new IterationKosztyTransportu(grid, ++m_iterator);
         m_iteracjaStartowa = iteracjaStartowa;
         Iteracje.Add(iteracjaStartowa);
         m_isInit = true;
      }

      private void RecalculateDataGrid()
      {
         var virtualGrid = Utility.CreateEmptyCellGrid(Dostawcy.Count, Odbiorcy.Count);
         for (int y = 0; y < m_userData.SiatkaKosztowJednostkowych.Length; y++)
         {
            for (int x = 0; x < m_userData.SiatkaKosztowJednostkowych[y].Length; x++)
            {
               var cell = m_userData.SiatkaKosztowJednostkowych[y][x];
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

         m_userData.SiatkaKosztowJednostkowych = virtualGrid;
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
      public void Resolve()
      {
         CanByResolve();

         var attempt = 0;
         var iteracja = m_iteracjaStartowa;
         int? aktualneKoszty = null;
         OptimalSolutionFound = false;
         iteracja.CalculateGridInit(Dostawcy, Odbiorcy);
         aktualneKoszty = iteracja.IterationResultValue;
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

            iteracja = new IterationKosztyTransportu(nextStepGrid, ++m_iterator);
            iteracja.CalculateIterationResult();
            var zoptymalizowaneKoszty = iteracja.IterationResultValue;

            if (aktualneKoszty > zoptymalizowaneKoszty)
            {
               aktualneKoszty = zoptymalizowaneKoszty;
               Iteracje.Add(iteracja);
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
         m_iteracjaStartowa.DataGrid[y][x].KosztyJednostkowe = a_value;
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
