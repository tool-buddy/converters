// =====================================================================
// Copyright © 2023 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

#if CURVY_SPLINES

using FluffyUnderware.Curvy;
using FluffyUnderware.DevTools;
using ToolBuddy.Converter.Common.Listeners;

namespace ToolBuddy.Converter.Unity.Listeners
{
    public class CurvySplineListener : BaseSplineListener<CurvySpline>
    {
        protected override void StartListeningToSpline(
            CurvySpline spline)
        {
            spline.OnRefresh.AddListenerOnce(OnSplineChanged);
        }

        protected override void StopListeningToSpline(
            CurvySpline spline)
        {
            spline.OnRefresh.RemoveListener(OnSplineChanged);
        }

        protected override void StartListeningToSplineType() { }

        protected override void StopListeningToSplineType() { }

        private void OnSplineChanged(
            CurvySplineEventArgs eventArguments)
        {
            CurvySpline spline = eventArguments.Spline;
            if (IsRegistered(spline) == false)
                DTLog.LogWarning("[Converter] OnSplineChanged called on an unregistered spline. Event will be ignored");
            else
                TriggerSplineChangedEvent(spline);
        }
    }
}
#endif