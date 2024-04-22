using UnityEngine;

namespace ElectronDynamics.ViewportNavigation
{
    public class PanMovement : MonoBehaviour
    {
        [SerializeField] private TransformMovement _transformMovement;
        [SerializeField, Min(0f)] private float _movementSpeed = 1f;

        public void Move(Vector2 vector)
        {
            if (_transformMovement == null)
            {
                return;
            }
            _transformMovement.MoveXY(-_movementSpeed * vector);
        }
    }
}
