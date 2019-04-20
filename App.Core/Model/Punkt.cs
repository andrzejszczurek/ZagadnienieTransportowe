using System;

namespace App.Core.Model
{
   public class Punkt
   {
      public Guid CycleId { get; set; }

      public int X { get; set; }

      public int Y { get; set; }

      public string Id { get; set; }

      public bool IsStart { get; set; }

      public Punkt(int a_y, int a_x)
         : this(a_y, a_x, false)
      {
      }

      public Punkt(int a_y, int a_x, bool a_isStart)
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

      public static Punkt Copy(Punkt a_punkt)
      {
         return new Punkt(a_punkt.Y, a_punkt.X, a_punkt.IsStart);
      }
   }
}
