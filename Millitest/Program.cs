﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopOptC;


using millipedeLibNET2;

namespace Millitest
{
    internal class Program
    {
        static void Main(string[] args)
        {

            /*
           ████████ ███████ ███████ ████████      ██████  █████  ███████ ███████      ██ 
              ██    ██      ██         ██        ██      ██   ██ ██      ██          ███ 
              ██    █████   ███████    ██        ██      ███████ ███████ █████        ██ 
              ██    ██           ██    ██        ██      ██   ██      ██ ██           ██ 
              ██    ███████ ███████    ██         ██████ ██   ██ ███████ ███████      ██ 
            */

            //var rs = new StatModel();
            //var n0 = rs.AddNode(0, 0, 0, BOUNDARYCONDITIONS.ALL);
            //var n1 = rs.AddNode(10, 0, 0);

            //Console.WriteLine(n0);
            //Console.WriteLine(n1);

            //n1.AddLoad(0, 0, -1.0);

            //var mat = new StatMaterial("mat")
            //{
            //    Em = 100000,
            //    Poisson = 0.1,
            //    Gm = 100.0
            //};
            //Console.WriteLine(mat);

            //var cs = new StatCrossSection("cs", mat);
            //cs.CircSolid(0.1);
            //cs.Area = 0.01;
            //cs.Jxx = 0.0001;
            //cs.Iyy = 0.0001;
            //cs.Izz = 0.0001;

            //Console.WriteLine(cs);


            //var beam = rs.AddBeam(n0, n1, cs);
            //Console.WriteLine(beam);

            //rs.SolveSystem();

            //Console.WriteLine("Building stiffness matrix using the beam object");

            //var k = new double[12 * 12];
            //beam.BuildStiffnessMatrix(k);
            ////print stiffness matrix to look like a matrix
            ////i know that this will be a 12x12 matrix
            //for (int i = 0; i < 12; i++)
            //{
            //    for (int j = 0; j < 12; j++)
            //    {
            //        Console.Write(k[i * 12 + j] + "\t");
            //    }
            //    Console.WriteLine();
            //}


            //Console.WriteLine("\n");

            //Console.WriteLine("Global Stiffness Matrix");

            //Console.WriteLine("\n");



            //var K = rs.GetGlobalStiffnessMatrix();
            ////print stiffness matrix to look like a matrix
            ////i know that this will be a 12x12 matrix
            //for (int i = 0; i < 12; i++)
            //{
            //    for (int j = 0; j < 12; j++)
            //    {
            //        Console.Write(K[i * 12 + j] + "\t");
            //    }
            //    Console.WriteLine();
            //}

            ////print the displacement vector
            //var value = rs.MaximumDisplacement;

            //Console.WriteLine("\n");
            //Console.WriteLine("Maximum Displacement");
            //Console.WriteLine(value);


            //Console.WriteLine("\n");
            //Console.WriteLine("Displacement vectors");
            ////print the difflections vector
            //foreach (var node in rs.Nodes)
            //{
            //    Console.WriteLine("{" + node.u.x + ", " + node.u.y + ", " + node.u.z + ", " + node.r.x + ", " + node.r.y + ", " + node.r.z + "}");
            //}


            ////keep console open
            //Console.ReadLine();



            /*
            ████████ ███████ ███████ ████████      ██████  █████  ███████ ███████     ██████  
               ██    ██      ██         ██        ██      ██   ██ ██      ██               ██ 
               ██    █████   ███████    ██        ██      ███████ ███████ █████        █████  
               ██    ██           ██    ██        ██      ██   ██      ██ ██          ██      
               ██    ███████ ███████    ██         ██████ ██   ██ ███████ ███████     ███████                                                                                                                                                                 
            */

            //var rs = new StatModel();
            //var n0 = rs.AddNode(0, 0, 0, BOUNDARYCONDITIONS.ALL);
            //var n1 = rs.AddNode(10, 0, 10);


            //Console.WriteLine(n0);
            //Console.WriteLine(n1);

            //n1.AddLoad(0, 0, -1);

            //var mat = new StatMaterial("mat")
            //{
            //    Em = 100000,
            //    Poisson = 0.1,
            //    Gm = 100.0
            //};
            //Console.WriteLine(mat);

            //var cs = new StatCrossSection("cs", mat);
            //cs.CircSolid(0.1);
            //cs.Area = 0.01;
            //cs.Jxx = 0.0001;
            //cs.Iyy = 0.0001;
            //cs.Izz = 0.0001;

            //Console.WriteLine(cs);


            //var beam1 = rs.AddBeam(n0, n1, cs);
            //Console.WriteLine(beam1);

            //rs.SolveSystem();



            //Console.WriteLine("b1");
            //Console.WriteLine("\n");
            //var k = new double[12 * 12];
            //beam1.BuildStiffnessMatrix(k);
            ////print stiffness matrix to look like a matrix
            ////i know that this will be a 12x12 matrix
            //for (int i = 0; i < 12; i++)
            //{
            //    for (int j = 0; j < 12; j++)
            //    {
            //        Console.Write(Math.Round(k[i * 12 + j], 3) + "\t");
            //    }
            //    Console.WriteLine();
            //}
            //Console.WriteLine("\n");



            //Console.WriteLine("\n");

            //Console.WriteLine("Global Stiffness Matrix");

            //Console.WriteLine("\n");



            //var K = rs.GetGlobalStiffnessMatrix();
            ////print stiffness matrix to look like a matrix
            ////i know that this will be a 12x12 matrix
            //for (int i = 0; i < 12; i++)
            //{
            //    for (int j = 0; j < 12; j++)
            //    {
            //        Console.Write(Math.Round(K[i * 12 + j], 3) + "\t");
            //    }
            //    Console.WriteLine();
            //}

            ////print the displacement vector
            //var value = rs.MaximumDisplacement;

            //Console.WriteLine("\n");
            //Console.WriteLine("Maximum Displacement");
            //Console.WriteLine(value);


            //Console.WriteLine("\n");
            //Console.WriteLine("Displacement vectors");
            ////print the difflections vector
            //foreach (var node in rs.Nodes)
            //{
            //    Console.WriteLine("{" + node.u.x + ", " + node.u.y + ", " + node.u.z + ", " + node.r.x + ", " + node.r.y + ", " + node.r.z + "}");
            //}


            ////keep console open
            //Console.ReadLine();
            ///*
            //████████ ███████ ███████ ████████      ██████  █████  ███████ ███████     ██████  
            //   ██    ██      ██         ██        ██      ██   ██ ██      ██               ██ 
            //   ██    █████   ███████    ██        ██      ███████ ███████ █████        █████  
            //   ██    ██           ██    ██        ██      ██   ██      ██ ██               ██ 
            //   ██    ███████ ███████    ██         ██████ ██   ██ ███████ ███████     ██████  



            var rs = new StatModel();
            var n0 = rs.AddNode(0, 0, 0, BOUNDARYCONDITIONS.ALL);
            var n1 = rs.AddNode(10, 0, 10);
            var n2 = rs.AddNode(20, 0, 0);


            Console.WriteLine(n0);
            Console.WriteLine(n1);
            Console.WriteLine(n2);

            n1.AddLoad(1, 0, 0);


            var mat = new StatMaterial("mat")
            {
                Em = 100000,
                Poisson = 0.1,
                Gm = 100.0
            };
            Console.WriteLine(mat);

            var cs = new StatCrossSection("cs", mat);
            cs.CircSolid(0.1);
            cs.Area = 0.01;
            cs.Jxx = 0.0001;
            cs.Iyy = 0.0001;
            cs.Izz = 0.0001;

            Console.WriteLine(cs);


            var beam1 = rs.AddBeam(n0, n1, cs);
            var beam2 = rs.AddBeam(n1, n2, cs);
            var beam3 = rs.AddBeam(n2, n0, cs);
            Console.WriteLine(beam1);

            rs.SolveSystem();



            Console.WriteLine("b0");
            Console.WriteLine("\n");
            var k = new double[12 * 12];
            beam1.BuildStiffnessMatrix(k);
            //print stiffness matrix to look like a matrix
            //i know that this will be a 12x12 matrix
            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    Console.Write(Math.Round(k[i * 12 + j], 3) + "\t");
                }
                Console.WriteLine();
            }
            Console.WriteLine("\n");



            Console.WriteLine("b1");
            Console.WriteLine("\n");
            var k1 = new double[12 * 12];
            beam2.BuildStiffnessMatrix(k1);
            //print stiffness matrix to look like a matrix
            //i know that this will be a 12x12 matrix
            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    Console.Write(Math.Round(k1[i * 12 + j], 3) + "\t");
                }
                Console.WriteLine();
            }
            Console.WriteLine("\n");




            Console.WriteLine("b2");
            Console.WriteLine("\n");
            var k2 = new double[12 * 12];
            beam3.BuildStiffnessMatrix(k2);
            //print stiffness matrix to look like a matrix
            //i know that this will be a 12x12 matrix
            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    Console.Write(Math.Round(k2[i * 12 + j], 3) + "\t");
                }
                Console.WriteLine();
            }
            Console.WriteLine("\n");






            Console.WriteLine("\n");

            Console.WriteLine("Global Stiffness Matrix");

            Console.WriteLine("\n");



            var K = rs.GetGlobalStiffnessMatrix();
            //print stiffness matrix to look like a matrix
            //i know that this will be a 12x12 matrix
            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    Console.Write(Math.Round(K[i * 12 + j], 3) + "\t");
                }
                Console.WriteLine();
            }

            //print the displacement vector
            var value = rs.MaximumDisplacement;

            Console.WriteLine("\n");
            Console.WriteLine("Maximum Displacement");
            Console.WriteLine(value);


            Console.WriteLine("\n");
            Console.WriteLine("Displacement vectors");
            //print the difflections vector
            foreach (var node in rs.Nodes)
            {
                Console.WriteLine("{" + node.u.x + ", " + node.u.y + ", " + node.u.z + ", " + node.r.x + ", " + node.r.y + ", " + node.r.z + "}");
            }


            //keep console open
            Console.ReadLine();
            /* 
            ██████  ██ ██   ██ ███████ ██      ███████     ███████ ██ ███████ ██      ██████  
            ██   ██ ██  ██ ██  ██      ██      ██          ██      ██ ██      ██      ██   ██ 
            ██████  ██   ███   █████   ██      ███████     █████   ██ █████   ██      ██   ██ 
            ██      ██  ██ ██  ██      ██           ██     ██      ██ ██      ██      ██   ██ 
            ██      ██ ██   ██ ███████ ███████ ███████     ██      ██ ███████ ███████ ██████  
            */


            //    TopoptField model = new TopoptField(3, 3, new Material(1.0, 0.3));


            //    //fix left side nodes
            //    for (int j = 0; j < model.NodeCountY; ++j)
            //    {
            //        model.Nodes[model.NodeId(0, j)].BC = BoundaryCondition.All;
            //    }

            //    //apply load to right side nodes
            //    for (int j = 0; j < model.NodeCountY; ++j)
            //    {
            //        model.Nodes[model.NodeId(model.NodeCountX - 1, j)].F = new Vec2(0.0, -1.0);
            //    }

            //model.FEAnalysis();

            //    model.ComputeSensitivity();
            //    model.UpdateDensity();

            //    Console.WriteLine(model.ComputeMaxDisplacement());
            //    Console.ReadLine();


        }
    }
}
