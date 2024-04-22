using System;

namespace ElectronDynamics.Task
{
    internal static class Constraints
    {
        public static readonly double QCharge = -1.6 * Math.Pow(10.0, -19.0);
        public static readonly double KBoltzman = 1.38 * Math.Pow(10.0, -23.0);
        public static readonly double K0_Kulon = 8.99 * Math.Pow(10.0, 9.0);
        public static readonly double ElectronMass = 9.11 * Math.Pow(10.0, -31.0);
    }
}
