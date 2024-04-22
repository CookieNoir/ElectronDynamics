using UnityEngine;
using UnityEngine.Events;

namespace ElectronDynamics.Input
{
    public class MouseInputConfig : MonoBehaviour
    {
        [field: SerializeField] public MouseButtonProperties LeftMouseButton { get; private set; }
        [field: SerializeField] public MouseButtonProperties RightMouseButton { get; private set; }
        [field: SerializeField] public MouseButtonProperties MiddleMouseButton { get; private set; }
        [field: SerializeField] public UnityEvent<float> OnMouseScrollDeltaChanged { get; private set; }
    }
}
