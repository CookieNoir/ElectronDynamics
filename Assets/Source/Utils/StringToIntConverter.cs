using System.Globalization;
using UnityEngine;
using UnityEngine.Events;

namespace ElectronDynamics.Utils
{
    public class StringToIntConverter : MonoBehaviour
    {
        [field: SerializeField] public UnityEvent<int> OnIntReceived { get; private set; }
        [field: SerializeField] public UnityEvent OnConversionFailed { get; private set; }

        public void ConvertToInt(string value)
        {
            if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result))
            {
                OnIntReceived.Invoke(result);
            }
            else
            {
                OnConversionFailed.Invoke();
            }
        }
    }
}
