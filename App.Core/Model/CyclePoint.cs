using System;

namespace App.Core.Model
{
   public class CyclePoint
   {
      public Guid CycleId { get; set; }

      public int X { get; set; }

      public int Y { get; set; }

      public string Id { get; set; }

      public bool IsStart { get; set; }

      public CyclePoint(int a_y, int a_x)
         : this(a_y, a_x, false)
      {
      }

      public CyclePoint(int a_y, int a_x, bool a_isStart)
      {
         IsStart = a_isStart;
         Y = a_y;
         X = a_x;
         if (IsPositive())
            Id = Y.ToString() + X.ToString();
      }

      public bool IsPositive()
      {
         return X >= 0 && Y >= 0;
      }

      public static CyclePoint Copy(CyclePoint a_punkt)
      {
         return new CyclePoint(a_punkt.Y, a_punkt.X, a_punkt.IsStart);
      }
   }
}
