// =====================================================================
// Copyright © 2023 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

#if CURVY_SPLINES

using FluffyUnderware.Curvy;
using ToolBuddy.Converter.Common.Converters;
using ToolBuddy.Converter.Common.Requests.Processors;
using ToolBuddy.Converter.Unity.Converters;
using Spline = UnityEngine.Splines.Spline;

namespace ToolBuddy.Converter.Unity.Requests.Processors
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public class UnityRequestProcessor : RequestProcessor<UnityRequestProcessor, Spline, CurvySpline>
    {
        static UnityRequestProcessor()
        {
            // Ensure the instance is created
            UnityRequestProcessor instance = Instance;
        }

        protected override BaseConverter<Spline, CurvySpline> GetConverter() =>
            new UnityToCurvyConverter();
    }
}
#endif