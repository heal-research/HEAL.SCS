using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
namespace SCS {


  [StructLayout(LayoutKind.Sequential)]
  ///<summary>
  ///This defines the data matrices which should be supplied in column compressed
  ///format with zero based indexing.
  ///https://people.sc.fsu.edu/~jburkardt/data/cc/cc.html
  ///</summary>
  public class ScsMatrix {
    ///<value>Matrix values, size: number of non-zeros.</value>
    public double[] x;
    ///<value>Matrix row indices, size: number of non-zeros.</value>
    public int[] i;
    ///<value>Matrix column pointers, size: `n+1`.</value>
    public int[] p;
    ///<value>Number of rows.</value>
    public int m;
    ///<value>Number of columns.</value>
    public int n;
  };

  [StructLayout(LayoutKind.Sequential)]
  ///<summary>Struct containing all settings.</summary>
  public class ScsSettings {
    /** Whether to heuristically rescale the data before solve. */
    public int normalize;
    /** Initial dual scaling factor (may be updated if adaptive_scale is on). */
    public double scale;
    /** Whether to adaptively update `scale`. */
    public int adaptive_scale;
    /** Primal constraint scaling factor. */
    public double rho_x;
    /** Maximum iterations to take. */
    public int max_iters;
    /** Absolute convergence tolerance. */
    public double eps_abs;
    /** Relative convergence tolerance. */
    public double eps_rel;
    /** Infeasible convergence tolerance. */
    public double eps_infeas;
    /** Douglas-Rachford relaxation parameter. */
    public double alpha;
    /** Time limit in secs (can be fractional). */
    public double time_limit_secs;
    /** Whether to log progress to stdout. */
    public int verbose;
    /** Whether to use warm start (put initial guess in ScsSolution struct). */
    public int warm_start;
    /** Memory for acceleration. */
    public int acceleration_lookback;
    /** Interval to apply acceleration. */
    public int acceleration_interval;
    /** String, if set will dump raw prob data to this file. */
    public string write_data_filename; // actually const char*
    /** String, if set will log data to this csv file (makes SCS very slow). */
    public string log_csv_filename; // actually const char*
  };

  public class ScsData {
    ///<value>A has `m` rows.</value>
    public int m;

    ///<value>A has `n` cols, P has `n` cols and `n` rows.</value>
    public int n;

    ///<value>A is supplied in CSC format (size `m` x `n`).</value>
    public ScsMatrix A;

    /// <summary>P is supplied in CSC format (size `n` x `n`), must be symmetric positive
    /// semidefinite. Only pass in the upper triangular entries. If `P = 0` then
    /// set `P = null`.
    /// </summary>
    public ScsMatrix P;

    ///<value>Dense array for b (size `m`).</value>
    public double[] b;

    ///<value>Dense array for c (size `n`).</value>
    public double[] c;
  };


  ///<summary>Cone data. Rows of data matrix `A` must be specified in this exact order.</summary>
  public class ScsCone {
    ///<value>Number of linear equality constraints (primal zero, dual free).</value>
    public int z;
    ///<value>Number of positive orthant cones.</value>
    public int l;
    ///<value>Upper box values, `len(bu) = len(bl) = max(bsize-1, 0)`.</value>
    public double[] bu;
    ///<value>Lower box values, `len(bu) = len(bl) = max(bsize-1, 0)`.</value>
    public double[] bl;
    ///<value>Total length of box cone (includes scale `t`).</value>
    public int bsize;
    ///<value>Array of second-order cone constraints.</value>
    public int[] q; // actually:  int *q;
    ///<value>Length of second-order cone array `q`.</value>
    public int qsize;
    ///<value>Array of semidefinite cone constraints.</value>
    public int[] s; // actually: int *s;
    ///<value>Length of semidefinite constraints array `s`.</value>
    public int ssize;
    ///<value> Number of primal exponential cone triples.</value>
    public int ep;
    ///<value>Number of dual exponential cone triples.</value>
    public int ed;
    ///<value>Array of power cone params, must be in `[-1, 1]`, negative values are
    ///interpreted as specifying the dual cone.</value>
    public double[] p; // actually double *p;
    ///<value>Number of (primal and dual) power cone triples.</value>
    public int psize;
  };

  ///<summary>Contains primal-dual solution arrays or a certificate of infeasibility.
  /// Check the exit flag to determine whether this contains a solution or a
  /// certificate.</summary> 
  public class ScsSolution {
    /** Primal variable. */
    public double[] x;
    /** Dual variable. */
    public double[] y;
    /** Slack variable. */
    public double[] s;
  };


  /** Contains information about the solve run at termination. */
  [StructLayout(LayoutKind.Sequential)]
  public class ScsInfo {
    /** Number of iterations taken. */
    public int iter;
    /** Status string, e.g. 'solved'. */
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
    public string status; // actually char[128]
    /** Status as int, defined in glbopts.h. */
    public int status_val;
    /** Number of updates to scale. */
    public int scale_updates;
    /** Primal objective. */
    public double pobj;
    /** Dual objective. */
    public double dobj;
    /** Primal equality residual. */
    public double res_pri;
    /** Dual equality residual. */
    public double res_dual;
    /** Duality gap. */
    public double gap;
    /** Infeasibility cert residual. */
    public double res_infeas;
    /** Unbounded cert residual. */
    public double res_unbdd_a;
    /** Unbounded cert residual. */
    public double res_unbdd_p;
    /** Time taken for setup phase (milliseconds). */
    public double setup_time;
    /** Time taken for solve phase (milliseconds). */
    public double solve_time;
    /** Final scale parameter. */
    public double scale;
    /** Complementary slackness. */
    public double comp_slack;
    /** Number of rejected AA steps. */
    public int rejected_accel_steps;
    /** Number of accepted AA steps. */
    public int accepted_accel_steps;
    /** Total time (milliseconds) spent in the linear system solver. */
    public double lin_sys_time;
    /** Total time (milliseconds) spent in the cone projection. */
    public double cone_time;
    /** Total time (milliseconds) spent in the acceleration routine. */
    public double accel_time;

    public override string ToString() {
      return status;
    }
  }

  public static class SCSWrapper {
    // in the following we define types for the parameters of the unmanaged functions

    /** Contains primal-dual solution arrays or a certificate of infeasibility.
     *  Check the exit flag to determine whether this contains a solution or a
     *  certificate. */
    [StructLayout(LayoutKind.Sequential)]
    private class _ScsSolution {
      /** Primal variable. */
      public IntPtr x; // actually double*
      /** Dual variable. */
      public IntPtr y;// actually double*
      /** Slack variable. */
      public IntPtr s;// actually double*
    };

    [StructLayout(LayoutKind.Sequential)]
    private class _ScsMatrix {
      ///<value>Matrix values, size: number of non-zeros.</value>
      public IntPtr x; // actually double *
      ///<value>Matrix row indices, size: number of non-zeros.</value>
      public IntPtr i; // actually int * 
      ///<value>Matrix column pointers, size: `n+1`.</value>
      public IntPtr p; // actually int *
      ///<value>Number of rows.</value>
      public int m;
      ///<value>Number of columns.</value>
      public int n;
    };

    /** Struct containing problem data. */
    [StructLayout(LayoutKind.Sequential)]
    private class _ScsData {
      ///<value>A has `m` rows.</value>
      public int m;

      ///<value>A has `n` cols, P has `n` cols and `n` rows.</value>
      public int n;

      ///<value>A is supplied in CSC format (size `m` x `n`).</value>
      public IntPtr A; // actually ScsMatrix *A;

      /// <summary>P is supplied in CSC format (size `n` x `n`), must be symmetric positive
      /// semidefinite. Only pass in the upper triangular entries. If `P = 0` then
      /// set `P = SCS_NULL`.
      /// </summary>
      public IntPtr P; // actually:  ScsMatrix *P;

      ///<value>Dense array for b (size `m`).</value>
      public IntPtr b; // actually double *b;

      ///<value>Dense array for c (size `n`).</value>
      public IntPtr c; // actually double *c;
    };

    /** Cone data. Rows of data matrix `A` must be specified in this exact order. */
    [StructLayout(LayoutKind.Sequential)]
    private class _ScsCone {
      ///<value>Number of linear equality constraints (primal zero, dual free).</value>
      public int z;
      ///<value>Number of positive orthant cones.</value>
      public int l;
      ///<value>Upper box values, `len(bu) = len(bl) = max(bsize-1, 0)`.</value>
      public IntPtr bu; // actually: double* bu;
      ///<value>Lower box values, `len(bu) = len(bl) = max(bsize-1, 0)`.</value>
      public IntPtr bl; // actually: double *bl;
      ///<value>Total length of box cone (includes scale `t`).</value>
      public int bsize;
      ///<value>Array of second-order cone constraints.</value>
      public IntPtr q; // actually:  int *q;
      ///<value>Length of second-order cone array `q`.</value>
      public int qsize;
      ///<value>Array of semidefinite cone constraints.</value>
      public IntPtr s; // actually: int *s;
      ///<value>Length of semidefinite constraints array `s`.</value>
      public int ssize;
      ///<value> Number of primal exponential cone triples.</value>
      public int ep;
      ///<value>Number of dual exponential cone triples.</value>
      public int ed;
      ///<value>Array of power cone params, must be in `[-1, 1]`, negative values are
      ///interpreted as specifying the dual cone.</value>
      public IntPtr p; // actually double *p;
      ///<value>Number of (primal and dual) power cone triples.</value>
      public int psize;
    };

    /**
     * Solve quadratic cone program defined by data in d and cone k.
     *
     * All the inputs must already be allocated in memory before calling.
     *
     * @param  d     Problem data.
     * @param  k     Cone data.
     * @param  stgs  SCS solver settings.
     * @param  sol   Solution will be stored here.
     * @param  info  Information about the solve will be stored here.
     * @return       Flag that determines solve type (see \a glbopts.h).
     */
    [DllImport("nativelib/libscsdir.dll", EntryPoint = "scs", CharSet = CharSet.Ansi,
                    CallingConvention = CallingConvention.Cdecl)]
    private static extern int Scs(_ScsData data, _ScsCone cone, ScsSettings settings, [In, Out] _ScsSolution solution, [In, Out] ScsInfo info); // int scs(const ScsData *d, const ScsCone *k, const ScsSettings *stgs,
                                                                                                                                                //             ScsSolution *sol, ScsInfo *info);
    public static int Scs(ScsData data, ScsCone cone, ScsSettings settings, out ScsSolution solution, out ScsInfo info) {
      var n = data.n;
      var m = data.m;
      solution = new ScsSolution() { x = new double[n], y = new double[m], s = new double[m] };

      using (var marshaller = new CustomMarshaller()) {
        var _data = marshaller.Marshal(data);
        var _cone = marshaller.Marshal(cone);
        var _sol = marshaller.Marshal(solution);
        info = new ScsInfo();
        var res = Scs(_data, _cone, settings, _sol, info);
        if (res >= 0) {
          Marshal.Copy(_sol.x, solution.x, 0, n);
          Marshal.Copy(_sol.y, solution.y, 0, m);
          Marshal.Copy(_sol.s, solution.s, 0, m);
        }
        return res;
      }
    }


    /**
     * Helper function to set all settings to default values (see \a glbopts.h).
     *
     * @param  stgs  Settings struct that will be populated.
     */
    [DllImport("nativelib/libscsdir.dll", EntryPoint = "scs_set_default_settings", CharSet = CharSet.Ansi,
                     CallingConvention = CallingConvention.Cdecl)]
    public static extern void SetDefaultSettings([In, Out] ScsSettings settings); // void SCS(set_default_settings)(ScsSettings *stgs);

    [DllImport("nativelib/libscsdir.dll", EntryPoint = "scs_version", CharSet = CharSet.Ansi,
                     CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr SCS_Version(); // const char *SCS(version)(void);
    public static string Version() {
      return Marshal.PtrToStringAnsi(SCS_Version());
    }

    [DllImport("nativelib/libscsdir.dll", EntryPoint = "scs_write_data", CharSet = CharSet.Ansi,
                 CallingConvention = CallingConvention.Cdecl)]
    private static extern void WriteData(_ScsData data, _ScsCone cone, ScsSettings settings);
    public static void WriteData(ScsData data, ScsCone cone, ScsSettings settings) {
      using (var marshaller = new CustomMarshaller()) {
        WriteData(marshaller.Marshal(data), marshaller.Marshal(cone), settings);
      }
    }

    [DllImport("nativelib/libscsdir.dll", EntryPoint = "scs_validate_cones", CharSet = CharSet.Ansi,
                 CallingConvention = CallingConvention.Cdecl)]
    private static extern int ValidateCones(_ScsData data, _ScsCone cone);
    public static int ValidateCones(ScsData data, ScsCone cone) {
      using (var marshaller = new CustomMarshaller()) {
        var _data = marshaller.Marshal(data);
        var _cone = marshaller.Marshal(cone);
        return ValidateCones(_data, _cone);
      }
    }

    #region custom marshalers

    // Marshaller for ScsData, ScsCone, ScsMatrix
    // manages all GCHandles and frees them correctly
    private class CustomMarshaller : IDisposable {
      private bool disposed = false;
      protected List<GCHandle> handles = new List<GCHandle>();

      #region disposing
      ~CustomMarshaller() => Dispose(false);

      public virtual void Dispose() {
        Dispose(true);
        // Suppress finalization.
        GC.SuppressFinalize(this);
      }

      public virtual void Dispose(bool disposing) {
        if (disposed) return;
        if (disposing) {
          foreach (var handle in handles) {
            handle.Free();
          }
          handles.Clear();
          handles = null;
        }
        disposed = true;
      }
      #endregion

      public _ScsCone Marshal(ScsCone cone) {
        if (cone == null) return null;
        // copy cone to unmanaged structure, pin arrays and replace with IntPtr
        return new _ScsCone {
          bsize = cone.bsize,
          bl = Pin(cone.bl),
          bu = Pin(cone.bu),
          ed = cone.ed,
          ep = cone.ep,
          l = cone.l,
          p = Pin(cone.p),
          psize = cone.psize,
          q = Pin(cone.q),
          qsize = cone.qsize,
          s = Pin(cone.s),
          ssize = cone.ssize,
          z = cone.z
        };
      }
      public _ScsSolution Marshal(ScsSolution solution) {
        return new _ScsSolution() {
          x = Pin(solution.x),
          y = Pin(solution.y),
          s = Pin(solution.s)
        };
      }

      public _ScsData Marshal(ScsData data) {
        if (data == null) return null;
        return new _ScsData {
          m = data.m,
          n = data.n,
          b = Pin(data.b),
          c = Pin(data.c),
          P = Pin(Marshal(data.P)),
          A = Pin(Marshal(data.A))
        };
      }

      public _ScsMatrix Marshal(ScsMatrix m) {
        if (m == null) return null;
        var _m = new _ScsMatrix {
          m = m.m,
          n = m.n,
          i = Pin(m.i),
          p = Pin(m.p),
          x = Pin(m.x)
        };
        return _m;
      }

      protected IntPtr Pin<T>(T obj) {
        if (obj == null) return IntPtr.Zero;
        var handle = GCHandle.Alloc(obj, GCHandleType.Pinned);
        handles.Add(handle);
        return handle.AddrOfPinnedObject();
      }

    }
    #endregion
  }
}
