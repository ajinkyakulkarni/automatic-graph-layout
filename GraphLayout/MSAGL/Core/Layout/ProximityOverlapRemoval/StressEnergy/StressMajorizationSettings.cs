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
using System.Text;

namespace Microsoft.Msagl.Core.Layout.ProximityOverlapRemoval.StressEnergy {
    /// <summary>
    /// Stress Majorization Settings.
    /// </summary>
    public class StressMajorizationSettings {

        int maxStressIterations = 31;
        SolvingMethod solvingMethod = SolvingMethod.PrecondConjugateGradient;
        UpdateMethod updateMethod = UpdateMethod.Parallel;
        double stressChangeTolerance = 0.01;
        bool cancelOnStressConvergence = true;
        bool cancelOnStressMaxIteration = true;
        
        //relevant for conjugate gradient methods only
        private int maxSolverIterations = 100;
        private MaxIterationMethod solverMaxIteratMethod=MaxIterationMethod.SqrtProblemSize;
        double residualTolerance = 0.01;
        bool cancelAfterFirstConjugate = true;

        private int parallelDegree = 4;
        private bool parallelize = false;

        /// <summary>
        /// Update Scheme for node positions. Only has an effect if the SolvingMethod has <value>Localized</value>.
        /// </summary>
        public UpdateMethod UpdateMethod {
            get { return updateMethod; }
            set { updateMethod = value; }
        }

        /// <summary>
        /// Method with which the Stress should be minimized. 
        /// </summary>
        public SolvingMethod SolvingMethod {
            get { return solvingMethod; }
            set { solvingMethod = value; }
        }

        /// <summary>
        /// Maximal number of iterations.
        /// </summary>
        public int MaxStressIterations {
            get { return maxStressIterations; }
            set { maxStressIterations = value; }
        }

        /// <summary>
        /// (stress(X(t))-stress(X(t+1)))/stress(X(t)), where X(t) (X(t+1)) are the node positions at iteration t (t+1). 
        /// When this value is small enough the layout process has converged (node positions will change only little in next iteration).
        /// </summary>
        public double StressChangeTolerance {
            get { return stressChangeTolerance; }
            set { stressChangeTolerance = value; }
        }

        /// <summary>
        /// if true: the StressMajorization process should be stopped when stress change is below the stressChangeTolerance
        /// </summary>
        public bool CancelOnStressConvergence {
            get { return cancelOnStressConvergence; }
            set { cancelOnStressConvergence = value; }
        }

        /// <summary>
        /// if true: the Stress Majorization process is canceled after the maximal number of iterations; 
        /// </summary>
        public bool CancelOnStressMaxIteration {
            get { return cancelOnStressMaxIteration; }
            set { cancelOnStressMaxIteration = value; }
        }

        /// <summary>
        /// Convergence Tolerance for the SolvingMethods. Has only effect on Conjugate Gradient methods.
        /// </summary>
        public double ResidualTolerance {
            get { return residualTolerance; }
            set { residualTolerance = value; }
        }

        /// <summary>
        /// Cancels the process after one iteration, when Conjugate Cradient as SolvingMethod is used.
        /// This is only suggested for OverlapRemoval and not for general graph layouting.
        /// </summary>
        public bool CancelAfterFirstConjugate {
            get { return cancelAfterFirstConjugate; }
            set { cancelAfterFirstConjugate = value; }
        }


        /// <summary>
        /// Method with which the maximal number of iterations is determined for the stress solver. Only relevant for conjugate gradient methods.
        /// </summary>
        public MaxIterationMethod SolverMaxIteratMethod {
            get { return solverMaxIteratMethod; }
            set { solverMaxIteratMethod = value;}
        }

        /// <summary>
        /// Maximal number of iterations for the solver used to minimize the stress. Only relevant for conjugate gradient methods.
        /// </summary>
        public int MaxSolverIterations {
            get { return maxSolverIterations; }
            set { maxSolverIterations = value; }
        }

        /// <summary>
        /// If true: Parallelization is used where possible.
        /// </summary>
        public bool Parallelize {
            get { return parallelize; }
            set { parallelize = value; }
        }

        /// <summary>
        /// Degree of parallelization: Number of Threads allowed to run in parallel.
        /// </summary>
        public int ParallelDegree {
            get { return parallelDegree; }
            set { parallelDegree = value; }
        }
    }
}
