/*
Microsoft Automatic Graph Layout,MSAGL 

Copyright (c) Microsoft Corporation

All rights reserved. 

MIT License 

Permission is hereby granted, free of charge, to any person obtaining
a copy of this software and associated documentation files (the
""Software""), to deal in the Software without restriction, including
without limitation the rights to use, copy, modify, merge, publish,
distribute, sublicense, and/or sell copies of the Software, and to
permit persons to whom the Software is furnished to do so, subject to
the following conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Msagl;
using System.Linq;
using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Core.Layout;
using Node = Microsoft.Msagl.Core.Layout.Node;

namespace LocationLabeling {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
            Node n0 = new Node( CurveFactory.CreateEllipse(10, 10, new Microsoft.Msagl.Core.Geometry.Point()),"a");
            Microsoft.Msagl.Core.Geometry.Point n1Center = new Microsoft.Msagl.Core.Geometry.Point(20, 0);
            Node n1 = new Node( CurveFactory.CreateEllipse(10, 10, n1Center),"b");
            n1.Center = n1Center;
            Microsoft.Msagl.Core.Geometry.Point n2Center = new Microsoft.Msagl.Core.Geometry.Point(5, 3);
            Node n2 = new Node( CurveFactory.CreateEllipse(10, 10, n2Center),"c");
            n2.Center = n2Center;

            Microsoft.Msagl.Core.Geometry.Point n3Center = new Microsoft.Msagl.Core.Geometry.Point(8, -1);
            Node n3 = new Node( CurveFactory.CreateEllipse(10, 10, n3Center),"d");
            n3.Center = n3Center;
            Microsoft.Msagl.Core.Geometry.Point n4Center = new Microsoft.Msagl.Core.Geometry.Point(6, -4);
            Node n4 = new Node( CurveFactory.CreateEllipse(10, 10, n4Center),"e");
            n4.Center = n4Center;

            Microsoft.Msagl.Core.Geometry.Point n5Center = new Microsoft.Msagl.Core.Geometry.Point(19, -3);
            Node n5 = new Node( CurveFactory.CreateEllipse(10, 10, n5Center),"f");
            n5.Center = n5Center;

            IncrementalLabeler il = new IncrementalLabeler(6, false, -1);
#if MYDEBUG
            Microsoft.Msagl.GraphViewerGdi.DisplayGeometryGraph.SetShowFunctions();
#endif

            il.AddNode(n0);
            il.AddNode(n1);
            il.Layout();
            il.AddNode(n2);
            il.AddNode(n3);
            il.Layout();
            GeometryGraph graph = LocationLabeler.PositionLabels(new[] { n0, n1, n2, n3, n4, n5 }, 5, false, -1);

//           var g=GeometryGraph.CreateFromFile("c:/tmp/graph");
           // ChangeShapes(g);
  //          var graph = LocationLabeler.PositionLabels(  g.Nodes , 6, false, -6);

        }

        private void ChangeShapes(GeometryGraph g) {
            foreach(var n in g.Nodes){
                var box=n.BoundaryCurve.BoundingBox;
                n.BoundaryCurve = new Ellipse(20, 20, n.Center);
            }
        }
    }
}
