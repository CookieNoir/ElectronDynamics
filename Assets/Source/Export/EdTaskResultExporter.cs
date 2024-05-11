using ElectronDynamics.Task;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

namespace ElectronDynamics.Export
{
    public class EdTaskResultExporter : MonoBehaviour
    {
        [field: SerializeField] public UnityEvent<string> OnDirectoryPathChanged { get; private set; }
        private string _directoryPath;
        private static string DefaultDirectoryPath => Application.persistentDataPath.Replace("/", "\\");

        private void Awake()
        {
            SetDirectoryPath(null);
        }

        public void SetDirectoryPath(string directoryPath)
        {
            if (directoryPath == null ||
                !IsPathValid(directoryPath, out string validDirectoryPath))
            {
                _directoryPath = DefaultDirectoryPath;
            }
            else
            {
                _directoryPath = validDirectoryPath;
            }
            OnDirectoryPathChanged.Invoke(_directoryPath);
        }

        private bool IsPathValid(string path, out string validPath)
        {
            validPath = null;
            bool status = false;
            try
            {
                validPath = Path.GetFullPath(path);
                status = true;
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"{gameObject.name}: {exception.Message}", gameObject);
            }
            return status;
        }

        public void ExportResultAsCSV(EdTaskResult edTaskResult)
        {
            try
            {
                if (string.IsNullOrEmpty(_directoryPath))
                {
                    SetDirectoryPath(DefaultDirectoryPath);
                }
                string csv = edTaskResult.ToCSV();
                string fileName = $"{edTaskResult.GetHashCode().ToString("X").Substring(2)}.{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.csv";
                string filePath = Path.Combine(_directoryPath, fileName);
                File.WriteAllText(filePath, csv);
                Debug.Log($"{gameObject.name}: File written successfully, file path: {filePath}", gameObject);
            }
            catch (Exception exception)
            {
                Debug.LogError($"{gameObject.name}: {exception.Message}", gameObject);
            }
        }
    }
}
