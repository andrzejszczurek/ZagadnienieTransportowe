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

      public List<Iteration> Iteracje { get; }

      public string ErrorMessage { get; private set; }

      public int AttemptsLimit { get; set; }

      public bool OptimalSolutionFound { get; private set; }

      private Iteration m_iteracjaStartowa;

      private bool m_isInit;

      private int m_iterator;


      public Solver(UserData a_userData)
      {
         m_userData = a_userData;
         Dostawcy = a_userData is null ? new List<InputData>() : m_userData.Dostawcy;
         Odbiorcy = a_userData is null ? new List<InputData>() : m_userData.Odbiorcy;
         Iteracje = new List<Iteration>();
         m_isInit = false;
         m_iterator = 0;
         AttemptsLimit = 10;
      }


      /// <summary>
      /// Inicjalizuje początkową iterację dla zadanych dostawców i odbiorców.
      /// Jeżeli podczas tworzenia instancji solvera nie zostały przekazane dane wejściowe, 
      /// po wykonaniu Init należy dodać koszty.
      /// </summary>
      public void Init()
      {
         var isUserDataSet = !(m_userData is null);

         if (!isUserDataSet && (Dostawcy.Count < 1 || Odbiorcy.Count < 1))
            throw new Exception("Przed zainicjowaniem solvera należy dodać conajmniej jednego dostawce oraz odbiorcę.");

         GridCell[][] grid = isUserDataSet
                           ? m_userData.SiatkaKosztowJednostkowych
                           : Utility.CreateEmptyCellGrid(Dostawcy.Count, Odbiorcy.Count);

         var iteracjaStartowa = new Iteration(grid, ++m_iterator);
         m_iteracjaStartowa = iteracjaStartowa;
         Iteracje.Add(iteracjaStartowa);
         m_isInit = true;
      }


      /// <summary>
      /// Rozwiązuje zadanie w iteraciach.
      /// </summary>
      public void Resolve()
      {
         CanByResolve();
         if (!IsBalanced())
         {
            OptimalSolutionFound = false;
            ErrorMessage = "Zadanie nie jest zbilansowane. Brak wsparcia dla tego typu danych.";
            return;
         }

         var attempt = 0;
         var iteracja = m_iteracjaStartowa;
         int? aktualneKoszty = null;
         OptimalSolutionFound = false;
         iteracja.CalculateGridInit(Dostawcy, Odbiorcy);
         aktualneKoszty = iteracja.KosztyTransportu;
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

            iteracja = new Iteration(nextStepGrid, ++m_iterator);
            iteracja.CalculateKosztyTransportu();
            var zoptymalizowaneKoszty = iteracja.KosztyTransportu;

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
         var supp = new InputData(Dostawcy.Count, InputType.Dostawca, a_value);
         Dostawcy.Add(supp);
      }

      public void AddOdbiorca(int a_value)
      {
         var supp = new InputData(Odbiorcy.Count, InputType.Odbiorca, a_value);
         Odbiorcy.Add(supp);
      }

      #endregion [Data preparetion]

   }
}
