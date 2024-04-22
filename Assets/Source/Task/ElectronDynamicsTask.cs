using System;
using System.Collections.Generic;
using System.Threading;

namespace ElectronDynamics.Task
{
    public class ElectronDynamicsTask
    {
        private readonly bool _saveIntermediateResults;
        private readonly EdVariables _variables;
        private CancellationToken _cancellationToken;
        private int _iterationsStep;
        private Action<Sample[]> _onIntermediateResultReceived;
        private readonly Random _random;
        private EdVector3[] _aKls;

        public ElectronDynamicsTask(EdVariables variables,
            CancellationToken cancellationToken,
            bool saveIntermediateResults = true,
            int iterationsStep = 0,
            Action<Sample[]> onIntermediateResultReceived = null,
            int randomSeed = 12345)
        {
            _variables = variables;
            _cancellationToken = cancellationToken;
            _saveIntermediateResults = saveIntermediateResults;
            _iterationsStep = iterationsStep;
            _onIntermediateResultReceived = onIntermediateResultReceived;
            _random = new Random(randomSeed);
        }

        public EdTaskResult Execute()
        {
            int Ne = _variables.Ne;
            Generate(out var samples, out var masses, out var charges);
            _aKls = new EdVector3[Ne * (Ne - 1) / 2];
            int iterationsCount = _variables.Tn - 1;
            int iterationsPassed = 0;
            var allSamples = new List<Sample[]> { samples };
            var lastSamples = samples;
            for (int i = 0; i < iterationsCount; ++i)
            {
                if (_cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                lastSamples = Iterate(lastSamples, masses, charges);
                iterationsPassed = i + 1;
                if (iterationsPassed < iterationsCount)
                {
                    if (_saveIntermediateResults)
                    {
                        allSamples.Add(lastSamples);
                    }
                    SendIntermediateResult(lastSamples, iterationsPassed);
                }
            }
            allSamples.Add(lastSamples);
            return new EdTaskResult(_variables, masses, charges, _saveIntermediateResults, iterationsPassed, allSamples);
        }

        private void SendIntermediateResult(Sample[] result, int iterationsPassed)
        {
            if (_iterationsStep <= 0)
            {
                return;
            }
            if (iterationsPassed % _iterationsStep == 0)
            {
                _onIntermediateResultReceived?.Invoke(result);
            }
        }

        private void Generate(out Sample[] samples, out double[] masses, out double[] charges)
        {
            int Ne = _variables.Ne;
            samples = new Sample[Ne];
            masses = new double[Ne];
            charges = new double[Ne];
            int last = Ne - 1;
            for (int i = 0; i < last; ++i)
            {
                masses[i] = Constraints.ElectronMass;
                charges[i] = Constraints.QCharge;
            }
            masses[last] = _variables.DropletMass;
            charges[last] = last * Constraints.QCharge;
            double min = -_variables.MagnetRadius;
            double max = _variables.MagnetRadius;
            for (int i = 0; i < Ne; ++i)
            {
                var position = new EdVector3(GetRandomValue(min, max), GetRandomValue(min, max), GetRandomValue(min, max));
                var velocity = GetV(masses[i]);
                samples[i] = new Sample(position, velocity);
            }
            var lastPosition = new EdVector3(0.0, 0.0, samples[last].Position.Z);
            samples[last] = new Sample(lastPosition, samples[last].Velocity);
        }

        private EdVector3 GetLorentzForce(EdVector3 velocity, EdVector3 B, double mass, double charge)
        {
            double module = velocity.Magnitude();
            EdVector3 acc = velocity.Multiply(B);
            EdVector3 newVelocity = velocity + (_variables.h * charge / mass) * acc;
            if (module > 0)
            {
                newVelocity = module / newVelocity.Magnitude() * newVelocity;
            }
            else
            {
                newVelocity = EdVector3.Zero;
            }
            return newVelocity;
        }

        private EdVector3 GetVectorB(EdVector3 position)
        {
            double z0 = _variables.MagnetPositionZ;
            double multiplier = Math.Exp(Math.Abs(position.Z - z0) / z0) - 1.0;
            EdVector3 b = new EdVector3(multiplier * position.X, multiplier * position.Y, position.Z - z0);
            double bNorm = b.Magnitude();
            double coefB = _variables.MagnetBMax * Math.Exp(-(position.Z - z0) / z0) / bNorm;
            return coefB * b;
        }

        private double GetRandomValue(double min, double max)
        {
            return min + _random.NextDouble() * (max - min);
        }

        private EdVector3 GetV(double mass)
        {
            EdVector3 v = new EdVector3(GetRandomValue(-1.0, 1.0), GetRandomValue(-1.0, 1.0), GetRandomValue(-1.0, 1.0));
            double thermalV = Math.Sqrt(3.0 * Constraints.KBoltzman * _variables.Temperature / mass) / v.Magnitude();
            return thermalV * v + new EdVector3(0.0, 0.0, _variables.FlowVT);
        }

        private int GetAKlIndex(int Ne, int min, int max)
        {
            return Ne * min + max - 1 - min * (min + 3) / 2;
        }

        private Sample[] Iterate(Sample[] samples, double[] masses, double[] charges)
        {
            int Ne = _variables.Ne;
            double h = _variables.h;
            var result = new Sample[Ne];
            for (int i = 0; i < Ne; ++i)
            {
                var iSample = samples[i];
                double iMass = masses[i];
                double iCharge = charges[i];

                double scatterChance = _random.NextDouble();
                EdVector3 v = scatterChance >= _variables.ScatterEffect ? GetV(iMass) : iSample.Velocity;
                EdVector3 B = GetVectorB(iSample.Position);
                v = GetLorentzForce(v, B, iMass, iCharge);
                EdVector3 aKl = EdVector3.Zero;
                for (int j = 0; j < i; ++j)
                {
                    aKl -= _aKls[GetAKlIndex(Ne, j, i)];
                }
                for (int j = i + 1; j < Ne; ++j)
                {
                    var jSample = samples[j];
                    double jCharge = charges[j];
                    EdVector3 dif = iSample.Position - jSample.Position;
                    double normDif = dif.Magnitude();
                    double denominator = normDif * normDif * normDif * iMass;
                    double coefAKl = Constraints.K0_Kulon * iCharge * jCharge / denominator;
                    EdVector3 curAKl = coefAKl * dif;
                    _aKls[GetAKlIndex(Ne, i, j)] = curAKl;
                    aKl += curAKl;
                }
                v += h * aKl;

                var newPosition = iSample.Position + h * v;
                var newVelocity = v;
                result[i] = new Sample(newPosition, newVelocity);
            }
            return result;
        }
    }
}
