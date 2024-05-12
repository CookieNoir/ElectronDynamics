using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace ElectronDynamics.Visualization
{
    public class DoubleInputField : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _inputField;
        [field: SerializeField] public UnityEvent<double> OnValueParsed { get; private set; }
        [field: SerializeField] public UnityEvent OnParsingFailed { get; private set; }
        private static readonly IFormatProvider _formatProvider = CultureInfo.InvariantCulture.NumberFormat;

        public void SetTextWithoutNotify(double value)
        {
            _inputField.SetTextWithoutNotify(value.ToString(_formatProvider));
        }

        public void ParseValue(string text)
        {
            if (double.TryParse(text, NumberStyles.Number, _formatProvider, out double parsedValue))
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
