using App.Core.Solver;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace App.UnitTests
{
   [TestClass]
   public class Tests
   {
      [TestMethod]
      public void SolveJob()
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

         var 

      }


      //[TestMethod]
      //public void GlobalTest()
      //{
      //   var solver = new Solver(null);
      //   solver.AddDostawca(50);
      //   solver.AddDostawca(70);
      //   solver.AddDostawca(30);

      //   solver.AddOdbiorca(20);
      //   solver.AddOdbiorca(40);
      //   solver.AddOdbiorca(90);

      //   grid.DataGrid = Solver.CreateGrid(3, 3);

      //   grid.AddKosztyJednostkowe(0, 0, 3);
      //   grid.AddKosztyJednostkowe(0, 1, 5);
      //   grid.AddKosztyJednostkowe(0, 2, 7);

      //   grid.AddKosztyJednostkowe(1, 0, 12);
      //   grid.AddKosztyJednostkowe(1, 1, 10);
      //   grid.AddKosztyJednostkowe(1, 2, 9);

      //   grid.AddKosztyJednostkowe(2, 0, 13);
      //   grid.AddKosztyJednostkowe(2, 1, 3);
      //   grid.AddKosztyJednostkowe(2, 2, 9);

      //   // tutaj uzytkownik klika przelicz

      //   // wyznaczenie zapotrzebowania
      //   grid.CalculatePrzydzial();


      //   Assert.AreEqual(grid.DataGrid[0][0].Przydział, 20);
      //   Assert.AreEqual(grid.DataGrid[0][1].Przydział, 30);
      //   Assert.AreEqual(grid.DataGrid[0][2].Przydział, null);

      //   Assert.AreEqual(grid.DataGrid[1][0].Przydział, null);
      //   Assert.AreEqual(grid.DataGrid[1][1].Przydział, 10);
      //   Assert.AreEqual(grid.DataGrid[1][2].Przydział, 60);

      //   Assert.AreEqual(grid.DataGrid[2][0].Przydział, null);
      //   Assert.AreEqual(grid.DataGrid[2][1].Przydział, null);
      //   Assert.AreEqual(grid.DataGrid[2][2].Przydział, 30);

      //   // wyznaczenie wspolczynnikow alfa i beta (zakładamy zbilansowany)

      //   grid.CalculateWspolczynnikiAlfaAndBeta();

      //   Assert.AreEqual(0, grid.Alfa[0]);
      //   Assert.AreEqual(5, grid.Alfa[1]);
      //   Assert.AreEqual(5, grid.Alfa[2]);

      //   Assert.AreEqual(3, grid.Beta[0]);
      //   Assert.AreEqual(5, grid.Beta[1]);
      //   Assert.AreEqual(4, grid.Beta[2]);

      //   // wyznaczanie wartości delt niebazowych
      //   grid.CalculateDeltyNiebazowe();

      //   Assert.AreEqual(null, grid.DataGrid[0][0].DeltaNiebazowa);
      //   Assert.AreEqual(null, grid.DataGrid[0][1].DeltaNiebazowa);
      //   Assert.AreEqual(3, grid.DataGrid[0][2].DeltaNiebazowa);

      //   Assert.AreEqual(4, grid.DataGrid[1][0].DeltaNiebazowa);
      //   Assert.AreEqual(null, grid.DataGrid[1][1].DeltaNiebazowa);
      //   Assert.AreEqual(null, grid.DataGrid[1][2].DeltaNiebazowa);

      //   Assert.AreEqual(5, grid.DataGrid[2][0].DeltaNiebazowa);
      //   Assert.AreEqual(-7, grid.DataGrid[2][1].DeltaNiebazowa);
      //   Assert.AreEqual(null, grid.DataGrid[2][2].DeltaNiebazowa);

      //   // Wyznaczamy cykl
      //   var expectedCyclePoints = new HashSet<string> {"11", "12", "21", "22" };

      //   var cykl = grid.FindCycle();

      //   Assert.IsTrue(expectedCyclePoints.Contains(cykl.A.Id));
      //   Assert.IsTrue(expectedCyclePoints.Contains(cykl.B.Id));
      //   Assert.IsTrue(expectedCyclePoints.Contains(cykl.C.Id));
      //   Assert.IsTrue(expectedCyclePoints.Contains(cykl.D.Id));

      //   // oznaczamy czykl negatywny
      //   var actualPrzydzial = grid.FindPrzydzialDoOptymalizacji(cykl);
      //   Assert.AreEqual(10, actualPrzydzial.Przydzał);

      //   // dokonujemy optymalizacji iteracji
      //   // TODO

      //   var nextStepGrid = grid.CalculateNextIteration(grid.DataGrid, cykl);

      //   Assert.AreEqual(nextStepGrid[0][0].Przydział, 20);
      //   Assert.AreEqual(nextStepGrid[0][1].Przydział, 30);
      //   Assert.AreEqual(nextStepGrid[0][2].Przydział, null);

      //   Assert.AreEqual(nextStepGrid[1][0].Przydział, null);
      //   Assert.AreEqual(nextStepGrid[1][1].Przydział, null);
      //   Assert.AreEqual(nextStepGrid[1][2].Przydział, 70);

      //   Assert.AreEqual(nextStepGrid[2][0].Przydział, null);
      //   Assert.AreEqual(nextStepGrid[2][1].Przydział, 10);
      //   Assert.AreEqual(nextStepGrid[2][2].Przydział, 20);

      //   // wyznaczamy koszty transportu

      //   var koszty = grid.CalculateKosztyTransportu();
      //   Assert.AreEqual(1120, koszty);

      //   // wyznaczamy kolejne iteracje, aż rozwiązanie nie będzie optymalne
      //   // TODO

      //}

      //[TestMethod]
      //public void TestGridIdGeneration()
      //{
      //   var supCount = 2;
      //   var custCount = 3;
      //   var grid = new IterationGrid();
      //   var result = grid.InitDataGrid();

      //   Assert.AreEqual(result[0][0].Id, "00");
      //   Assert.AreEqual(result[0][1].Id, "01");
      //   Assert.AreEqual(result[0][2].Id, "02");

      //   Assert.AreEqual(result[1][0].Id, "10");
      //   Assert.AreEqual(result[1][1].Id, "11");
      //   Assert.AreEqual(result[1][2].Id, "12");
      //}
   }
}
