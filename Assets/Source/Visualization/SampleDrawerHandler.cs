using ElectronDynamics.Task;
using ElectronDynamics.Task.UnityExtensions;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace ElectronDynamics.Visualization
{
    public class SampleDrawerHandler : MonoBehaviour
    {
        [SerializeField] private GameObject _sampleDrawer;
        [SerializeField] private Transform _instanceParent;
        private IObjectPool<SampleDrawer> _drawersPool;
        private IList<SampleDrawer> _sampleDrawers;
        private bool _isPoolCreated = false;

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
            var (minBounds, maxBounds) = GetBounds(samples);
            if (minBounds == maxBounds)
            {
                ClearDrawers();
                return;
            }
            int samplesCount = samples.Length;
            double multiplier = 1d / (maxBounds.X - minBounds.X);
            SetDrawersCount(samplesCount);
            for (int i = 0; i < samplesCount; ++i)
            {
                var sample = samples[i];
                var position = sample.Position;
                var velocity = sample.Velocity;
                Vector3 scaledPosition = (multiplier * (position - minBounds)).ToVector3();
                Vector3 scaledVelocity = velocity.ToVector3();
                var drawer = _sampleDrawers[i];
                drawer.SetValues(scaledPosition, scaledVelocity);
            }
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
                    int last = _sampleDrawers.Count;
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
            _drawersPool.Clear();
            _sampleDrawers.Clear();
        }

        private void OnDestroy()
        {
            ClearPool();
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

        private static (EdVector3, EdVector3) GetBounds(Sample[] samples)
        {
            var (minBounds, maxBounds) = GetBoundsByPosition(samples);
            double maxExtents = GetBoundsMaxExtents(minBounds, maxBounds);
            return GetBoundsWithEqualExtents(minBounds, maxBounds, maxExtents);
        }

        private static (EdVector3, EdVector3) GetBoundsByPosition(Sample[] samples)
        {
            double minX = double.PositiveInfinity;
            double minY = double.PositiveInfinity;
            double minZ = double.PositiveInfinity;
            double maxX = double.NegativeInfinity;
            double maxY = double.NegativeInfinity;
            double maxZ = double.NegativeInfinity;
            foreach (var sample in samples)
            {
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
