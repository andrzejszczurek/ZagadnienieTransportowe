using App.Core.Model;
using System;
using System.Collections.Generic;

namespace App.Core.Solver
{
   public class Solver
   {
      private readonly UserData m_userData;

      public List<InputData> Dostawcy { get; set; }

      public List<InputData> Odbiorcy { get; set; }

      public List<Iteration> Iteracje { get; set; }

      private Iteration m_iteracjaStartowa;

      private bool m_isInit;

      public Solver(UserData a_userData)
      {
         m_userData = a_userData;
         Dostawcy = new List<InputData>();
         Odbiorcy = new List<InputData>();
         Iteracje = new List<Iteration>();
         m_isInit = false;
      }

      public void Init()
      {
         var grid = Utility.CreateEmptyGrid(Dostawcy.Count, Odbiorcy.Count);
         var iteracjaStartowa = new Iteration(grid);
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

            iteracja = new Iteration(nextStepGrid);
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
         var supp = new InputData(Dostawcy.Count + 1, InputType.Dostawca, a_value);
         Dostawcy.Add(supp);
      }

      public void AddOdbiorca(int a_value)
      {
         var supp = new InputData(Odbiorcy.Count + 1, InputType.Odbiorca, a_value);
         Odbiorcy.Add(supp);
      }

      #endregion [Data preparetion]

   }
}
