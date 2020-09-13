using System;
using System.Drawing;
using System.Collections.Generic;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino;
using Rhino.Geometry;
using Rhino.Geometry.Collections;


namespace Angelfish
{
    public class Region : Pattern
    {
        PolylineCurve curveAsPoly;
        List<Point3d> edgePoints; 


        public Region(Curve _curve) : base()
        {
            RhinoDoc doc = RhinoDoc.ActiveDoc; 
            curveAsPoly = _curve.ToPolyline(doc.ModelAbsoluteTolerance, doc.ModelAngleToleranceRadians, 0.1, 10.0);
        }

        public Region(Mesh _mesh, List<double> _values, Curve _curve) : base()
        {
            //curve = _curve;
            mesh = _mesh;

            InitRegion(_values);
        }

        public Region (List<Apoint> _apoints) : base()
        {
            Apoints = _apoints;
        }

        public Region (Asystem _asystem)
        {
            this.Apoints = _asystem.Apoints;
            this.edgeCount = _asystem.edgeCount;
            this.min = _asystem.min;
            this.max = _asystem.max;

            this.excludeX = _asystem.excludeX;
            this.excludeY = _asystem.excludeY;
            this.excludeZ = _asystem.excludeZ;

            this.mesh = _asystem.mesh;
        }

       
        void InitRegion(List<double> _values)
        {
            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
               // if(curveAsPoly.Contains(mesh.Vertices[i]))
            }
        }

    }
}