using UnityEngine;
using UnityInput = UnityEngine.Input;

namespace ElectronDynamics.Input
{
    public class MouseInput : MonoBehaviour
    {
        [SerializeField] private MouseInputConfig _mouseInputConfig;
        [SerializeField, Min(0f)] private float _mouseScrollDeltaThreshold = 0.001f;
        private Vector3 _previousMousePosition;

        private void Start()
        {
            _previousMousePosition = UnityInput.mousePosition;
        }

        public void SetMouseInputConfig(MouseInputConfig config)
        {
            UpAllButtons(UnityInput.mousePosition);
            _mouseInputConfig = config;
        }

        private void UpAllButtons(Vector3 currentMousePosition)
        {
            if (_mouseInputConfig == null)
            {
                return;
            }
            OnButtonUp(_mouseInputConfig.LeftMouseButton, currentMousePosition);
            OnButtonUp(_mouseInputConfig.RightMouseButton, currentMousePosition);
            OnButtonUp(_mouseInputConfig.MiddleMouseButton, currentMousePosition);
        }

        private void HandleMouseButton(int index, MouseButtonProperties mouseButton, Vector3 currentMousePosition, Vector2 difference)
        {
            Vector2 scaledDifference = new Vector2(difference.x / Screen.width, difference.y / Screen.height);
            if (InputHelpers.IsMouseOverGameWindow() && 
                !InputHelpers.IsMouseOverUI() && 
                UnityInput.GetMouseButtonDown(index))
            {
                OnButtonDown(mouseButton, currentMousePosition);
            }
            if (mouseButton.IsMouseButtonPressed)
            {
                mouseButton.OnButtonHold.Invoke(scaledDifference);
                mouseButton.OnButtonHoldUnscaled.Invoke(difference);
            }
            if (UnityInput.GetMouseButtonUp(index))
            {
                OnButtonUp(mouseButton, currentMousePosition);
            }
        }

        private void HandleMouseScroll()
        {
            float scrollDelta = UnityInput.mouseScrollDelta.y;
            if (!InputHelpers.IsMouseOverGameWindow() ||
                InputHelpers.IsMouseOverUI() ||
                Mathf.Abs(scrollDelta) < _mouseScrollDeltaThreshold)
            {
                return;
            }
            _mouseInputConfig.OnMouseScrollDeltaChanged.Invoke(scrollDelta);
        }

        private void OnButtonDown(MouseButtonProperties mouseButton, Vector3 currentMousePosition)
        {
            mouseButton.IsMouseButtonPressed = true;
            mouseButton.OnButtonDown.Invoke(currentMousePosition);
        }

        private void OnButtonUp(MouseButtonProperties mouseButton, Vector3 currentMousePosition)
        {
            if (!mouseButton.IsMouseButtonPressed)
            {
                return;
            }
            mouseButton.IsMouseButtonPressed = false;
            mouseButton.OnButtonUp.Invoke(currentMousePosition);
        }

        private void Update()
        {
            if (_mouseInputConfig == null)
            {
                return;
            }
            Vector3 currentMousePosition = UnityInput.mousePosition;
            Vector3 difference = currentMousePosition - _previousMousePosition;
            HandleMouseButton(0, _mouseInputConfig.LeftMouseButton, currentMousePosition, difference);
            HandleMouseButton(1, _mouseInputConfig.RightMouseButton, currentMousePosition, difference);
            HandleMouseButton(2, _mouseInputConfig.MiddleMouseButton, currentMousePosition, difference);
            HandleMouseScroll();
            _previousMousePosition = UnityInput.mousePosition;
        }

        private void OnDisable()
        {
            UpAllButtons(UnityInput.mousePosition);
        }
    }
}
