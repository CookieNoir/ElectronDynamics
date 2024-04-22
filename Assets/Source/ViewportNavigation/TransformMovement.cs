using UnityEngine;

namespace ElectronDynamics.ViewportNavigation
{
    public class TransformMovement : MonoBehaviour
    {
        [SerializeField] private Transform _targetTransform;
        [SerializeField, Min(0f)] private float _linearSpeed = 1f;
        [SerializeField, Min(0f)] private float _angularSpeed = 120f;
        [SerializeField] private Vector3 _positionLowerBounds = -Vector3.one;
        [SerializeField] private Vector3 _positionUpperBounds = 2f * Vector3.one;
        private Vector3 _positionOffset;
        private Vector2 _rotationOffset;

        private void OnEnable()
        {
            if (_targetTransform == null)
            {
                Debug.LogWarning($"{gameObject.name}: Target transform is null, component will be disabled", gameObject);
                enabled = false;
                return;
            }
            DiscardOffsets();
        }

        private void AddPositionOffset(Vector3 vector)
        {
            _positionOffset += vector;
        }

        public void MoveXY(Vector2 vector)
        {
            AddPositionOffset(vector);
        }

        public void MoveZ(float speed)
        {
            AddPositionOffset(speed * Vector3.forward);
        }

        private void AddRotationOffset(Vector2 vector)
        {
            _rotationOffset += vector;
        }

        public void RotateXY(Vector2 vector)
        {
            AddRotationOffset(vector);
        }

        public void MoveForward()
        {
            AddPositionOffset(Vector3.forward);
        }

        public void MoveBackward()
        {
            AddPositionOffset(Vector3.back);
        }

        public void MoveLeft()
        {
            AddPositionOffset(Vector3.left);
        }

        public void MoveRight()
        {
            AddPositionOffset(Vector3.right);
        }

        public void MoveUp()
        {
            AddPositionOffset(Vector3.up);
        }

        public void MoveDown()
        {
            AddPositionOffset(Vector3.down);
        }

        private void MoveLocalSpace(float deltaTime)
        {
            if (_positionOffset == Vector3.zero)
            {
                return;
            }
            float speed = deltaTime * _linearSpeed;
            _targetTransform.Translate(speed * _positionOffset);
            Vector3 localPosition = _targetTransform.localPosition;
            float x = Mathf.Clamp(localPosition.x, _positionLowerBounds.x, _positionUpperBounds.x);
            float y = Mathf.Clamp(localPosition.y, _positionLowerBounds.y, _positionUpperBounds.y);
            float z = Mathf.Clamp(localPosition.z, _positionLowerBounds.z, _positionUpperBounds.z);
            _targetTransform.localPosition = new Vector3(x, y, z);
            _positionOffset = Vector3.zero;
        }

        private void RotateLocalSpace(float deltaTime)
        {
            if (_rotationOffset == Vector2.zero)
            {
                return;
            }
            float speed = deltaTime * _angularSpeed;
            float deltaX = speed * _rotationOffset.x;
            float deltaY = speed * _rotationOffset.y;
            Quaternion rotation = _targetTransform.localRotation * Quaternion.Euler(-deltaY, 0f, 0f);
            rotation *= Quaternion.Inverse(rotation) * Quaternion.Euler(0f, deltaX, 0f) * rotation;
            _targetTransform.localRotation = rotation;
            _rotationOffset = Vector2.zero;
        }

        private void DiscardOffsets()
        {
            _positionOffset = Vector3.zero;
            _rotationOffset = Vector2.zero;
        }

        private void MoveAndRotate(float deltaTime)
        {
            MoveLocalSpace(deltaTime);
            RotateLocalSpace(deltaTime);
        }

        private void LateUpdate()
        {
            MoveAndRotate(Time.deltaTime);
        }
    }
}
