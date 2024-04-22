using UnityEngine;
using UnityEngine.Events;

namespace ElectronDynamics.Input
{
    [System.Serializable]
    public class MouseButtonProperties
    {
        public bool IsMouseButtonPressed { get; set; }
        [field: SerializeField] public UnityEvent<Vector2> OnButtonDown { get; private set; }
        [field: SerializeField] public UnityEvent<Vector2> OnButtonHold { get; private set; }
        [field: SerializeField] public UnityEvent<Vector2> OnButtonHoldUnscaled { get; private set; }
        [field: SerializeField] public UnityEvent<Vector2> OnButtonUp { get; private set; }
    }
}
