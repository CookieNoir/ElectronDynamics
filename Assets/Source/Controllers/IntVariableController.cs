using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;

namespace ElectronDynamics.Controllers
{
    public class IntVariableController : MonoBehaviour
    {
        [SerializeField] private int _startValue;
        [SerializeField] private int _defaultValue;
        [field: SerializeField] public UnityEvent<int> OnValueChanged { get; private set; }
        private static readonly IFormatProvider _formatProvider = CultureInfo.InvariantCulture.NumberFormat;
        private int _value;
        private bool _isAnyValueSet;
        public int Value
        {
            get
            {
                if (_isAnyValueSet)
                {
                    return _value;
                }
                return _startValue;
            }
            set
            {
                _value = value;
                _isAnyValueSet = true;
                OnValueChanged.Invoke(_value);
            }
        }

        private void Awake()
        {
            Value = _startValue;
        }

        public void SetValueFromString(string value)
        {
            if (int.TryParse(value, NumberStyles.Number, _formatProvider, out int parsedValue))
            {
                Value = parsedValue;
            }
            else
            {
                SetDefaultValue();
            }
        }

        public void SetDefaultValue()
        {
            Value = _defaultValue;
        }
    }
}
