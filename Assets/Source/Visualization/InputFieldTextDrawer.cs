using TMPro;
using UnityEngine;

namespace ElectronDynamics.Visualization
{
    internal class InputFieldTextDrawer : TextDrawer
    {
        [SerializeField] private TMP_InputField _inputField;

        public override void DrawText(string text)
        {
            _inputField?.SetTextWithoutNotify(text);
        }
    }
}
