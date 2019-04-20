using System;
using System.Collections.Generic;
using App.Core.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace App.UnitTests
{
   [TestClass]
   public class Tests
   {
      [TestMethod]
      public void GlobalTest()
      {
         var grid = new IterationGrid();
         grid.AddDostawca(50);
         grid.AddDostawca(70);
         grid.AddDostawca(30);
         grid.AddOdbiorca(20);
         grid.AddOdbiorca(40);
         grid.AddOdbiorca(90);

         grid.CreateGrid();

         grid.AddKosztyJednostkowe(0, 0, 3);
         grid.AddKosztyJednostkowe(0, 1, 5);
         grid.AddKosztyJednostkowe(0, 2, 7);

         grid.AddKosztyJednostkowe(1, 0, 12);
         grid.AddKosztyJednostkowe(1, 1, 10);
         grid.AddKosztyJednostkowe(1, 2, 9);

         grid.AddKosztyJednostkowe(2, 0, 13);
         grid.AddKosztyJednostkowe(2, 1, 3);
         grid.AddKosztyJednostkowe(2, 2, 9);

         // tutaj uzytkownik klika przelicz

         // wyznaczenie zapotrzebowania
         grid.CalculatePrzydzial();


         Assert.AreEqual(grid.DataGrid[0][0].Przydział, 20);
         Assert.AreEqual(grid.DataGrid[0][1].Przydział, 30);
         Assert.AreEqual(grid.DataGrid[0][2].Przydział, null);

         Assert.AreEqual(grid.DataGrid[1][0].Przydział, null);
         Assert.AreEqual(grid.DataGrid[1][1].Przydział, 10);
         Assert.AreEqual(grid.DataGrid[1][2].Przydział, 60);

         Assert.AreEqual(grid.DataGrid[2][0].Przydział, null);
         Assert.AreEqual(grid.DataGrid[2][1].Przydział, null);
         Assert.AreEqual(grid.DataGrid[2][2].Przydział, 30);

         // wyznaczenie wspolczynnikow alfa i beta (zakładamy zbilansowany)

         grid.CalculateWspolczynnikiAlfaAndBeta();

         Assert.AreEqual(0, grid.Alfa[0]);
         Assert.AreEqual(5, grid.Alfa[1]);
         Assert.AreEqual(5, grid.Alfa[2]);

         Assert.AreEqual(3, grid.Beta[0]);
         Assert.AreEqual(5, grid.Beta[1]);
         Assert.AreEqual(4, grid.Beta[2]);

         // wyznaczanie wartości delt niebazowych
         grid.CalculateDeltyNiebazowe();

         Assert.AreEqual(null, grid.DataGrid[0][0].DeltaNiebazowa);
         Assert.AreEqual(null, grid.DataGrid[0][1].DeltaNiebazowa);
         Assert.AreEqual(3, grid.DataGrid[0][2].DeltaNiebazowa);

         Assert.AreEqual(4, grid.DataGrid[1][0].DeltaNiebazowa);
         Assert.AreEqual(null, grid.DataGrid[1][1].DeltaNiebazowa);
         Assert.AreEqual(null, grid.DataGrid[1][2].DeltaNiebazowa);

         Assert.AreEqual(5, grid.DataGrid[2][0].DeltaNiebazowa);
         Assert.AreEqual(-7, grid.DataGrid[2][1].DeltaNiebazowa);
         Assert.AreEqual(null, grid.DataGrid[2][2].DeltaNiebazowa);

         // Wyznaczamy cykl
         var expectedCyclePoints = new HashSet<string> {"11", "12", "21", "22" };

         var cykl = grid.FindCycle();

         Assert.IsTrue(expectedCyclePoints.Contains(cykl.A.Id));
         Assert.IsTrue(expectedCyclePoints.Contains(cykl.B.Id));
         Assert.IsTrue(expectedCyclePoints.Contains(cykl.C.Id));
         Assert.IsTrue(expectedCyclePoints.Contains(cykl.D.Id));

         // dokonujemy optymalizacji iteracji
         // TODO

         // wyznaczamy kolejne iteracje, aż rozwiązanie nie będzie optymalne
         // TODO

      }

      [TestMethod]
      public void TestGridIdGeneration()
      {
         var supCount = 2;
         var custCount = 3;
         var grid = new IterationGrid();
         var result = grid.InitDataGrid();

         Assert.AreEqual(result[0][0].Id, "00");
         Assert.AreEqual(result[0][1].Id, "01");
         Assert.AreEqual(result[0][2].Id, "02");
                                     
         Assert.AreEqual(result[1][0].Id, "10");
         Assert.AreEqual(result[1][1].Id, "11");
         Assert.AreEqual(result[1][2].Id, "12");
      }
   }
}
