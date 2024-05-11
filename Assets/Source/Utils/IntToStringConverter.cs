using UnityEngine;
using UnityEngine.Events;

namespace ElectronDynamics.Utils
{
    public class IntToStringConverter : MonoBehaviour
    {
        [field: SerializeField] public UnityEvent<string> OnStringReceived { get; private set; }

        public void ConvertToString(int value)
        {
            OnStringReceived.Invoke(value.ToString());
        }
    }
}
