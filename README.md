# HEAL.SCS 

Provides a .NET wrapper to SCS.

SCS (splitting conic solver) is a numerical optimization package for solving
large-scale convex cone problems. This package includes windows binaries of SCS version 3.0.0.

The full documentation for SCS is available at [https://www.cvxgrp.org/scs/](https://www.cvxgrp.org/scs/). 

## Building

```
git clone https://github.com/heal-research/HEAL.SCS
cd HEAL.SCS
dotnet build
```

## Running 
Demo:
```
cd Demo
dotnet run
```

Unit tests:
`dotnet test`

## Usage example
```csharp
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
var data = new ScsData() {
  A = Util.DenseToSparse(A),
  b = new double[] { 0.0, 0.0, Math.Sqrt(2), 0.0, 0.0 },
  c = new double[] { -1, -1 },
  m = 5,
  n = 2,
};

var cone = new ScsCone() {
  l = 2,
  qsize = 1,
  q = new int[] { 3 },
};

if (0 <= SCSWrapper.Scs(data, cone, settings, out var solution, out var info)) {
  Console.WriteLine(info);
  Console.WriteLine($"x = {string.Join(", ", solution.x)}");
  Console.WriteLine($"y = {string.Join(", ", solution.y)}");
  Console.WriteLine($"s = {string.Join(", ", solution.s)}");
}
```

## Building SCS
Clone SCS from github and build it. I used [MSYS2](https://www.msys2.org/) on Windows to build the binaries. 
```
git clone https://github.com/cvxgrp/scs
cd scs
make 
```

The native scs library can be found in the `out/` folder. Copy `libscsdir.dll` and all other native dll files to `HEAL.SCS/nativelib`.


## Helping out
We are open to pull-requests.
