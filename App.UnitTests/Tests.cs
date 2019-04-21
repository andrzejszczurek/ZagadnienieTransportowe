using App.Core;
using App.Core.Model;
using App.Core.Solver;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace App.UnitTests
{
   [TestClass]
   public class Tests
   {
      [TestMethod]
      public void SolveJob_1()
      {
         var solver = new Solver(null);
         solver.AddDostawca(50);
         solver.AddDostawca(70);
         solver.AddDostawca(30);

         solver.AddOdbiorca(20);
         solver.AddOdbiorca(40);
         solver.AddOdbiorca(90);

         solver.Init();

         solver.AddKosztyJednostkowe(0, 0, 3);
         solver.AddKosztyJednostkowe(0, 1, 5);
         solver.AddKosztyJednostkowe(0, 2, 7);

         solver.AddKosztyJednostkowe(1, 0, 12);
         solver.AddKosztyJednostkowe(1, 1, 10);
         solver.AddKosztyJednostkowe(1, 2, 9);

         solver.AddKosztyJednostkowe(2, 0, 13);
         solver.AddKosztyJednostkowe(2, 1, 3);
         solver.AddKosztyJednostkowe(2, 2, 9);

         solver.Resolve();

         var iterations = solver.Iteracje;

         Assert.AreEqual(3, iterations.Count);
         Assert.AreEqual(1120, iterations[0].KosztyTransportu);
         Assert.AreEqual(1050, iterations[1].KosztyTransportu);
         Assert.AreEqual(970, iterations[2].KosztyTransportu);

      }

      [TestMethod]
      public void CalculatePrzydzial_Test()
      {
         var odbiorcy = new List<InputData>()
         {
            new InputData(0, InputType.Odbiorca, 20),
            new InputData(1, InputType.Odbiorca, 40),
            new InputData(2, InputType.Odbiorca, 90),
         };

         var dostawcy = new List<InputData>()
         {
            new InputData(0, InputType.Dostawca, 50),
            new InputData(1, InputType.Dostawca, 70),
            new InputData(2, InputType.Dostawca, 30),
         };

         var datagrid = Utility.CreateEmptyCellGrid(3, 3);
         var iteracja = new Iteration(datagrid, 1);
         iteracja.CalculatePrzydzial(dostawcy, odbiorcy);

         Assert.AreEqual(20, iteracja.DataGrid[0][0].Przydzial);
         Assert.AreEqual(30, iteracja.DataGrid[0][1].Przydzial);
         Assert.AreEqual(null, iteracja.DataGrid[0][2].Przydzial);

         Assert.AreEqual(null, iteracja.DataGrid[1][0].Przydzial);
         Assert.AreEqual(10, iteracja.DataGrid[1][1].Przydzial);
         Assert.AreEqual(60, iteracja.DataGrid[1][2].Przydzial);

         Assert.AreEqual(null, iteracja.DataGrid[2][0].Przydzial);
         Assert.AreEqual(null, iteracja.DataGrid[2][1].Przydzial);
         Assert.AreEqual(30, iteracja.DataGrid[2][2].Przydzial);
      }


   }
}
