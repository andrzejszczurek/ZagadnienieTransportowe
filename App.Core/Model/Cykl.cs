using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.Model
{
   public class Cykl
   {
      public Guid CycleId { get;}

      public Punkt A { get; set; }

      public Punkt B { get; set; }

      public Punkt C { get; set; }

      public Punkt D { get; set; }

      public Cykl(Punkt a, Punkt b, Punkt c, Punkt d)
      {
         A = Punkt.Copy(a);
         B = Punkt.Copy(b);
         C = Punkt.Copy(c);
         D = Punkt.Copy(d);
         CycleId = Guid.NewGuid();
         A.CycleId = CycleId;
         B.CycleId = CycleId;
         C.CycleId = CycleId;
         D.CycleId = CycleId;

      }

      public bool IsPositive()
      {
         return A.IsPositive() && B.IsPositive()
            && C.IsPositive() && D.IsPositive();
      }

      public class Punkt
      {
         public Guid CycleId { get; set; }

         public int X { get; set; }

         public int Y { get; set; }

         public string Id { get; set; }

         public Punkt(int y, int x)
         {
            Y = y;
            X = x;
            if (IsPositive())
               Id = X.ToString() + Y.ToString();
         }

         public bool IsPositive()
         {
            return X >= 0 && Y >= 0;
         }

         public static Punkt Copy(Punkt a_punkt)
         {
            return new Punkt(a_punkt.Y, a_punkt.X);
         }
      }

   }
}
