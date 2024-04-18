// See https://aka.ms/new-console-template for more information
using BettaLib.FEAModel;
using BettaLib.FEAStructure;
using BettaLib.Geometry;


//start the structure
Structure str = new Structure();

//////////////////////////////////////////////////////////////////////////////////////

//add material to the system
Material m1 = Material.MakeSteel("Steel");

Console.WriteLine(m1 + "\n");

/*
//you can edit the material properties of a material 
m1.ElasticModulus = 200000;
m1.PoissonRatio = 0.35;
m1.Density = 7650;
m1.ThermalExpansionCoefficient = 0.00001;
m1.YieldStrength = 220;
*/

Console.WriteLine(m1 + "\n");

//s.AddMaterial(m)XXX;
//Actually this is not needed because the material is added to the cross section also it's better to be floating as it coould be used in multiple structures

/////////////////////////////////////////////////////////////////////////////////////

//Add cross section to the system
CrossSection cs1 = CrossSection.MakeCircularHollow("CircHollow", 0.05, 0.03, 0.01, m1);

/*
//you can edit the cross section properties 
cs1.Izz = 100;
cs1.Iyy = 100;
cs1.Ixx = 100;
cs1.Area = 100;
*/

Console.WriteLine(cs1 + "\n");
//s.AddCrossSection(s)XXX;
//Actually this is not needed because the cross section is added to the beam also it's better to be floating as it coould be used in multiple structures

/////////////////////////////////////////////////////////////////////////////////////

//Add a node to the system
Point3 p1 = new Point3(0.0, 0.0, 0.0);
Point3 p2 = new Point3(5.0, 0.0, 0.0);


Node n1 = str.AddNode(p1);
Node n2 = str.AddNode(p2);


Console.WriteLine(n1.Position + "\n");
Console.WriteLine(n1 + "\n");

/////////////////////////////////////////////////////////////////////////////////////

//Add beams to the system

Beam b1 = str.AddBeam(n1, n2, cs1); //Could also be added using the nodes

Console.WriteLine(b1 + "\n");


/////////////////////////////////////////////////////////////////////////////////////

//Add the loadcase which include loads and supports

LoadCase lc = new LoadCase("Test Case");

lc.AddSupport(n1, Constraints.All);


Console.WriteLine(n1 + "\n");
Console.WriteLine(n2 + "\n");

lc.AddPointLoad(n2, 0, 0, -200, 0, 0, 0, n2);


Console.WriteLine(lc + "\n");





/////////////////////////////////////////////////////////////////////////////////////
FEModel model = new FEModel(str, lc);

model.PerformAnalysis();

Console.WriteLine(model + "\n");
