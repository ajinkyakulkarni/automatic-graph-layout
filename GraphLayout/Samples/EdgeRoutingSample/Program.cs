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
using System.Linq;
using Microsoft.Msagl.Core.Geometry;
using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Core.Layout;
using Microsoft.Msagl.Core.Routing;
using Microsoft.Msagl.DebugHelpers;
using Microsoft.Msagl.DebugHelpers.Persistence;
using Microsoft.Msagl.GraphViewerGdi;
using Microsoft.Msagl.Routing;
using Microsoft.Msagl.Routing.Rectilinear;
using Node = Microsoft.Msagl.Core.Layout.Node;

namespace EdgeRoutingSample {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
#if TEST
            DisplayGeometryGraph.SetShowFunctions();
#else 
            Console.WriteLine("run the Debug version to see the edge routes");
#endif
            var graph = GeometryGraphReader.CreateFromFile("channel.msagl.geom");
            foreach (var edge in graph.Edges) {
                if (edge.SourcePort == null)
                    edge.SourcePort = new FloatingPort(edge.Source.BoundaryCurve, edge.Source.Center);
                if (edge.TargetPort == null)
                    edge.TargetPort = new FloatingPort(edge.Target.BoundaryCurve, edge.Target.Center);
            }
         
            DemoEdgeRouterHelper(graph);
            
        }

        static void DemoEdgeRouterHelper(GeometryGraph graph) {

            DemoRoutingFromPortToPort(graph);

            var router = new SplineRouter(graph, 3, 3, Math.PI / 6, null);


            router.Run();
#if TEST
            LayoutAlgorithmSettings.ShowGraph(graph);
#endif

            var rectRouter = new RectilinearEdgeRouter(graph, 3,3, true);
            rectRouter.Run();
#if TEST
            LayoutAlgorithmSettings.ShowGraph(graph);
#endif
            
           
        }

        static void DemoRoutingFromPortToPort(GeometryGraph graph) {
            var edges = graph.Edges.ToArray();
            SetCurvesToNull(edges);
            var portRouter = new InteractiveEdgeRouter(graph.Nodes.Select(n => n.BoundaryCurve), 3, 0.65*3, 0);
            portRouter.Run(); //calculates the whole visibility graph, takes a long time
            DrawEdgeWithPort(edges[0], portRouter, 0.3, 0.4);
            DrawEdgeWithPort(edges[1], portRouter, 0.7, 1.5*Math.PI);
                //I know here that my node boundary curves are ellipses so the parameters run from 0 to 2Pi
            //otherwise the curve parameter runs from curve.ParStart, to curve.ParEnd

#if TEST
            LayoutAlgorithmSettings.ShowGraph(graph);
#endif
        }

        static void SetCurvesToNull(Edge[] edges) {
            foreach (var edge in edges)
                edge.Curve = null;
        }

        static void DrawEdgeWithPort(Edge edge, InteractiveEdgeRouter portRouter, double par0, double par1) {

            var port0 = new CurvePort(edge.Source.BoundaryCurve, par0);
            var port1 = new CurvePort(edge.Target.BoundaryCurve,par1);

            SmoothedPolyline sp;
            var spline = portRouter.RouteSplineFromPortToPortWhenTheWholeGraphIsReady(port0, port1, true, out sp);
            
            Arrowheads.TrimSplineAndCalculateArrowheads(edge.EdgeGeometry,
                                                         edge.Source.BoundaryCurve,
                                                         edge.Target.BoundaryCurve,
                                                         spline, true,
                                                         false);

        }


        private static IEnumerable<ICurve> ArrowHeadCurves(EdgeGeometry edgeGeom) {
            var start = edgeGeom.Curve.End;
            var end = edgeGeom.TargetArrowhead.TipPosition;
            var ang = Math.PI / 12;
            var leftTip = end + (start - end).Rotate(ang);
            var rightTip = end + (start - end).Rotate(-ang);
            return new List<ICurve> { new LineSegment(leftTip, end), new LineSegment(rightTip, end) };
        }
    }
}
