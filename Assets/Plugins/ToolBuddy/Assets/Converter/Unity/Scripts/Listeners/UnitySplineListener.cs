using System.Linq;
using JetBrains.Annotations;
using ToolBuddy.Converter.Common.Listeners;
using UnityEngine;
using UnityEngine.Splines;
using Object = UnityEngine.Object;

namespace ToolBuddy.Converter.Unity.Listeners
{
    public class UnitySplineListener : BaseSplineListener<SplineContainer>
    {
        protected override void StartListeningToSpline(
            SplineContainer spline) { }

        protected override void StopListeningToSpline(
            SplineContainer spline) { }

        protected override void StartListeningToSplineType()
        {
            SplineContainer.SplineAdded += OnContainerModified;
            SplineContainer.SplineRemoved += OnContainerModified;
            SplineContainer.SplineReordered += OnContainerReordered;
            Spline.Changed += OnSplineChanged;
        }

        protected override void StopListeningToSplineType()
        {
            SplineContainer.SplineAdded -= OnContainerModified;
            SplineContainer.SplineRemoved -= OnContainerModified;
            SplineContainer.SplineReordered -= OnContainerReordered;
            Spline.Changed -= OnSplineChanged;
        }

        private void OnSplineChanged(
            [NotNull] Spline spline,
            int knotIndex,
            SplineModification modification)
        {
            SplineContainer splineContainer = GetSplineContainer(spline);
            if (splineContainer != null && IsRegistered(splineContainer))
                TriggerSplineChangedEvent(splineContainer);
        }

        [CanBeNull]
        private static SplineContainer GetSplineContainer(
            [NotNull] Spline spline)
        {
            SplineContainer[] activeSplineContainers = Object.FindObjectsByType<SplineContainer>(
                FindObjectsInactive.Exclude,
                FindObjectsSortMode.None
            );
            return activeSplineContainers.SingleOrDefault(c => c.Splines.Contains(spline));
        }

        private void OnContainerReordered(
            [NotNull] SplineContainer splineContainer,
            int previousSplineIndex,
            int newSplineIndex)
        {
            if (IsRegistered(splineContainer))
                TriggerSplineChangedEvent(splineContainer);
        }

        private void OnContainerModified(
            [NotNull] SplineContainer splineContainer,
            int splineIndex)
        {
            if (IsRegistered(splineContainer))
                TriggerSplineChangedEvent(splineContainer);
        }
    }
}