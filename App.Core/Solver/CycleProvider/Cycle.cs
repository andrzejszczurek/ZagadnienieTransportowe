using System;
using System.Collections.Generic;
using System.Linq;

namespace App.Core.Solver.CycleProvider
{
   public class Cycle
   {
      public Guid CycleId { get; }

      public CyclePoint A { get; }

      public CyclePoint B { get; }

      public CyclePoint C { get; }

      public CyclePoint D { get; }

      public CyclePoint Start { get; }


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


      /// <summary>
      /// Zwraca punkty cyklu jako lista.
      /// </summary>
      public List<CyclePoint> ToPointsList()
         => new List<CyclePoint>() { A, B, C, D };



      /// <summary>
      /// Sprawdza czy cykl znajduję się w siatce.
      /// </summary>
      public bool IsInGridScope()
         => A.IsInGridScope() && B.IsInGridScope()
            && C.IsInGridScope() && D.IsInGridScope();

   }
}
