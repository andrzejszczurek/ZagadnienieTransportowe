using System;
using System.Collections.Generic;
using System.Linq;

namespace App.Core.Model
{
   public class Cycle
   {
      public Guid CycleId { get;}

      public CyclePoint A { get; set; }

      public CyclePoint B { get; set; }

      public CyclePoint C { get; set; }

      public CyclePoint D { get; set; }

      public CyclePoint Start { get; set; }

      public Cycle(CyclePoint a, CyclePoint b, CyclePoint c, CyclePoint d)
      {
         A = CyclePoint.Copy(a);
         B = CyclePoint.Copy(b);
         C = CyclePoint.Copy(c);
         D = CyclePoint.Copy(d);
         CycleId = Guid.NewGuid();
         A.CycleId = CycleId;
         B.CycleId = CycleId;
         C.CycleId = CycleId;
         D.CycleId = CycleId;
         Start = ToPointsList().Single(p => p.IsStart);
      }

      public List<CyclePoint> ToPointsList()
      {
         return new List<CyclePoint>() { A, B, C, D };
      }

      public bool IsPositive()
      {
         return A.IsPositive() && B.IsPositive()
            && C.IsPositive() && D.IsPositive();
      }

   }
}
