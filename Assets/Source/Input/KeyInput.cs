using UnityEngine;
using UnityEngine.Events;
using UnityInput = UnityEngine.Input;

namespace ElectronDynamics.Input
{
    public class KeyInput : MonoBehaviour
    {
        [SerializeField] private KeyCode _keyCode;
        [SerializeField] private bool _ignoreWhenMouseIsOverUI;
        [field: SerializeField] public UnityEvent OnKeyDown { get; private set; }
        [field: SerializeField] public UnityEvent OnKeyHold { get; private set; }
        [field: SerializeField] public UnityEvent OnKeyUp { get; private set; }
        private bool _isKeyDown = false;

        private void Update()
        {
            if (InputHelpers.IsMouseOverGameWindow() &&
                !(_ignoreWhenMouseIsOverUI && InputHelpers.IsMouseOverUI()))
            {
                if (UnityInput.GetKeyDown(_keyCode))
                {
                    OnKeyDown.Invoke();
                    _isKeyDown = true;
                }
            }
            if (_isKeyDown && UnityInput.GetKey(_keyCode))
            {
                OnKeyHold.Invoke();
            }
            if (UnityInput.GetKeyUp(_keyCode))
            {
                OnKeyUp.Invoke();
                _isKeyDown = false;
            }
        }

        private void OnDisable()
        {
            _isKeyDown = false;
        }
    }
}
