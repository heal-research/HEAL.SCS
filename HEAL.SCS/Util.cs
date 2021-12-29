#region License Information
/*
 * This file is part of HEAL.SCS which is licensed under the MIT license.
 * See the LICENSE file in the project root for more information.
 */
#endregion

using System.Collections.Generic;

namespace SCS {
  public static class Util {

    public static ScsMatrix DenseToSparse(double[,] A) {
      var row = new List<int>();
      var col = new List<int>();
      var x = new List<double>();
      for (int c = 0; c < A.GetLength(1); c++) {
        var newCol = true;
        for (int r = 0; r < A.GetLength(0); r++) {
          if (A[r, c] != 0.0) {
            x.Add(A[r, c]);
            row.Add(r);
            if (newCol) {
              col.Add(row.Count - 1);
              newCol = false;
            }
          }
        }
      }
      col.Add(row.Count); // end of matrix

      return new ScsMatrix() {
        m = A.GetLength(0), n = A.GetLength(1),
        i = row.ToArray(),
        p = col.ToArray(),
        x = x.ToArray()
      };
    }
  }
}
