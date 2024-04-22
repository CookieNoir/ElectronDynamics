using UnityEngine;
using UnityEngine.EventSystems;
using UnityInput = UnityEngine.Input;

namespace ElectronDynamics.Input
{
    public static class InputHelpers
    {
        public static bool IsMouseOverGameWindow()
        {
            return UnityInput.mousePosition.x >= 0f &&
                   UnityInput.mousePosition.y >= 0f &&
                   UnityInput.mousePosition.x <= Screen.width &&
                   UnityInput.mousePosition.y <= Screen.height;
        }

        public static bool IsMouseOverUI()
        {
            return EventSystem.current != null &&
                   EventSystem.current.IsPointerOverGameObject();
        }
    }
}
