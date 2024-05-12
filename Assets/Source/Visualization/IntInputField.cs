using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace ElectronDynamics.Visualization
{
    public class IntInputField : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _inputField;
        [field: SerializeField] public UnityEvent<int> OnValueParsed { get; private set; }
        [field: SerializeField] public UnityEvent OnParsingFailed { get; private set; }
        private static readonly IFormatProvider _formatProvider = CultureInfo.InvariantCulture.NumberFormat;

        public void SetTextWithoutNotify(int value)
        {
            _inputField.SetTextWithoutNotify(value.ToString());
        }

        public void ParseValue(string text)
        {
            if (int.TryParse(text, NumberStyles.Number, _formatProvider, out int parsedValue))
            {
                OnValueParsed.Invoke(parsedValue);
            }
            else
            {
                OnParsingFailed.Invoke();
            }
        }
    }
}
