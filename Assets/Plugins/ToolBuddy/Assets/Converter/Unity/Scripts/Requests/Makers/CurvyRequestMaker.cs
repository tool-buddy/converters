// =====================================================================
// Copyright © 2023 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

#if CURVY_SPLINES

using FluffyUnderware.Curvy;
using JetBrains.Annotations;
using ToolBuddy.Converter.Common.Requests;
using Spline = UnityEngine.Splines.Spline;

namespace ToolBuddy.Converter.Unity.Requests.Makers
{
    public static class CurvyRequestMaker
    {
        public static SplineConversionRequest<CurvySpline, Spline> MakeSplineConversionRequests(
            [NotNull] CurvySpline source,
            [NotNull] Spline destination) =>
            new SplineConversionRequest<CurvySpline, Spline>(
                source,
                destination
            );
    }
}
#endif