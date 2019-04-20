using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core
{
   public static class Extenssions
   {
      public static T[][] Init2DimmArray<T>(int x, int y)
      {
         var arr = new T[y][];
         for (int i = 0; i < arr.Length; i++)
         {
            arr[i] = new T[x];
         }

         return arr;
      }
   }
}
