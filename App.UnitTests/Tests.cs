using App.Core;
using App.Core.Model;
using App.Core.Solver;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

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
         Assert.AreEqual(1120, iterations[0].IterationResultValue);
         Assert.AreEqual(1050, iterations[1].IterationResultValue);
         Assert.AreEqual(970, iterations[2].IterationResultValue);

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
         var iteracja = new IterationKosztyTransportu(datagrid, 1);
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


      [TestMethod]
      public void Calculate_zysk_test()
      {
         var odbiorcy = new List<InputData>()
         {
            new InputData(0, InputType.Odbiorca, 10, 30),
            new InputData(1, InputType.Odbiorca, 28, 25),
            new InputData(2, InputType.Odbiorca, 27, 30),
         };

         var dostawcy = new List<InputData>()
         {
            new InputData(0, InputType.Dostawca, 20, 10),
            new InputData(1, InputType.Dostawca, 30, 12),
         };

         var datagrid = Utility.CreateEmptyCellGrid(dostawcy.Count, odbiorcy.Count);
         datagrid[0][0].KosztyJednostkowe = 8;
         datagrid[0][1].KosztyJednostkowe = 14;
         datagrid[0][2].KosztyJednostkowe = 17;

         datagrid[1][0].KosztyJednostkowe = 12;
         datagrid[1][1].KosztyJednostkowe = 9;
         datagrid[1][2].KosztyJednostkowe = 19;

         var iteracja = new IterationZysk(datagrid, 1);
         iteracja.CalculateZysk(dostawcy, odbiorcy);

         Assert.AreEqual(12, iteracja.DataGrid[0][0].Zysk);
         Assert.AreEqual(1, iteracja.DataGrid[0][1].Zysk);
         Assert.AreEqual(3, iteracja.DataGrid[0][2].Zysk);

         Assert.AreEqual(6, iteracja.DataGrid[1][0].Zysk);
         Assert.AreEqual(4, iteracja.DataGrid[1][1].Zysk);
         Assert.AreEqual(-1, iteracja.DataGrid[1][2].Zysk);
      }


      [TestMethod]
      public void Calculate_przycial_max_element_macierzy()
      {
         var odbiorcy = new List<InputData>()
         {
            new InputData(0, InputType.Odbiorca, 10, 30),
            new InputData(1, InputType.Odbiorca, 28, 25),
            new InputData(2, InputType.Odbiorca, 27, 30),
            new InputData(3, InputType.Odbiorca, 50, null, true)
         };

         var dostawcy = new List<InputData>()
         {
            new InputData(0, InputType.Dostawca, 20, 10),
            new InputData(1, InputType.Dostawca, 30, 12),
            new InputData(2, InputType.Odbiorca, 65, null, true)
         };

         var datagrid = Utility.CreateEmptyCellGrid(dostawcy.Count, odbiorcy.Count);
         datagrid[0][0].KosztyJednostkowe = 8;
         datagrid[0][1].KosztyJednostkowe = 14;
         datagrid[0][2].KosztyJednostkowe = 17;

         datagrid[1][0].KosztyJednostkowe = 12;
         datagrid[1][1].KosztyJednostkowe = 9;
         datagrid[1][2].KosztyJednostkowe = 19;


         datagrid[0][3].IsVirtual = true;
         datagrid[1][3].IsVirtual = true;
         datagrid[2][3].IsVirtual = true;

         datagrid[2][0].IsVirtual = true;
         datagrid[2][1].IsVirtual = true;
         datagrid[2][2].IsVirtual = true;


         var iteracja = new IterationZysk(datagrid, 1);
         iteracja.CalculateZysk(dostawcy, odbiorcy);
         iteracja.CalculatePrzydzial(dostawcy, odbiorcy);

         Assert.AreEqual(10, iteracja.DataGrid[0][0].Przydzial);
         Assert.AreEqual(null, iteracja.DataGrid[0][1].Przydzial);
         Assert.AreEqual(10, iteracja.DataGrid[0][2].Przydzial);

         Assert.AreEqual(null, iteracja.DataGrid[1][0].Przydzial);
         Assert.AreEqual(28, iteracja.DataGrid[1][1].Przydzial);
         Assert.AreEqual(2, iteracja.DataGrid[1][2].Przydzial);

         Assert.AreEqual(null, iteracja.DataGrid[0][3].Przydzial);
         Assert.AreEqual(null, iteracja.DataGrid[1][3].Przydzial);


         Assert.AreEqual(null, iteracja.DataGrid[2][0].Przydzial);
         Assert.AreEqual(null, iteracja.DataGrid[2][1].Przydzial);
         Assert.AreEqual(15, iteracja.DataGrid[2][2].Przydzial);
         Assert.AreEqual(50, iteracja.DataGrid[2][3].Przydzial);
      }

      [TestMethod]
      public void Calculate_wspolczynniki_alfa_beta()
      {
         var odbiorcy = new List<InputData>()
         {
            new InputData(0, InputType.Odbiorca, 10, 30),
            new InputData(1, InputType.Odbiorca, 28, 25),
            new InputData(2, InputType.Odbiorca, 27, 30),
            new InputData(3, InputType.Odbiorca, 50, null, true)
         };

         var dostawcy = new List<InputData>()
         {
            new InputData(0, InputType.Dostawca, 20, 10),
            new InputData(1, InputType.Dostawca, 30, 12),
            new InputData(2, InputType.Odbiorca, 65, null, true)
         };

         var datagrid = Utility.CreateEmptyCellGrid(dostawcy.Count, odbiorcy.Count);
         datagrid[0][0].KosztyJednostkowe = 8;
         datagrid[0][1].KosztyJednostkowe = 14;
         datagrid[0][2].KosztyJednostkowe = 17;

         datagrid[1][0].KosztyJednostkowe = 12;
         datagrid[1][1].KosztyJednostkowe = 9;
         datagrid[1][2].KosztyJednostkowe = 19;

         datagrid[0][3].IsVirtual = true;
         datagrid[1][3].IsVirtual = true;
         datagrid[2][3].IsVirtual = true;

         datagrid[2][0].IsVirtual = true;
         datagrid[2][1].IsVirtual = true;
         datagrid[2][2].IsVirtual = true;


         var iteracja = new IterationZysk(datagrid, 1);
         iteracja.CalculateZysk(dostawcy, odbiorcy);
         iteracja.CalculatePrzydzial(dostawcy, odbiorcy);
         iteracja.CalculateWspolczynnikiAlfaAndBeta();

         Assert.AreEqual(0, iteracja.Alfa[0].Value);
         Assert.AreEqual(-4, iteracja.Alfa[1].Value);
         Assert.AreEqual(-3, iteracja.Alfa[2].Value);

         Assert.AreEqual(12, iteracja.Beta[0].Value);
         Assert.AreEqual(8, iteracja.Beta[1].Value);
         Assert.AreEqual(3, iteracja.Beta[2].Value);
         Assert.AreEqual(3, iteracja.Beta[3].Value);

      }

      [TestMethod]
      public void Calculate_iteration_value() // zysk
      {
         var odbiorcy = new List<InputData>()
         {
            new InputData(0, InputType.Odbiorca, 10, 30),
            new InputData(1, InputType.Odbiorca, 28, 25),
            new InputData(2, InputType.Odbiorca, 27, 30),
            new InputData(3, InputType.Odbiorca, 50, null, true)
         };

         var dostawcy = new List<InputData>()
         {
            new InputData(0, InputType.Dostawca, 20, 10),
            new InputData(1, InputType.Dostawca, 30, 12),
            new InputData(2, InputType.Odbiorca, 65, null, true)
         };

         var datagrid = Utility.CreateEmptyCellGrid(dostawcy.Count, odbiorcy.Count);
         datagrid[0][0].KosztyJednostkowe = 8;
         datagrid[0][1].KosztyJednostkowe = 14;
         datagrid[0][2].KosztyJednostkowe = 17;

         datagrid[1][0].KosztyJednostkowe = 12;
         datagrid[1][1].KosztyJednostkowe = 9;
         datagrid[1][2].KosztyJednostkowe = 19;

         datagrid[0][3].IsVirtual = true;
         datagrid[1][3].IsVirtual = true;
         datagrid[2][3].IsVirtual = true;

         datagrid[2][0].IsVirtual = true;
         datagrid[2][1].IsVirtual = true;
         datagrid[2][2].IsVirtual = true;


         var iteracja = new IterationZysk(datagrid, 1);
         iteracja.CalculateZysk(dostawcy, odbiorcy);
         iteracja.CalculatePrzydzial(dostawcy, odbiorcy);
         iteracja.CalculateIterationResult();

         Assert.AreEqual(260, iteracja.IterationResultValue);
      }

      [TestMethod]
      public void Calculate_delty_niebazowe()
      {
         var odbiorcy = new List<InputData>()
         {
            new InputData(0, InputType.Odbiorca, 10, 30),
            new InputData(1, InputType.Odbiorca, 28, 25),
            new InputData(2, InputType.Odbiorca, 27, 30),
            new InputData(3, InputType.Odbiorca, 50, null, true)
         };

         var dostawcy = new List<InputData>()
         {
            new InputData(0, InputType.Dostawca, 20, 10),
            new InputData(1, InputType.Dostawca, 30, 12),
            new InputData(2, InputType.Odbiorca, 65, null, true)
         };

         var datagrid = Utility.CreateEmptyCellGrid(dostawcy.Count, odbiorcy.Count);
         datagrid[0][0].KosztyJednostkowe = 8;
         datagrid[0][1].KosztyJednostkowe = 14;
         datagrid[0][2].KosztyJednostkowe = 17;

         datagrid[1][0].KosztyJednostkowe = 12;
         datagrid[1][1].KosztyJednostkowe = 9;
         datagrid[1][2].KosztyJednostkowe = 19;

         datagrid[0][3].IsVirtual = true;
         datagrid[1][3].IsVirtual = true;
         datagrid[2][3].IsVirtual = true;

         datagrid[2][0].IsVirtual = true;
         datagrid[2][1].IsVirtual = true;
         datagrid[2][2].IsVirtual = true;

         var iteracja = new IterationZysk(datagrid, 1);
         iteracja.CalculateZysk(dostawcy, odbiorcy);
         iteracja.CalculatePrzydzial(dostawcy, odbiorcy);
         iteracja.CalculateWspolczynnikiAlfaAndBeta();
         iteracja.CalculateDeltyNiebazowe();

         Assert.AreEqual(null, iteracja.DataGrid[0][0].DeltaNiebazowa);
         Assert.AreEqual(-7, iteracja.DataGrid[0][1].DeltaNiebazowa);
         Assert.AreEqual(null, iteracja.DataGrid[0][2].DeltaNiebazowa);
         Assert.AreEqual(-3, iteracja.DataGrid[0][3].DeltaNiebazowa);

         Assert.AreEqual(-2, iteracja.DataGrid[1][0].DeltaNiebazowa);
         Assert.AreEqual(null, iteracja.DataGrid[1][1].DeltaNiebazowa);
         Assert.AreEqual(null, iteracja.DataGrid[1][2].DeltaNiebazowa);
         Assert.AreEqual(1, iteracja.DataGrid[1][3].DeltaNiebazowa);

         Assert.AreEqual(-9, iteracja.DataGrid[2][0].DeltaNiebazowa);
         Assert.AreEqual(-5, iteracja.DataGrid[2][1].DeltaNiebazowa);
         Assert.AreEqual(null, iteracja.DataGrid[2][2].DeltaNiebazowa);
         Assert.AreEqual(null, iteracja.DataGrid[2][3].DeltaNiebazowa);
      }



      [TestMethod]
      public void Calculate_cykl()
      {
         var odbiorcy = new List<InputData>()
         {
            new InputData(0, InputType.Odbiorca, 10, 30),
            new InputData(1, InputType.Odbiorca, 28, 25),
            new InputData(2, InputType.Odbiorca, 27, 30),
            new InputData(3, InputType.Odbiorca, 50, null, true)
         };

         var dostawcy = new List<InputData>()
         {
            new InputData(0, InputType.Dostawca, 20, 10),
            new InputData(1, InputType.Dostawca, 30, 12),
            new InputData(2, InputType.Odbiorca, 65, null, true)
         };

         var datagrid = Utility.CreateEmptyCellGrid(dostawcy.Count, odbiorcy.Count);
         datagrid[0][0].KosztyJednostkowe = 8;
         datagrid[0][1].KosztyJednostkowe = 14;
         datagrid[0][2].KosztyJednostkowe = 17;

         datagrid[1][0].KosztyJednostkowe = 12;
         datagrid[1][1].KosztyJednostkowe = 9;
         datagrid[1][2].KosztyJednostkowe = 19;

         datagrid[0][3].IsVirtual = true;
         datagrid[1][3].IsVirtual = true;
         datagrid[2][3].IsVirtual = true;

         datagrid[2][0].IsVirtual = true;
         datagrid[2][1].IsVirtual = true;
         datagrid[2][2].IsVirtual = true;

         var iteracja = new IterationZysk(datagrid, 1);
         iteracja.CalculateZysk(dostawcy, odbiorcy);
         iteracja.CalculatePrzydzial(dostawcy, odbiorcy);
         iteracja.CalculateWspolczynnikiAlfaAndBeta();
         iteracja.CalculateDeltyNiebazowe();

         var cycleDetector = new CycleDetector(iteracja.DataGrid, CycleDetector.CycleType.Positive).Detect();
         var points = cycleDetector.WyznaczonyCykl.ToPointsList();
         var expectedPoints = new string[] { "13", "12", "22", "23" }.ToList();

         Assert.IsTrue(expectedPoints.Contains(points[0].Id));
         Assert.IsTrue(expectedPoints.Contains(points[1].Id));
         Assert.IsTrue(expectedPoints.Contains(points[2].Id));
         Assert.IsTrue(expectedPoints.Contains(points[3].Id));

         Assert.AreEqual(2, cycleDetector.FindPrzydzialDoOptymalizacji());
      }


      [TestMethod]
      public void Calculate_Next_interation_grid()
      {
         var odbiorcy = new List<InputData>()
         {
            new InputData(0, InputType.Odbiorca, 10, 30),
            new InputData(1, InputType.Odbiorca, 28, 25),
            new InputData(2, InputType.Odbiorca, 27, 30),
            new InputData(3, InputType.Odbiorca, 50, null, true)
         };

         var dostawcy = new List<InputData>()
         {
            new InputData(0, InputType.Dostawca, 20, 10),
            new InputData(1, InputType.Dostawca, 30, 12),
            new InputData(2, InputType.Odbiorca, 65, null, true)
         };

         var datagrid = Utility.CreateEmptyCellGrid(dostawcy.Count, odbiorcy.Count);
         datagrid[0][0].KosztyJednostkowe = 8;
         datagrid[0][1].KosztyJednostkowe = 14;
         datagrid[0][2].KosztyJednostkowe = 17;

         datagrid[1][0].KosztyJednostkowe = 12;
         datagrid[1][1].KosztyJednostkowe = 9;
         datagrid[1][2].KosztyJednostkowe = 19;

         datagrid[0][3].IsVirtual = true;
         datagrid[1][3].IsVirtual = true;
         datagrid[2][3].IsVirtual = true;

         datagrid[2][0].IsVirtual = true;
         datagrid[2][1].IsVirtual = true;
         datagrid[2][2].IsVirtual = true;

         var iteracja = new IterationZysk(datagrid, 1);
         iteracja.CalculateZysk(dostawcy, odbiorcy);
         iteracja.CalculatePrzydzial(dostawcy, odbiorcy);
         iteracja.CalculateWspolczynnikiAlfaAndBeta();
         iteracja.CalculateDeltyNiebazowe();
         var newGrid = iteracja.CalculateNextIteration();

         Assert.AreEqual(10, newGrid[0][0].Przydzial);
         Assert.AreEqual(null, newGrid[0][1].Przydzial);
         Assert.AreEqual(10, newGrid[0][2].Przydzial);
         Assert.AreEqual(null, newGrid[0][3].Przydzial);

         Assert.AreEqual(null, newGrid[1][0].Przydzial);
         Assert.AreEqual(28, newGrid[1][1].Przydzial);
         Assert.AreEqual(null, newGrid[1][2].Przydzial);
         Assert.AreEqual(2, newGrid[1][3].Przydzial);

         Assert.AreEqual(null, newGrid[2][0].Przydzial);
         Assert.AreEqual(null, newGrid[2][1].Przydzial);
         Assert.AreEqual(17, newGrid[2][2].Przydzial);
         Assert.AreEqual(48, newGrid[2][3].Przydzial);
      }

   }
}
