using ElectronDynamics.Task;
using ElectronDynamics.Visualization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        [field: SerializeField] public UnityEvent<int> OnIterationsStepChanged { get; private set; }
        [SerializeField, Min(1)] private int _tasksToStart = 1;
        [field: SerializeField] public UnityEvent<int> OnTasksToStartChanged { get; private set; }
        [field: SerializeField] public UnityEvent OnTaskStarted { get; private set; }
        [field: SerializeField] public UnityEvent OnTaskEnded { get; private set; }
        [field: SerializeField] public UnityEvent<EdTaskResult> OnEdTaskResultReceived { get; private set; }
        private ConcurrentQueue<Sample[]> _samples = new ConcurrentQueue<Sample[]>();
        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private CancellationToken _token;
        private bool _isRunning;

        private void Awake()
        {
            SetIterationsStep(_iterationsStep);
            SetTasksToStart(_tasksToStart);
        }

        public void StartTaskWithIntermediateResults()
        {
            SetTasksToStart(1);
            StartTask(true);
        }

        public void StartTaskWithoutIntermediateResults()
        {
            StartTask(false);
        }

        public void SetIterationsStep(int value)
        {
            if (value < 0)
            {
                value = 0;
            }
            _iterationsStep = value;
            OnIterationsStepChanged.Invoke(_iterationsStep);
        }

        public void SetTasksToStart(int value)
        {
            if (value < 1)
            {
                value = 1;
            }
            _tasksToStart = value;
            OnTasksToStartChanged.Invoke(_tasksToStart);
        }

        private ElectronDynamicsTask GetEdTask(EdVariables edVariables, bool saveIntermediateResults, Action<Sample[]> onIntermediateResultReceived)
        {
            return new ElectronDynamicsTask(edVariables, _token, saveIntermediateResults, _iterationsStep, onIntermediateResultReceived,
                UnityEngine.Random.Range(0, 1_000_000_000));
        }

        private async void StartTask(bool saveIntermediateResults)
        {
            if (_isRunning)
            {
                return;
            }
            _isRunning = true;
            var variables = _variablesController.Variables;
            _token = _tokenSource.Token;
            var tasks = new List<System.Threading.Tasks.Task<EdTaskResult>>();
            var mainTask = System.Threading.Tasks.Task.Factory.StartNew(GetEdTask(variables, saveIntermediateResults, _samples.Enqueue).Execute);
            tasks.Add(mainTask);
            for (int i = 1; i < _tasksToStart; ++i)
            {
                var hiddenTask = System.Threading.Tasks.Task.Factory.StartNew(GetEdTask(variables, false, null).Execute);
                tasks.Add(hiddenTask);
            }
            Debug.Log($"{gameObject.name}: Tasks execution started", gameObject);
            OnTaskStarted.Invoke();
            await System.Threading.Tasks.Task.WhenAll(tasks);
            Debug.Log($"{gameObject.name}: Tasks execution ended", gameObject);
            OnTaskEnded.Invoke();
            foreach (var task in tasks)
            {
                OnEdTaskResultReceived.Invoke(task.Result);
            }
            _samples.Enqueue(tasks[0].Result.Samples.Last());
            _isRunning = false;
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
