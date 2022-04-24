#region License Information
/*
 * This file is part of HEAL.SCS which is licensed under the MIT license.
 * See the LICENSE file in the project root for more information.
 */
#endregion

using SCS;
using System;

namespace Demo {
  class Program {
    static void Main(string[] args) {
      SolveExample(verbose: true);

      // // run repeatedly to check for memory leaks (observe allocated memory in task manager)
      // for (int i = 0; i < 100000; i++) {
      //   SolveExample();
      // }
    }

    private static void SolveExample(bool verbose = false) {
      var settings = new ScsSettings();
      SCSWrapper.SetDefaultSettings(settings);
      // example from https://www.cvxgrp.org/scs/examples/r.html
      // maximize x + y
      // subject to sqrt(x² + y²) <= sqrt(2)
      //            x,y >= 0
      var A = new double[,] {
          {-1.0,  0.0 }, // x >= 0
          { 0.0, -1.0 }, // y >= 0
          { 0.0,  0.0 },
          {-1.0,  0.0 },
          { 0.0, -1.0 }
        };
      var data = new ScsData();

      data.A = Util.DenseToSparse(A);
      data.b = new double[] { 0.0, 0.0, Math.Sqrt(2), 0.0, 0.0 };
      data.c = new double[] { -1, -1 };
      data.m = 5; data.n = 2;

      var cone = new ScsCone();
      cone.l = 2;
      cone.qsize = 1;
      cone.q = new int[] { 3 };
      settings.verbose = verbose ? 1 : 0;

      // SCSWrapper.ValidateCones(data, cone);
      if (0 <= SCSWrapper.Scs(data, cone, settings, out var solution, out var info)) {
        if (verbose) {
          Console.WriteLine(info);
          Console.WriteLine($"x = {string.Join(", ", solution.x)}");
          Console.WriteLine($"y = {string.Join(", ", solution.y)}");
          Console.WriteLine($"s = {string.Join(", ", solution.s)}");
        }
      }
    }
  }
}
