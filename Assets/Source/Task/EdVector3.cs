using System;
using System.Globalization;

namespace ElectronDynamics.Task
{
    public struct EdVector3 : IEquatable<EdVector3>
    {
        public double X;
        public double Y;
        public double Z;

        public EdVector3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static EdVector3 Zero => new EdVector3(0.0, 0.0, 0.0);

        public override bool Equals(object obj)
        {
            return obj is EdVector3 vector &&
                   X == vector.X &&
                   Y == vector.Y &&
                   Z == vector.Z;
        }

        public bool Equals(EdVector3 other)
        {
            return X == other.X &&
                   Y == other.Y &&
                   Z == other.Z;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }

        public static bool operator ==(EdVector3 left, EdVector3 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EdVector3 left, EdVector3 right)
        {
            return !left.Equals(right);
        }

        public static EdVector3 operator +(EdVector3 left, EdVector3 right)
        {
            return new EdVector3(
                left.X + right.X,
                left.Y + right.Y,
                left.Z + right.Z
            );
        }

        public static EdVector3 operator -(EdVector3 left, EdVector3 right)
        {
            return new EdVector3(
                left.X - right.X,
                left.Y - right.Y,
                left.Z - right.Z
            );
        }

        public static EdVector3 operator *(double number, EdVector3 vector)
        {
            return new EdVector3(
                number * vector.X,
                number * vector.Y,
                number * vector.Z
            );
        }

        public static EdVector3 operator *(EdVector3 left, EdVector3 right)
        {
            return new EdVector3(
                left.X * right.X,
                left.Y * right.Y,
                left.Z * right.Z
            );
        }

        public double Magnitude()
        {
            return Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        public EdVector3 Multiply(EdVector3 other)
        {
            double x = Y * other.Z - Z * other.Y;
            double y = Z * other.X - X * other.Z;
            double z = X * other.Y - Y * other.X;
            return new EdVector3(x, y, z);
        }

        public override string ToString()
        {
            var formatProvider = CultureInfo.InvariantCulture.NumberFormat;
            return string.Format("({0}, {1}, {2}", X.ToString(formatProvider), Y.ToString(formatProvider), Z.ToString(formatProvider));
        }
    }
}
