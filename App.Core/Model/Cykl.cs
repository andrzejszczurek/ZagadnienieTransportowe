using System;
using System.Collections.Generic;
using System.Linq;

namespace App.Core.Model
{
   public class Cykl
   {
      public Guid CycleId { get;}

      public Punkt A { get; set; }

      public Punkt B { get; set; }

      public Punkt C { get; set; }

      public Punkt D { get; set; }

      public Punkt Start { get; set; }

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
         Start = ToPointsList().Single(p => p.IsStart);
      }

      public List<Punkt> ToPointsList()
      {
         return new List<Punkt>() { A, B, C, D };
      }

      public bool IsPositive()
      {
         return A.IsPositive() && B.IsPositive()
            && C.IsPositive() && D.IsPositive();
      }

   }
}
