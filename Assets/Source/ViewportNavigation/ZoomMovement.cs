using UnityEngine;

namespace ElectronDynamics.ViewportNavigation
{
    public class ZoomMovement : MonoBehaviour
    {
        [SerializeField] private TransformMovement _transformMovement;
        [SerializeField] private float _mouseWheelScrollMultiplier = 1f;

        public void MoveUsingMouseWheelScroll(float value)
        {
            if (_transformMovement == null)
            {
                return;
            }
            float speed = _mouseWheelScrollMultiplier * value;
            _transformMovement.MoveZ(speed);
        }
    }
}
