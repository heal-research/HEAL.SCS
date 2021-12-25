#region License Information
/*
 * This file is part of HEAL.SCS which is licensed under the MIT license.
 * See the LICENSE file in the project root for more information.
 */
#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SCS;
using System;

namespace Test {
  [TestClass]
  public class Test {
    [TestMethod]
    public void TestVersion() {
      string version = SCSWrapper.Version();
      Assert.AreEqual(version, "3.0.0");
    }

    [TestMethod]
    public void TestSetDefaultSettings() {
      var settings = new ScsSettings();
      // settings.write_data_filename = string.Empty;
      // settings.log_csv_filename = string.Empty;

      SCSWrapper.SetDefaultSettings(settings);
      Assert.IsTrue(settings.eps_abs > 0.0);
    }

    [TestMethod]
    public void TestSolve() {
      var settings = new ScsSettings();
      SCSWrapper.SetDefaultSettings(settings);


      {
        // example from https://www.cvxgrp.org/scs/examples/r.html
        // maximize x + y
        // subject to sqrt(x² + y²) <= sqrt(2)
        //            x,y >= 0
        var obj = new double[] { -1, -1 };
        var A = new double[,] {
          {-1.0,  0.0 }, // x >= 0
          { 0.0, -1.0 }, // y >= 0
          { 0.0,  0.0 },
          {-1.0,  0.0 },
          { 0.0, -1.0 }
        };
        var b = new double[] { 0.0, 0.0, Math.Sqrt(2), 0.0, 0.0 };
        var data = new ScsData {
          A = Util.DenseToSparse(A),
          b = b,
          c = obj,
          m = 5, n = 2,
          P = null
        };
        var cone = new ScsCone {
          z = 0,
          l = 2,
          qsize = 1,
          q = new int[] { 3 }
        };
        // settings.log_csv_filename = @"C:\temp\scs.log";
        // settings.write_data_filename = @"c:\temp\scs_data.log";
        settings.verbose = 0;

        Assert.AreEqual(0, SCSWrapper.ValidateCones(data, cone));
        Assert.IsTrue(SCSWrapper.Scs(data, cone, settings, out var solution, out var info) >= 0);
        Console.WriteLine(info);
        Console.WriteLine($"x = {string.Join(", ", solution.x)}");
        Console.WriteLine($"y = {string.Join(", ", solution.y)}");
        Console.WriteLine($"s = {string.Join(", ", solution.s)}");
      }
    }



    [TestMethod]
    public void TestWriteData() {
      var settings = new ScsSettings();
      SCSWrapper.SetDefaultSettings(settings);


      {
        // SCS Matrix must be given in Compressed Column Sparse Format 
        // https://people.sc.fsu.edu/~jburkardt/data/cc/cc.html

        // maximize x + y
        // subject to x,y >= 0

        var A = new double[,] {
          {-1.0,  0.0 }, // x >= 0
          { 0.0, -1.0 }, // y >= 0
        };


        var data = new ScsData {
          A = Util.DenseToSparse(A),
          b = new double[] { 0.0, 0.0 },
          c = new double[] { -1, -1 },
          m = 2, n = 2
        };

        var cone = new ScsCone {
          l = 2,
          qsize = 0
        };

        settings.log_csv_filename = @"C:\temp\scs.log";
        settings.write_data_filename = @"c:\temp\scs_data.log";
        settings.verbose = 1;
        Assert.AreEqual(0, SCSWrapper.ValidateCones(data, cone));
        SCSWrapper.WriteData(data, cone, settings);
      }
    }


    [TestMethod]
    public void TestValidateCones() {
      {
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
        var A_sparse = Util.DenseToSparse(A);
        var data = new ScsData {
          A = A_sparse,
          b = new double[] { 0.0, 0.0, Math.Sqrt(2), 0.0, 0.0 },
          c = new double[] { -1, -1 },
          m = 5,
          n = 2
        };

        var cone = new ScsCone {
          l = 2,
          qsize = 1,
          q = new int[] { 3 }
        };

        Assert.AreEqual(0, SCSWrapper.ValidateCones(data, cone));
      }
    }

    [TestMethod]
    public void TestScsMatrix() {
      // from https://people.sc.fsu.edu/~jburkardt/data/cc/cc.html
      var A = new double[,] {
        {  11.0,   0.0,   0.0,  14.0,   0.0,  16.0 },
        {   0.0,  22.0,   0.0,   0.0,  25.0,  26.0 },
        {   0.0,   0.0,  33.0,  34.0,   0.0,  36.0 },
        {  41.0,   0.0,  43.0,  44.0,   0.0,  46.0 }
      };

      var sparse = Util.DenseToSparse(A);
      CollectionAssert.AreEqual(new int[] { 0, 2, 3, 5, 8, 9, 13 }, sparse.p);
      CollectionAssert.AreEqual(new int[] { 0, 3, 1, 2, 3, 0, 2, 3, 1, 0, 1, 2, 3 }, sparse.i);
      CollectionAssert.AreEqual(new double[] { 11.0, 41.0, 22.0, 33.0, 43.0, 14.0, 34.0, 44.0, 25.0, 16.0, 26.0, 36.0, 46.0 }, sparse.x);
    }

  }
}
