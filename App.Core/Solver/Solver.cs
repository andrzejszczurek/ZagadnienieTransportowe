using App.Core.Model;
using System;
using System.Collections.Generic;

namespace App.Core.Solver
{
   public class Solver
   {
      private readonly UserData m_userData;

      public List<InputData> Dostawcy { get; }

      public List<InputData> Odbiorcy { get; }

      public List<Iteration> Iteracje { get; }

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
                           : Utility.CreateEmptyGrid(Dostawcy.Count, Odbiorcy.Count);

         var iteracjaStartowa = new Iteration(grid, ++m_iterator);
         m_iteracjaStartowa = iteracjaStartowa;
         Iteracje.Add(iteracjaStartowa);
         m_isInit = true;
      }

      public void Resolve()
      {
         CanByResolve();

         var iteracja = m_iteracjaStartowa;
         int? aktualneKoszty = null;
         var isOptimal = false;
         iteracja.CalculateGridInit(Dostawcy, Odbiorcy);
         aktualneKoszty = iteracja.KosztyTransportu;
         while (!isOptimal)
         {
            var nextStepGrid = iteracja.CalculateNextIteration();

            if (iteracja.IsOptimal)
            {
               isOptimal = true;
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
               continue;
            }
            isOptimal = true;
         }
      }

      private void CanByResolve()
      {
         if (!m_isInit)
            throw new Exception("Solver nie został zainicjowany");
      }

      public void AddKosztyJednostkowe(int y, int x, int a_value)
      {
         m_iteracjaStartowa.DataGrid[y][x].KosztyJednostkowe = a_value;
      }

      #region [Data preparetion]

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
