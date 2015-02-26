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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Msagl.Core.Geometry;
using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Routing.ConstrainedDelaunayTriangulation;

namespace Microsoft.Msagl.Routing
{
    ///<summary>
    ///</summary>
    [DebuggerDisplay("{Point}")]
#if TEST_MSAGL
    [Serializable]
#endif
    public class CdtSite
    {
        /// <summary>
        /// Object to which this site refers to.
        /// </summary>
        public object Owner { get; set; }

        internal IEnumerable<CdtEdge> AllEdges
        {
            get
            {
                if (Edges != null)
                    foreach (var e in Edges)
                        yield return e;
                if (InEdges != null)
                    foreach (var e in InEdges)
                        yield return e;
            }
        }

        ///<summary>
        ///</summary>
        public Point Point;
        /// <summary>
        /// each CdtSite points to the edges for which it is the upper virtex ( for horizontal edges it is the left point)
        /// </summary>
        public List<CdtEdge> Edges;

        internal List<CdtEdge> InEdges;

        ///<summary>
        ///</summary>
        ///<param name="isolatedSite"></param>
        public CdtSite(Point isolatedSite)
        {
            Point = isolatedSite;
        }

        internal void AddEdgeToSite(CdtEdge edge)
        {
            if (Edges == null)
                Edges = new List<CdtEdge>();
            Edges.Add(edge);
        }
#if DEBUG && TEST_MSAGL
        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return Point.ToString();
        }
#endif

        internal CdtEdge EdgeBetweenUpperSiteAndLowerSite(CdtSite b)
        {
            Debug.Assert(Cdt.Above(this, b) > 0);
            if (Edges != null)
                foreach (var edge in Edges)
                    if (edge.lowerSite == b)
                        return edge;
            return null;
        }

        internal void AddInEdge(CdtEdge e)
        {
            if (InEdges == null)
                InEdges = new List<CdtEdge>();
            InEdges.Add(e);
        }
        /// <summary>
        /// enumerates over all site triangles
        /// </summary>
        internal IEnumerable<CdtTriangle> Triangles
        {
            // this function might not work correctly if InEdges are not set
            get
            {
                CdtEdge edge;
                if (Edges != null && Edges.Count>0)
                    edge = Edges[0];
                else if (InEdges != null && InEdges.Count>0)
                    edge = InEdges[0];
                else yield break;

                //going counterclockwise around the site
                var e = edge;
                do
                {
                    var t = e.upperSite == this ? e.CcwTriangle : e.CwTriangle;
                    if (t == null)
                    {
                        e = null;
                        break;
                    }
                    yield return t;
                    e = t.Edges[t.Edges.Index(e) + 2];
                } while (e != edge);//full circle

                if (e != edge)
                { //we have not done the full circle, starting again with edge but now going clockwise around the site
                    e = edge;
                    do
                    { 
                        var t = e.upperSite == this ? e.CwTriangle : e.CcwTriangle;
                        if (t == null)
                        {
                            break;
                        }
                        yield return t;
                        e = t.Edges[t.Edges.Index(e) + 1];
                    } while (true); // we will hit a null triangle for the convex hull border edge
                }
            }
            
        }
    }
}
