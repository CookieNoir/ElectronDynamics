using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;

namespace ElectronDynamics.Controllers
{
    public class DoubleVariableController : MonoBehaviour
    {
        [SerializeField] private double _startValue;
        [SerializeField] private double _defaultValue;
        [field: SerializeField] public UnityEvent<double> OnValueChanged { get; private set; }
        private static readonly IFormatProvider _formatProvider = CultureInfo.InvariantCulture.NumberFormat;
        private double _value;
        private bool _isAnyValueSet;
        public double Value
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
            if (double.TryParse(value, NumberStyles.Number, _formatProvider, out double parsedValue))
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
