using ElectronDynamics.Task;
using ElectronDynamics.Visualization;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

namespace ElectronDynamics.Controllers
{
    public class TaskController : MonoBehaviour
    {
        [SerializeField] private VariablesController _variablesController;
        [SerializeField] private SampleDrawerHandler _sampleDrawerHandler;
        [SerializeField, Min(0)] private int _iterationsStep = 1;
        [SerializeField] private int _randomSeed = 12345;
        [field: SerializeField] public UnityEvent OnTaskStarted { get; private set; }
        [field: SerializeField] public UnityEvent OnTaskEnded { get; private set; }
        private ConcurrentQueue<Sample[]> _samples = new ConcurrentQueue<Sample[]>();
        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private CancellationToken _token;

        public void StartTaskWithIntermediateResults()
        {
            StartTask(true);
        }

        public void StartTaskWithoutIntermediateResults()
        {
            StartTask(false);
        }

        private async void StartTask(bool saveIntermediateResults)
        {
            var variables = _variablesController.GetVariables();
            _token = _tokenSource.Token;
            var task = new ElectronDynamicsTask(variables, _token, saveIntermediateResults, _iterationsStep, _samples.Enqueue, _randomSeed);
            Debug.Log($"{gameObject.name}: Task started", gameObject);
            OnTaskStarted.Invoke();
            var result = await System.Threading.Tasks.Task.Run(task.Execute);
            Debug.Log($"{gameObject.name}: Task ended", gameObject);
            OnTaskEnded.Invoke();
            _samples.Enqueue(result.Samples.Last());
        }

        public void StopTask()
        {
            if (!_token.CanBeCanceled)
            {
                return;
            }
            _tokenSource.Cancel();
        }

        private void Update()
        {
            if (_samples.TryDequeue(out var samples))
            {
                _sampleDrawerHandler.DrawSamples(samples);
            }
        }

        private void Dispose()
        {
            _tokenSource.Dispose();
            _samples.Clear();
        }

        private void OnDisable()
        {
            _samples.Clear();
        }

        private void OnDestroy()
        {
            StopTask();
            Dispose();
        }
    }
}
