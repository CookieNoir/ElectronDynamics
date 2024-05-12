using System;

namespace ElectronDynamics.Task
{
    public class EdVariables
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

        public EdVariables(double flowVT, double magnetRadius, double magnetPositionZ, double magnetBMax, double temperature, double dropletMass, 
            int Ne, double T, double h, double t0)
        {
            FlowVT = flowVT;
            MagnetRadius = magnetRadius;
            MagnetPositionZ = magnetPositionZ;
            MagnetBMax = magnetBMax;
            Temperature = temperature;
            DropletMass = dropletMass;
            this.Ne = Ne;
            this.T = T;
            this.h = h;
            this.t0 = t0;
            Tn = (int)Math.Round(T / h);
            ScatterEffect = 1.0 - h / t0;
        }

        public static EdVariables Create(int Ne, double T, double h, double t0)
        {
            double flowVT = 1412.0;
            double magnetRadius = 0.0001;
            double magnetPositionZ = 0.5;
            double magnetBMax = 0.5;
            double temperature = 1000;
            double dropletMass = 2.8 * Math.Pow(10.0, -15.0);
            return new EdVariables(flowVT, magnetRadius, magnetPositionZ, magnetBMax, temperature, dropletMass, Ne, T, h, t0);
        }
    }
}
