using App.Core.Model;

namespace App.Core
{
   public static class Utility
   {

      public static GridCell[][] CreateEmptyCellGrid(int a_rows, int a_columns)
      {
         var grid = Init2DimmArray<GridCell>(a_columns, a_rows);
         return InitCellGrid(grid);
      }


      public static T[][] Init2DimmArray<T>(int x, int y)
      {
         var arr = new T[y][];
         for (int i = 0; i < arr.Length; i++)
            arr[i] = new T[x];
         return arr;
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
