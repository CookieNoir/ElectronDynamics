using UnityEngine;
using ElectronDynamics.Task;

namespace ElectronDynamics.Controllers
{
    public class VariablesController : MonoBehaviour
    {
        [SerializeField] private DoubleVariableController _flowVT;
        [SerializeField] private DoubleVariableController _magnetRadius;
        [SerializeField] private DoubleVariableController _magnetPositionZ;
        [SerializeField] private DoubleVariableController _magnetBMax;
        [SerializeField] private DoubleVariableController _temperature;
        [SerializeField] private DoubleVariableController _dropletMass;
        [SerializeField] private IntVariableController _Ne;
        [SerializeField] private DoubleVariableController _T;
        [SerializeField] private DoubleVariableController _h;
        [SerializeField] private DoubleVariableController _t0;
        private const int _validNe = 15;
        private const double _validT = 1000d;
        private const double _valid_h = 0.0000000001d;
        private const double _valid_t0 = 0.0000001d;

        public EdVariables Variables => new EdVariables(_flowVT.Value,
            _magnetRadius.Value,
            _magnetPositionZ.Value,
            _magnetBMax.Value,
            _temperature.Value,
            _dropletMass.Value,
            _Ne.Value,
            _T.Value,
            _h.Value,
            _t0.Value);
    }
}
