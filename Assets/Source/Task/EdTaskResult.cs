using System.Collections.Generic;

namespace ElectronDynamics.Task
{
    public class EdTaskResult
    {
        public readonly EdVariables Variables;
        public readonly double[] Masses;
        public readonly double[] Charges;
        public readonly bool ContainsIntermediateResults;
        public readonly int IterationsPassed;
        public readonly IReadOnlyList<Sample[]> Samples;

        public EdTaskResult(EdVariables variables,
            double[] masses,
            double[] charges,
            bool containsIntermediateResults,
            int iterationsPassed,
            IReadOnlyList<Sample[]> samples)
        {
            Variables = variables;
            Masses = masses;
            Charges = charges;
            ContainsIntermediateResults = containsIntermediateResults;
            IterationsPassed = iterationsPassed;
            Samples = samples;
        }
    }
}
