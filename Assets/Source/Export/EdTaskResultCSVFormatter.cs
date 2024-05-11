using ElectronDynamics.Task;
using System;
using System.Globalization;
using System.Text;

namespace ElectronDynamics.Export
{
    public static class EdTaskResultCSVFormatter
    {
        private static readonly IFormatProvider _formatProvider = CultureInfo.InvariantCulture;

        private static string Format(double value)
        {
            return value.ToString(_formatProvider);
        }

        private const string Header = "StartPosition.X,StartPosition.Y,StartPosition.Z," +
                                      "StartVelocity.X,StartVelocity.Y,StartVelocity.Z," +
                                      "EndPosition.X,EndPosition.Y,EndPosition.Z," +
                                      "EndVelocity.X,EndVelocity.Y,EndVelocity.Z";

        private static string SingleItemToCSV(EdVector3 startPosition, EdVector3 startVelocity, EdVector3 endPosition, EdVector3 endVelocity)
        {
            return $"{Format(startPosition.X)},{Format(startPosition.Y)},{Format(startPosition.Z)}," +
                   $"{Format(startVelocity.X)},{Format(startVelocity.Y)},{Format(startVelocity.Z)}," +
                   $"{Format(endPosition.X)},{Format(endPosition.Y)},{Format(endPosition.Z)}," +
                   $"{Format(endVelocity.X)},{Format(endVelocity.Y)},{Format(endVelocity.Z)}";
        }

        public static string ToCSV(this EdTaskResult edTaskResult)
        {
            if (edTaskResult == null)
            {
                return string.Empty;
            }
            var samples = edTaskResult.Samples;
            if (samples == null || samples.Count == 0)
            {
                return string.Empty;
            }
            Sample[] startSample = samples[0];
            Sample[] endSample = samples[^1];
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(Header);
            for (int i = 0; i < startSample.Length; ++i)
            {
                Sample startItem = startSample[i];
                Sample endItem = endSample[i];
                stringBuilder.AppendLine(SingleItemToCSV(startItem.Position, startItem.Velocity, endItem.Position, endItem.Velocity));
            }
            return stringBuilder.ToString();
        }
    }
}
