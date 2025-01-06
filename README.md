# BettaFEA vesion Alpha 

This repository hosts a C# implementation of FEA as part of an ongiong research conducted by Zeiad Amin at Cornell University.

> **⚠️ Note:** This project is currently under development. Full documentation is forthcoming. For now, use this Quick Start guide to get started with one of the test cases.

## Quick Start Guide

### Prerequisites

To run the code, ensure you have the following installed:

- **.NET SDK** (version 7.0 or higher)
- A compatible IDE, such as Visual Studio or JetBrains Rider

### Getting Started

1. Clone the repository:

   ```bash
   git clone https://github.com/aminzeiad/BettaFEA.git
   cd fea-csharp
   ```

2. Open the solution file (`BettaFEA.sln`) in your preferred IDE.

3. Navigate to the `Program.cs` file under the project `BettaConsoleUI`. It contains a simple test case to help you understand the framework.

4. Build and run the solution. The output will be displayed in the console.

### Test Case: Simple Beam Structure

This example demonstrates how to define a simple beam structure, apply loads and constraints, and perform a finite element analysis. Here's an overview of what the test case does:

#### 1. Create a Structure

A `Structure` object is created to hold all elements, including nodes, beams, materials, and cross-sections.

#### 2. Add a Material

We define a basic material for the structure using the `Material` class:

```csharp
Material m1 = new();
Console.WriteLine(m1 + "\n");
```

#### 3. Add a Cross Section

We define a circular cross-section with specific properties:

```csharp
CrossSection cs = new("Circ", 0.01, 0.0001, 0.0001, 0.0001, m1);
Console.WriteLine(cs + "\n");
```

#### 4. Add Nodes

Nodes define key points in the structure. Here, we add two nodes:

```csharp
Point3 p1 = new Point3(0.0, 0.0, 0.0);
Point3 p2 = new Point3(10.0, 0.0, 10.0);

Node n1 = str.AddNode(p1);
Node n2 = str.AddNode(p2);
```

#### 5. Add a Beam

We connect the two nodes with a beam using the defined cross-section:

```csharp
Beam b1 = str.AddBeam(n1, n2, cs, new Vector3(0, 0, 1));
Console.WriteLine(b1 + "\n");
```

#### 6. Define a Load Case

Load cases specify loads and constraints. In this example:

- Node `n1` is fully constrained.
- Node `n2` has a point load applied in the negative Z-direction.

```csharp
LoadCase lc = new LoadCase("Test Case");

lc.AddSupport(n1, Constraints.All);
lc.AddPointLoad(n2, 0, 0, -1, 0, 0, 0);
Console.WriteLine(lc + "\n");
```

#### 7. Perform Analysis

Finally, we create an `FEModel` to combine the structure and load case, then perform the analysis:

```csharp
FEModel model = new FEModel(str, lc);

model.PerformAnalysis();
Console.WriteLine(model + "\n");
```

### Running the Code

When you run the program, it outputs information about each component (materials, cross-sections, beams, nodes, and loads) and the results of the analysis.

### Sample Output

```text
Default
E: 100000 | V: 0.1 | p: 100 | σ: 100 | α: 1E-05

CrossSection Circ
Area: 0.01
Ixx: 0.0001
Iyy: 0.0001
Izz: 0.0001
Material: Default

Beam from Node (0, 0, 0) with Boundary Conditions (False, False, False, False, False, False) to Node (10, 0, 10) with Boundary Conditions (False, False, False, False, False, False)
CrossSection: CrossSection Circ
Area: 0.01
Ixx: 0.0001
Iyy: 0.0001
Izz: 0.0001
Material: Default
Length: 14.142135623730951
CG: (5, 0, 5)
Up: (0, 0, 1)


Load Case: Test Case with 1 Loads and 1 Supports

__________________________________________________________________________________
__________________________________________________________________________________

FE Model:
__________________________________________________________________________________
__________________________________________________________________________________

Beams:
__________________________________________________________________________________
__________________________________________________________________________________
A Beam FROM: Node 0 at (0, 0, 0)TO: Node 1 at (10, 0, 10)
T Matrix:
DenseMatrix 12x12-Double
0.707107   0   0.707107         0   0          0         0   0          0         0   0          0
       0  -1          0         0   0          0         0   0          0         0   0          0
0.707107   0  -0.707107         0   0          0         0   0          0         0   0          0
       0   0          0  0.707107   0   0.707107         0   0          0         0   0          0
       0   0          0         0  -1          0         0   0          0         0   0          0
       0   0          0  0.707107   0  -0.707107         0   0          0         0   0          0
       0   0          0         0   0          0  0.707107   0   0.707107         0   0          0
       0   0          0         0   0          0         0  -1          0         0   0          0
       0   0          0         0   0          0  0.707107   0  -0.707107         0   0          0
       0   0          0         0   0          0         0   0          0  0.707107   0   0.707107
       0   0          0         0   0          0         0   0          0         0  -1          0
       0   0          0         0   0          0         0   0          0  0.707107   0  -0.707107

k Matrix:
DenseMatrix 12x12-Double
 70.7107           0           0             0        0        0  -70.7107           0           0             0        0        0
       0   0.0424264           0             0        0      0.3         0  -0.0424264           0             0        0      0.3
       0           0   0.0424264             0     -0.3        0         0           0  -0.0424264             0     -0.3        0
       0           0           0   0.000707107        0        0         0           0           0  -0.000707107        0        0
       0           0        -0.3             0  2.82843        0         0           0         0.3             0  1.41421        0
       0         0.3           0             0        0  2.82843         0        -0.3           0             0        0  1.41421
-70.7107           0           0             0        0        0   70.7107           0           0             0        0        0
       0  -0.0424264           0             0        0     -0.3         0   0.0424264           0             0        0     -0.3
       0           0  -0.0424264             0      0.3        0         0           0   0.0424264             0      0.3        0
       0           0           0  -0.000707107        0        0         0           0           0   0.000707107        0        0
       0           0        -0.3             0  1.41421        0         0           0         0.3             0  2.82843        0
       0         0.3           0             0        0  1.41421         0        -0.3           0             0        0  2.82843

T' k T Matrix
DenseMatrix 12x12-Double
 35.3766           0    35.3341          0   0.212132          0   -35.3766           0  -35.3341          0   0.212132          0
       0   0.0424264          0  -0.212132          0   0.212132          0  -0.0424264         0  -0.212132          0   0.212132
 35.3341           0    35.3766          0  -0.212132          0   -35.3341           0  -35.3766          0  -0.212132          0
       0   -0.212132          0    1.41457          0   -1.41386          0    0.212132         0   0.706753          0   -0.70746
0.212132           0  -0.212132          0    2.82843          0  -0.212132           0  0.212132          0    1.41421          0
       0    0.212132          0   -1.41386          0    1.41457          0   -0.212132         0   -0.70746          0   0.706753
-35.3766           0   -35.3341          0  -0.212132          0    35.3766           0   35.3341          0  -0.212132          0
       0  -0.0424264          0   0.212132          0  -0.212132          0   0.0424264         0   0.212132          0  -0.212132
-35.3341           0   -35.3766          0   0.212132          0    35.3341           0   35.3766          0   0.212132          0
       0   -0.212132          0   0.706753          0   -0.70746          0    0.212132         0    1.41457          0   -1.41386
0.212132           0  -0.212132          0    1.41421          0  -0.212132           0  0.212132          0    2.82843          0
       0    0.212132          0   -0.70746          0   0.706753          0   -0.212132         0   -1.41386          0    1.41457

______________________________________________________________________________
______________________________________________________________________________

Nodes:
______________________________________________________________________________
______________________________________________________________________________
Node 0 at (0, 0, 0)
Node 0 has a force of (0, 0, 0)
Node 0 has a moment of (0, 0, 0)
Node 0 has deflections of SparseMatrix 6x1-Double 0.00 % Filled
0
0
0
0
0
0

______________________________________________________________________________
______________________________________________________________________________
Node 1 at (10, 0, 10)
Node 1 has a force of (0, 0, -1)
Node 1 has a moment of (0, 0, 0)
Node 1 has deflections of SparseMatrix 6x1-Double 50.00 % Filled
 47.1334
       0
-47.1475
       0
 7.07107
       0

______________________________________________________________________________
______________________________________________________________________________
Global Stiffness Matrix:
SparseMatrix 12x12-Double 16.67 % Filled
1  0  0  0  0  0          0          0         0         0          0          0
0  1  0  0  0  0          0          0         0         0          0          0
0  0  1  0  0  0          0          0         0         0          0          0
0  0  0  1  0  0          0          0         0         0          0          0
0  0  0  0  1  0          0          0         0         0          0          0
0  0  0  0  0  1          0          0         0         0          0          0
0  0  0  0  0  0    35.3766          0   35.3341         0  -0.212132          0
0  0  0  0  0  0          0  0.0424264         0  0.212132          0  -0.212132
0  0  0  0  0  0    35.3341          0   35.3766         0   0.212132          0
0  0  0  0  0  0          0   0.212132         0   1.41457          0   -1.41386
0  0  0  0  0  0  -0.212132          0  0.212132         0    2.82843          0
0  0  0  0  0  0          0  -0.212132         0  -1.41386          0    1.41457

Global Load Matrix:
DenseVector 12-Double
 0
 0
 0
 0
 0
 0
 0
 0
-1
 0
 0
 0

______________________________________________________________________________
______________________________________________________________________________
Global Displacement Matrix:
DenseMatrix 12x1-Double
       0
       0
       0
       0
       0
       0
 47.1334
       0
-47.1475
       0
 7.07107
       0

______________________________________________________________________________
______________________________________________________________________________
Nodes Deflections
Node 0 at (0, 0, 0)
Node 0 has deflections of SparseMatrix 6x1-Double 0.00 % Filled
0
0
0
0
0
0

______________________________________________________________________________
______________________________________________________________________________
Node 1 at (10, 0, 10)
Node 1 has deflections of SparseMatrix 6x1-Double 50.00 % Filled
 47.1334
       0
-47.1475
       0
 7.07107
       0

______________________________________________________________________________
______________________________________________________________________________
```

### Roadmap

This project is in active development, with the following features planned:

- Enhanced documentation
- Additional element types (e.g., shell, plate)
- GUI for easier modeling and visualization
- Unit tests and benchmarks

### License

This project is licensed under the Creative Commons Attribution-NonCommercial 4.0 International License.

