using System;

namespace ElectronDynamics.Task
{
    public readonly struct EdVariables
    {
        public readonly double FlowVT;
        public readonly double MagnetRadius;
        public readonly double MagnetPositionZ;
        public readonly double MagnetBMax;
        public readonly double Temperature;
        public readonly double DropletMass;
        public readonly int Ne;
        public readonly double T;
        public readonly double h;
        public readonly double t0;
        public readonly int Tn;
        public readonly double ScatterEffect;

        public EdVariables(int Ne, double T, double h, double t0)
        {
            FlowVT = 1412.0;
            MagnetRadius = 0.0001;
            MagnetPositionZ = 0.5;
            MagnetBMax = 0.5;
            Temperature = 1000;
            DropletMass = 2.8 * Math.Pow(10.0, -15.0);
            this.Ne = Ne;
            this.T = T;
            this.h = h;
            this.t0 = t0;
            Tn = (int)Math.Round(T / h);
            ScatterEffect = 1.0 - h / t0;
        }
    }
}
