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
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Microsoft.Msagl.Core.Geometry
{
    /// <summary>
    /// Greedily pack rectangles (without rotation) into a given aspect ratio
    /// </summary>
    public class RectanglePacking<TData> : Packing
    {
        private IEnumerable<RectangleToPack<TData>> rectanglesByDescendingHeight;

        private double wrapWidth;

        /// <summary>
        /// Constructor for packing, call Run to do the actual pack.
        /// Each RectangleToPack.Rectangle is updated in place.
        /// Pack rectangles tallest to shortest, left to right until wrapWidth is reached, 
        /// then wrap to right-most rectangle still with vertical space to fit the next rectangle
        /// </summary>
        /// <param name="rectangles"></param>
        /// <param name="wrapWidth"></param>
        /// <param name="rectanglesPresorted">If the rectangles are already sorted into the order to pack, then specify true</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public RectanglePacking(IEnumerable<RectangleToPack<TData>> rectangles, double wrapWidth, bool rectanglesPresorted = false)
        {
            this.rectanglesByDescendingHeight = rectanglesPresorted ? rectangles : SortRectangles(rectangles);
            this.wrapWidth = wrapWidth;
        }
        /// <summary>
        /// Sort rectangles by height
        /// </summary>
        /// <param name="rectangles"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static IEnumerable<RectangleToPack<TData>> SortRectangles(IEnumerable<RectangleToPack<TData>> rectangles)
        {
            return from r in rectangles orderby r.Rectangle.Height descending, r.Rectangle.Width select r;
        }

        /// <summary>
        /// Pack rectangles tallest to shortest, left to right until wrapWidth is reached, 
        /// then wrap to right-most rectangle still with vertical space to fit the next rectangle
        /// </summary>
        protected override void RunInternal()
        {
            this.Pack(rectanglesByDescendingHeight.GetEnumerator());
        }

        /// <summary>
        /// Traverses the rectangleEnumerator and places rectangles at the next available slot beneath the current parent,
        /// until the parent is filled or until maxRowWidth is reached.  Each successfully placed rectangle is pushed onto
        /// a stack, when there is no room for the rectangle we pop the stack for a new parent and try again.
        /// </summary>
        /// <param name="rectangleEnumerator">rectangles to pack</param>
        private void Pack(IEnumerator<RectangleToPack<TData>> rectangleEnumerator)
        {
            PackedHeight = PackedWidth = 0;

            // get next rectangle
            var stack = new Stack<RectangleToPack<TData>>();
            bool wrap = false;
            double verticalPosition = 0;
            double packedWidth = 0;
            double packedHeight = 0;
            while (wrap || rectangleEnumerator.MoveNext())
            {
                var current = rectangleEnumerator.Current;
                var parent = stack.Count > 0 ? stack.Peek() : null;
                Rectangle r = current.Rectangle;
                if (parent == null ||
                    parent.Rectangle.Right + r.Width <= wrapWidth &&
                    verticalPosition + r.Height <= parent.Rectangle.Top)
                {
                    r = current.Rectangle = new Rectangle(
                        parent == null ? 0 : parent.Rectangle.Right,
                        verticalPosition,
                        new Point(r.Width, r.Height));
                    packedWidth = Math.Max(packedWidth, r.Right);
                    packedHeight = Math.Max(packedHeight, r.Top);
                    stack.Push(current);
                    wrap = false;
                }
                else
                {
                    verticalPosition = parent.Rectangle.Top;
                    stack.Pop();
                    wrap = true;
                }
            }
            this.PackedWidth = packedWidth;
            this.PackedHeight = packedHeight;
        }
    }
}
