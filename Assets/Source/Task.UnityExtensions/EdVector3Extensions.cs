using UnityEngine;

namespace ElectronDynamics.Task.UnityExtensions
{
    public static class EdVector3Extensions
    {
        public static Vector3 ToVector3(this EdVector3 edVector3)
        {
            return new Vector3((float)edVector3.X, (float)edVector3.Y, (float)edVector3.Z);
        }
    }
}
