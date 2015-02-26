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
using System.Diagnostics;
using System.Linq;

using Microsoft.Msagl.Core;
using Microsoft.Msagl.Core.Geometry;
using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Core.Layout;

namespace Microsoft.Msagl.Layout.MDS
{
    /// <summary>
    /// Initial layout using PivotMDS method for FastIncrementalLayout
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "MDS")]
    public class PivotMDS : AlgorithmBase
    {
        class PivotMDSNodeWrap
        {
            internal Node node;
            internal PivotMDSNodeWrap(Node node)
            {
                this.node = node;
            }
        }

        private GeometryGraph graph;

        /// <summary>
        /// scales the final layout by the specified factor
        /// </summary>
        public double Scale { get; set; }

        /// <summary>
        /// Layout graph by the PivotMds method.  Uses spectral techniques to obtain a layout in
        /// O(n^2) time.
        /// </summary>
        /// <param name="graph"></param>
        public PivotMDS(GeometryGraph graph)
        {
            this.graph = graph;
            this.Scale = 1;
        }

        /// <summary>
        /// Executes the algorithm.
        /// </summary>
        /// <summary>
        /// Executes the actual algorithm.
        /// </summary>
        protected override void RunInternal()
        {
            var g = new GeometryGraph();
            foreach (var v in graph.Nodes)
            {
                Debug.Assert(!(v is Cluster));
                var u = new Node(v.BoundaryCurve.Clone())
                {
                    UserData = v
                };
                v.AlgorithmData = new PivotMDSNodeWrap(u);
                g.Nodes.Add(u);
            }
            double avgLength = 0;
            foreach (var e in graph.Edges)
            {
                avgLength += e.Length;
                if (e.Source is Cluster || e.Target is Cluster) continue;
                var u = e.Source.AlgorithmData as PivotMDSNodeWrap;
                var v = e.Target.AlgorithmData as PivotMDSNodeWrap;
                var ee = new Edge(u.node, v.node)
                {
                    Length = e.Length
                };
                g.Edges.Add(ee);
            }
            if (graph.Edges.Count != 0)
            {
                avgLength /= graph.Edges.Count;
            }
            else
            {
                avgLength = 100;
            }

            // create edges from the children of each parent cluster to the parent cluster node
            foreach (var c in graph.RootCluster.AllClustersDepthFirst())
            {
                if (c == graph.RootCluster) continue;

                var u = new Node(CurveFactory.CreateRectangle(10, 10, new Point()));
                u.UserData = c;
                c.AlgorithmData = new PivotMDSNodeWrap(u);
                g.Nodes.Add(u);
                    
                foreach (var v in c.Nodes.Concat(from cc in c.Clusters select (Node)cc))
                {
                    var vv = v.AlgorithmData as PivotMDSNodeWrap;
                    g.Edges.Add(new Edge(u, vv.node)
                    {
                        Length = avgLength
                    });
                }
            }

            // create edges between clusters
            foreach (var e in graph.Edges)
            {
                if (e.Source is Cluster || e.Target is Cluster)
                {
                    var u = e.Source.AlgorithmData as PivotMDSNodeWrap;
                    var v = e.Target.AlgorithmData as PivotMDSNodeWrap;
                    var ee = new Edge(u.node, v.node)
                    {
                        Length = e.Length
                    };
                    g.Edges.Add(ee);
                }
            }

            // with 0 majorization iterations we just do PivotMDS
            MdsLayoutSettings settings = new MdsLayoutSettings
            {
                ScaleX = this.Scale,
                ScaleY = this.Scale,
                IterationsWithMajorization = 0,
                RemoveOverlaps = false,
                AdjustScale = false
            };

            MdsGraphLayout mdsLayout = new MdsGraphLayout(settings, g);
            this.RunChildAlgorithm(mdsLayout, 1.0);

            g.UpdateBoundingBox();
            foreach (var v in graph.Nodes)
            {
                var m = v.AlgorithmData as PivotMDSNodeWrap;
                v.Center = m.node.Center;
            }
        }
    }
}
