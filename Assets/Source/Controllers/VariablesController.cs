using System.Globalization;
using UnityEngine;
using ElectronDynamics.Task;
using System;
using ElectronDynamics.Visualization;

namespace ElectronDynamics.Controllers
{
    public class VariablesController : MonoBehaviour
    {
        [SerializeField] private string _startNe = "15";
        [SerializeField] private string _startT = "1000";
        [SerializeField] private string _start_h = "0.0000000001";
        [SerializeField] private string _start_t0 = "0.0000001";
        [SerializeField] private TextDrawer _Ne_TextDrawer;
        [SerializeField] private TextDrawer _T_TextDrawer;
        [SerializeField] private TextDrawer _h_TextDrawer;
        [SerializeField] private TextDrawer _t0_TextDrawer;
        private const int _validNe = 15;
        private const double _validT = 1000d;
        private const double _valid_h = 0.0000000001d;
        private const double _valid_t0 = 0.0000001d;
        private static readonly IFormatProvider _formatProvider = CultureInfo.InvariantCulture.NumberFormat;
        private string _Ne;
        private string _T;
        private string _h;
        private string _t0;

        private void Start()
        {
            SetNe(_startNe);
            SetT(_startT);
            Set_h(_start_h);
            Set_t0(_start_t0);
        }

        public void SetNe(string value)
        {
            if (int.TryParse(value, NumberStyles.Number, _formatProvider, out int parsedValue) &&
                parsedValue >= 0)
            {
                _Ne = value;
            }
            else
            {
                _Ne = _validNe.ToString();
            }
            _Ne_TextDrawer.DrawText(_Ne);
        }

        public void SetT(string value)
        {
            if (double.TryParse(value, NumberStyles.Number, _formatProvider, out double parsedValue))
            {
                _T = value;
            }
            else
            {
                _T = _validT.ToString();
            }
            _T_TextDrawer.DrawText(_T);
        }

        public void Set_h(string value)
        {
            if (double.TryParse(value, NumberStyles.Number, _formatProvider, out double parsedValue))
            {
                _h = value;
            }
            else
            {
                _h = _valid_h.ToString();
            }
            _h_TextDrawer.DrawText(_h);
        }

        public void Set_t0(string value)
        {
            if (double.TryParse(value, NumberStyles.Number, _formatProvider, out double parsedValue))
            {
                _t0 = value;
            }
            else
            {
                _t0 = _valid_t0.ToString();
            }
            _t0_TextDrawer.DrawText(_t0);
        }

        public EdVariables GetVariables()
        {
            try
            {
                int Ne = string.IsNullOrEmpty(_Ne) ? _validNe : int.Parse(_Ne, _formatProvider);
                double T = string.IsNullOrEmpty(_T) ? _validT : double.Parse(_T, _formatProvider);
                double H = string.IsNullOrEmpty(_h) ? _valid_h : double.Parse(_h, _formatProvider);
                double t0 = string.IsNullOrEmpty(_t0) ? _valid_t0 : double.Parse(_t0, _formatProvider);
                return new EdVariables(Ne, T, H, t0);
            }
            catch
            {
                Debug.LogError($"{gameObject.name}: Parsing failed, default values are used (" +
                    $"Ne: {_validNe}, " +
                    $"T: {_validT.ToString(_formatProvider)}, " +
                    $"h: {_valid_h.ToString(_formatProvider)}, " +
                    $"t0: {_valid_t0.ToString(_formatProvider)})", gameObject);
                return new EdVariables(_validNe, _validT, _valid_h, _valid_t0);
            }

        }
    }
}
