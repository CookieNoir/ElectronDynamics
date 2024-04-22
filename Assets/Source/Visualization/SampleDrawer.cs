using UnityEngine;

namespace ElectronDynamics.Visualization
{
    internal class SampleDrawer : MonoBehaviour
    {
        [SerializeField] private Transform _velocityTransform;

        public void SetValues(Vector3 scaledPosition, Vector3 scaledVelocity)
        {
            SetPosition(scaledPosition);
            SetVelocity(scaledVelocity);
        }

        private void SetPosition(Vector3 scaledPosition)
        {
            transform.localPosition = scaledPosition;
        }

        private void SetVelocity(Vector3 scaledVelocity)
        {
            if (_velocityTransform == null)
            {
                return;
            }
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, scaledVelocity.normalized);
            _velocityTransform.localRotation = rotation;
        }

        public void SetVisible()
        {
            gameObject.SetActive(true);
        }

        public void SetHidden()
        {
            gameObject.SetActive(false);
        }
    }
}
