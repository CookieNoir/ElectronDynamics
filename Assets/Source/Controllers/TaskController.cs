using ElectronDynamics.Task;
using ElectronDynamics.Visualization;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace ElectronDynamics.Controllers
{
    public class TaskController : MonoBehaviour
    {
        [SerializeField] private VariablesController _variablesController;
        [SerializeField] private SampleDrawerHandler _sampleDrawerHandler;
        [SerializeField, Min(0)] private int _iterationsStep = 1;
        [SerializeField] private int _randomSeed = 12345;
        private ConcurrentStack<Sample[]> _samples = new ConcurrentStack<Sample[]>();
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
            var task = new ElectronDynamicsTask(variables, _token, saveIntermediateResults, _iterationsStep, _samples.Push, _randomSeed);
            var result = await System.Threading.Tasks.Task.Run(task.Execute);
            _sampleDrawerHandler.DrawSamples(result.Samples.Last());
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
            if (_samples.TryPop(out var samples))
            {
                _sampleDrawerHandler.DrawSamples(samples);
                _samples.Clear();
            }
        }

        private void Dispose()
        {
            _tokenSource.Dispose();
            _samples.Clear();
        }

        private void OnDestroy()
        {
            StopTask();
            Dispose();
        }
    }
}
