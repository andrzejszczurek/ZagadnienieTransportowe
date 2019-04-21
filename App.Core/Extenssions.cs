using App.Core.Model;

namespace App.Core
{
   public static class Utility
   {
      public static T[][] Init2DimmArray<T>(int x, int y)
      {
         var arr = new T[y][];
         for (int i = 0; i < arr.Length; i++)
            arr[i] = new T[x];
         return arr;
      }


      public static GridCell[][] CreateEmptyCellGrid(int a_dostawcyCount, int a_odbiorcyCount)
      {
         var grid = Init2DimmArray<GridCell>(a_odbiorcyCount, a_dostawcyCount);
         return InitCellGrid(grid);
      }


      private static GridCell[][] InitCellGrid(GridCell[][] a_grid)
      {
         for (int y = 0; y < a_grid.Length; y++)
            for (int x = 0; x < a_grid[y].Length; x++)
               a_grid[y][x] = new GridCell(x, y);
         return a_grid;
      }

   }
}
