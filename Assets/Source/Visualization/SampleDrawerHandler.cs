using ElectronDynamics.Task;
using ElectronDynamics.Task.UnityExtensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

namespace ElectronDynamics.Visualization
{
    public class SampleDrawerHandler : MonoBehaviour
    {
        [SerializeField] private GameObject _sampleDrawer;
        [SerializeField] private Transform _instanceParent;
        [SerializeField] private bool _ignoreLastSample = true;
        [SerializeField] private Transform _axisXBar;
        [SerializeField] private TMP_Text _axisXRangeText;
        [SerializeField] private Transform _axisYBar;
        [SerializeField] private TMP_Text _axisYRangeText;
        [SerializeField] private Transform _axisZBar;
        [SerializeField] private TMP_Text _axisZRangeText;
        private ObjectPool<SampleDrawer> _drawersPool;
        private List<SampleDrawer> _sampleDrawers;
        private bool _isPoolCreated = false;
        private IFormatProvider _formatProvider = CultureInfo.InvariantCulture.NumberFormat;

        private void Awake()
        {
            CreatePoolIfNull();
        }

        private void CreatePoolIfNull()
        {
            if (_isPoolCreated)
            {
                return;
            }
            _drawersPool = new ObjectPool<SampleDrawer>(createFunc: CreateDrawer, actionOnGet: OnGetDrawer,
                actionOnRelease: OnReleaseDrawer, actionOnDestroy: OnDestroyDrawer);
            _sampleDrawers = new List<SampleDrawer>();
            _isPoolCreated = true;
        }

        public void ClearDrawers()
        {
            CreatePoolIfNull();
            foreach (var drawer in _sampleDrawers)
            {
                _drawersPool.Release(drawer);
            }
            _sampleDrawers.Clear();
        }

        public void DrawSamples(Sample[] samples)
        {
            if (samples == null || samples.Length == 0)
            {
                ClearDrawers();
                return;
            }
            int samplesCount = samples.Length;
            if (_ignoreLastSample)
            {
                samplesCount--;
            }
            if (samplesCount == 0)
            {
                ClearDrawers();
                return;
            }
            var (minBounds, maxBounds) = GetBoundsByPosition(samples, samplesCount);
            double maxExtents = GetBoundsMaxExtents(minBounds, maxBounds);
            var (minCubeBounds, maxCubeBounds) = GetBoundsWithEqualExtents(minBounds, maxBounds, maxExtents);
            if (minCubeBounds == maxCubeBounds)
            {
                ClearDrawers();
                return;
            }
            double multiplier = 1d / (maxCubeBounds.X - minCubeBounds.X);
            SetDrawersCount(samplesCount);
            for (int i = 0; i < samplesCount; ++i)
            {
                var sample = samples[i];
                var position = sample.Position;
                var velocity = sample.Velocity;
                Vector3 scaledPosition = (multiplier * (position - minCubeBounds)).ToVector3();
                Vector3 scaledVelocity = velocity.ToVector3();
                var drawer = _sampleDrawers[i];
                drawer.SetValues(scaledPosition, scaledVelocity);
            }
            DrawAxisBars(minBounds, maxBounds);
        }

        private void DrawAxisBars(EdVector3 minBounds, EdVector3 maxBounds)
        {
            double xRange = maxBounds.X - minBounds.X;
            double yRange = maxBounds.Y - minBounds.Y;
            double zRange = maxBounds.Z - minBounds.Z;
            double maxRange = Math.Max(xRange, Math.Max(yRange, zRange));
            _axisXBar.localScale = new Vector3(1f, (float)(xRange / maxRange), 1f);
            _axisXRangeText.SetText(xRange.ToString(_formatProvider));
            _axisYBar.localScale = new Vector3(1f, (float)(yRange / maxRange), 1f);
            _axisYRangeText.SetText(yRange.ToString(_formatProvider));
            _axisZBar.localScale = new Vector3(1f, (float)(zRange / maxRange), 1f);
            _axisZRangeText.SetText(zRange.ToString(_formatProvider));
        }

        private void SetDrawersCount(int count)
        {
            CreatePoolIfNull();
            int drawersToAdd = count - _sampleDrawers.Count;
            if (drawersToAdd == 0)
            {
                return;
            }
            if (drawersToAdd > 0)
            {
                for (int i = 0; i < drawersToAdd; ++i)
                {
                    var drawer = _drawersPool.Get();
                    _sampleDrawers.Add(drawer);
                }
            }
            else
            {
                for (int i = drawersToAdd; i < 0; ++i)
                {
                    int last = _sampleDrawers.Count - 1;
                    var drawer = _sampleDrawers[last];
                    _sampleDrawers.RemoveAt(last);
                    _drawersPool.Release(drawer);
                }
            }
        }

        private void ClearPool()
        {
            if (!_isPoolCreated)
            {
                return;
            }
            _drawersPool?.Clear();
            _sampleDrawers?.Clear();
        }

        private void OnDisable()
        {
            ClearPool();
        }

        private void OnDestroy()
        {
            _drawersPool?.Dispose();
        }

        #region ObjectPool Functions

        private SampleDrawer CreateDrawer()
        {
            GameObject instance = Instantiate(_sampleDrawer, _instanceParent);
            if (!instance.TryGetComponent(out SampleDrawer sampleDrawer))
            {
                Debug.LogError($"{gameObject.name}: Can't find component of type {typeof(SampleDrawer)} in prefab instance");
                Destroy(instance);
                return null;
            }
            return sampleDrawer;
        }

        private void OnGetDrawer(SampleDrawer sampleDrawer)
        {
            sampleDrawer.SetVisible();
        }

        private void OnReleaseDrawer(SampleDrawer sampleDrawer)
        {
            sampleDrawer.SetHidden();
        }

        private void OnDestroyDrawer(SampleDrawer sampleDrawer) { }

        #endregion

        #region Bounds Receivers

        private static (EdVector3, EdVector3) GetBounds(Sample[] samples, int samplesCount)
        {
            var (minBounds, maxBounds) = GetBoundsByPosition(samples, samplesCount);
            double maxExtents = GetBoundsMaxExtents(minBounds, maxBounds);
            return GetBoundsWithEqualExtents(minBounds, maxBounds, maxExtents);
        }

        private static (EdVector3, EdVector3) GetBoundsByPosition(Sample[] samples, int samplesCount)
        {
            double minX = double.PositiveInfinity;
            double minY = double.PositiveInfinity;
            double minZ = double.PositiveInfinity;
            double maxX = double.NegativeInfinity;
            double maxY = double.NegativeInfinity;
            double maxZ = double.NegativeInfinity;
            for (int i = 0; i < samplesCount; ++i)
            {
                var sample = samples[i];
                var position = sample.Position;
                // min
                if (position.X < minX)
                {
                    minX = position.X;
                }
                if (position.Y < minY)
                {
                    minY = position.Y;
                }
                if (position.Z < minZ)
                {
                    minZ = position.Z;
                }
                // max
                if (position.X > maxX)
                {
                    maxX = position.X;
                }
                if (position.Y > maxY)
                {
                    maxY = position.Y;
                }
                if (position.Z > maxZ)
                {
                    maxZ = position.Z;
                }
            }
            var minBounds = new EdVector3(minX, minY, minZ);
            var maxBounds = new EdVector3(maxX, maxY, maxZ);
            return (minBounds, maxBounds);
        }

        private static double GetBoundsMaxExtents(EdVector3 minBounds, EdVector3 maxBounds)
        {
            double rangeX = maxBounds.X - minBounds.X;
            double rangeY = maxBounds.Y - minBounds.Y;
            double rangeZ = maxBounds.Z - minBounds.Z;
            double max = Math.Max(rangeX, Math.Max(rangeY, rangeZ)) / 2d;
            return max;
        }

        private static (EdVector3, EdVector3) GetBoundsWithEqualExtents(EdVector3 minBounds, EdVector3 maxBounds, double maxExtents)
        {
            EdVector3 center = minBounds + 0.5d * (maxBounds - minBounds);
            EdVector3 extents = new EdVector3(maxExtents, maxExtents, maxExtents);
            minBounds = center - extents;
            maxBounds = center + extents;
            return (minBounds, maxBounds);
        }

        #endregion
    }
}
