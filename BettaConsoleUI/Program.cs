// See https://aka.ms/new-console-template for more information
using BettaLib.FEAModel;
using BettaLib.FEAModelPix;
using BettaLib.FEAStructure;
using BettaLib.Geometry;
using OpenTK.Graphics.OpenGL;

/*
████████ ███████ ███████ ████████      ██████  █████  ███████ ███████      ██ 
   ██    ██      ██         ██        ██      ██   ██ ██      ██          ███ 
   ██    █████   ███████    ██        ██      ███████ ███████ █████        ██ 
   ██    ██           ██    ██        ██      ██   ██      ██ ██           ██ 
   ██    ███████ ███████    ██         ██████ ██   ██ ███████ ███████      ██ 
 */
 

////start the structure
//Structure str = new Structure(); 

////////////////////////////////////////////////////////////////////////////////////////

////add material to the system
//Material m1 = new();

//Console.WriteLine(m1 + "\n");

///////////////////////////////////////////////////////////////////////////////////////

////Add cross section to the system
//CrossSection cs = new("Circ", 0.01, 0.0001, 0.0001, 0.0001, m1);

//Console.WriteLine(cs + "\n");

///////////////////////////////////////////////////////////////////////////////////////

////Add a node to the system
//Point3 p1 = new Point3(0.0, 0.0, 0.0);
//Point3 p2 = new Point3(10.0, 0.0, 0.0);


//Node n1 = str.AddNode(p1);
//Node n2 = str.AddNode(p2);


//Console.WriteLine(n1 + "\n");
//Console.WriteLine(n2 + "\n");

///////////////////////////////////////////////////////////////////////////////////////

////Add beams to the system
//Beam b1 = str.AddBeam(n1, n2, cs, new Vector3(0, 0, 1));

//Console.WriteLine(b1 + "\n");

////////////////////////////////////////////////////////////////////////////////////////

////Add the loadcase which include loads and supports

//LoadCase lc = new LoadCase("Test Case");

//lc.AddSupport(n1, Constraints.All);

//Console.WriteLine(n1 + "\n");
//Console.WriteLine(n2 + "\n");

//lc.AddPointLoad(n2, 0, 0, -1, 0, 0, 0);

//Console.WriteLine(lc + "\n");

///////////////////////////////////////////////////////////////////////////////////////
//FEModel model = new FEModel(str, lc);

//model.PerformAnalysis();

//Console.WriteLine(model + "\n");



/*
████████ ███████ ███████ ████████      ██████  █████  ███████ ███████     ██████  
   ██    ██      ██         ██        ██      ██   ██ ██      ██               ██ 
   ██    █████   ███████    ██        ██      ███████ ███████ █████        █████  
   ██    ██           ██    ██        ██      ██   ██      ██ ██          ██      
   ██    ███████ ███████    ██         ██████ ██   ██ ███████ ███████     ███████                                                                                                                                                                 
*/



//start the structure
Structure str = new Structure();

//////////////////////////////////////////////////////////////////////////////////////

//add material to the system
Material m1 = new();

Console.WriteLine(m1 + "\n");

/////////////////////////////////////////////////////////////////////////////////////

//Add cross section to the system
CrossSection cs = new("Circ", 0.01, 0.0001, 0.0001, 0.0001, m1);

Console.WriteLine(cs + "\n");

/////////////////////////////////////////////////////////////////////////////////////

//Add a node to the system
Point3 p1 = new Point3(0.0, 0.0, 0.0);
Point3 p2 = new Point3(10.0, 0.0, 10.0);



Node n1 = str.AddNode(p1);
Node n2 = str.AddNode(p2);


/////////////////////////////////////////////////////////////////////////////////////

//Add beams to the system
Beam b1 = str.AddBeam(n1, n2, cs, new Vector3(0, 0, 1));

Console.WriteLine(b1 + "\n");


//////////////////////////////////////////////////////////////////////////////////////

//Add the loadcase which include loads and supports

LoadCase lc = new LoadCase("Test Case");

lc.AddSupport(n1, Constraints.All);




lc.AddPointLoad(n2, 0, 0, -1, 0, 0, 0);

Console.WriteLine(lc + "\n");

/////////////////////////////////////////////////////////////////////////////////////
FEModel model = new FEModel(str, lc);

model.PerformAnalysis();

Console.WriteLine(model + "\n");

///*
//████████ ███████ ███████ ████████      ██████  █████  ███████ ███████     ██████  
//   ██    ██      ██         ██        ██      ██   ██ ██      ██               ██ 
//   ██    █████   ███████    ██        ██      ███████ ███████ █████        █████  
//   ██    ██           ██    ██        ██      ██   ██      ██ ██               ██ 
//   ██    ███████ ███████    ██         ██████ ██   ██ ███████ ███████     ██████  


////start the structure
//Structure str = new Structure();

////////////////////////////////////////////////////////////////////////////////////////

////add material to the system
//Material m1 = new();

//Console.WriteLine(m1 + "\n");

///////////////////////////////////////////////////////////////////////////////////////

////Add cross section to the system
//CrossSection cs = new("Circ", 0.01, 0.0001, 0.0001, 0.0001, m1);

//Console.WriteLine(cs + "\n");

///////////////////////////////////////////////////////////////////////////////////////

////Add a node to the system
//Point3 p1 = new Point3(0.0, 0.0, 0.0);
//Point3 p2 = new Point3(10.0, 0.0, 10.0);
//Point3 p3 = new Point3(20.0, 0.0, 0.0);



//Node n1 = str.AddNode(p1);
//Node n2 = str.AddNode(p2);
//Node n3 = str.AddNode(p3);


///////////////////////////////////////////////////////////////////////////////////////

////Add beams to the system
//Beam b1 = str.AddBeam(n2, n1, cs, new Vector3(0, 0, 1));
//Beam b2 = str.AddBeam(n2, n3, cs, new Vector3(0, 0, 1));
//Beam b3 = str.AddBeam(n3, n1, cs, new Vector3(0, 0, 1));
//Beam b4 = str.AddBeam(n3, n1, cs, new Vector3(0, 0, 1));
//Beam b5 = str.AddBeam(n1, n2, cs, new Vector3(0, 0, 1));




////////////////////////////////////////////////////////////////////////////////////////

////Add the loadcase which include loads and supports

//LoadCase lc = new LoadCase("Test Case");

//lc.AddSupport(n1, Constraints.All);




//lc.AddPointLoad(n2, 1, 0, 0, 0, 0, 0);

//Console.WriteLine(lc + "\n");

///////////////////////////////////////////////////////////////////////////////////////
//FEModel model = new FEModel(str, lc);

//model.PerformAnalysis();

//Console.WriteLine(model + "\n");

//Console.WriteLine(model.MaximumDisplacement());

/* 
██████  ██ ██   ██ ███████ ██      ███████     ███████ ██ ███████ ██      ██████  
██   ██ ██  ██ ██  ██      ██      ██          ██      ██ ██      ██      ██   ██ 
██████  ██   ███   █████   ██      ███████     █████   ██ █████   ██      ██   ██ 
██      ██  ██ ██  ██      ██           ██     ██      ██ ██      ██      ██   ██ 
██      ██ ██   ██ ███████ ███████ ███████     ██      ██ ███████ ███████ ██████  
*/

//FEPixField model = new FEPixField(3, 3, new Material("Dummy", 1.0, 0.3, 1.0, 1.0, 1.0));


////fix left side nodes
//for (int j = 0; j < model.NodeCountY; ++j)
//{
//    model.Nodes[0 + j * model.NodeCountX].SupportType = FENode2SupportType.All;
//}

////apply load to right side nodes
//for (int j = 0; j < model.NodeCountY; ++j)
//{
//    model.Nodes[model.NodeCountX - 1 + j * model.NodeCountX].F = new BettaLib.Geometry.Vector2(0.0, -1.0);
//}

//model.PeformAnalysis();

//model.ComputeSensitivity();
//model.UpdateDensity();

//Console.WriteLine(model.MaximumDisplacement());

