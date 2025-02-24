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
using UnityEngine.Splines;

namespace ToolBuddy.Converter.Unity.Requests.Processors
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public class CurvyRequestProcessor : RequestProcessor<CurvyRequestProcessor, CurvySpline, Spline>
    {
        static CurvyRequestProcessor()
        {
            // Ensure the instance is created
            CurvyRequestProcessor instance = Instance;
        }

        protected override BaseConverter<CurvySpline, Spline> GetConverter() =>
            new CurvyToUnityConverter();
    }
}
#endif