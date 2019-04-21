using System;

namespace App.Core.Model
{
   public class CyclePoint
   {
      public enum CyclePointType
      {
         CyklDodatni,
         CyklUjemny,
      }


      public Guid CycleId { get; set; }

      public int X { get; }

      public int Y { get; }

      public string Id { get; }

      public bool IsStart { get; }

      public int MyProperty { get; set; }

      public CyclePointType Type { get; set; }



      public CyclePoint(int a_y, int a_x)
         : this(a_y, a_x, false)
      {
      }


      public CyclePoint(int a_y, int a_x, bool a_isStart)
      {
         IsStart = a_isStart;
         Y = a_y;
         X = a_x;
         if (IsInGridScope())
            Id = Y.ToString() + X.ToString();
         if (IsStart)
            Type = CyclePointType.CyklDodatni;
      }


      /// <summary>
      /// Sprawdza czy punkt znajduję się w siatce.
      /// </summary>
      /// <returns></returns>
      public bool IsInGridScope()
         => X >= 0 && Y >= 0;


      /// <summary>
      /// Zwraca kopie punktu jako nowy obiekt.
      /// </summary>
      public static CyclePoint Copy(CyclePoint a_punkt)
      {
         return new CyclePoint(a_punkt.Y, a_punkt.X, a_punkt.IsStart);
      }
   }
}
