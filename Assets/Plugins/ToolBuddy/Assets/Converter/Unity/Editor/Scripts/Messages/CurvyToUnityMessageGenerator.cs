// =====================================================================
// Copyright © 2023 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using System.Linq;
using ToolBuddy.Converter.Editor.Common.Messages;
using UnityEditor;
using UnityEngine.Splines;
#if CURVY_SPLINES
using FluffyUnderware.Curvy;
using FluffyUnderware.DevTools.Extensions;
using ToolBuddy.Converter.Common.Requests;
using ToolBuddy.Converter.Unity.Requests.Processors;
#endif

namespace ToolBuddy.Converter.Editor.Unity.Messages
{
    [InitializeOnLoad]
    public static class CurvyToUnityMessageGenerator
    {
        static CurvyToUnityMessageGenerator()
        {
#if CURVY_SPLINES
            CurvyRequestProcessor.Instance.AfterSplineConversion += OnSplineConversionRequestProcessed;
#endif
        }

#if CURVY_SPLINES
        private static void OnSplineConversionRequestProcessed(
            SplineConversionRequest<CurvySpline, Spline>[] requests)
        {
            const string SupportedTypesMessage =
                "\nFully supported interpolation types are: Bezier and Linear.\nPartially supported interpolation types are: Catmull Rom.";
            MessageRepository messages = MessageRepository.Instance;

            requests.Where(r => r.Source is { Interpolation: CurvyInterpolation.BSpline })
                .Select(r => r.Source)
                .ForEach(
                    s => messages.AddMessage(
                        $"Tried converting spline(s) with B Spline interpolation. This is currently not supported.{SupportedTypesMessage}",
                        MessageType.Error
                    )
                );

            requests.Where(r => r.Source is { Interpolation: CurvyInterpolation.TCB })
                .Select(r => r.Source)
                .ForEach(
                    s => messages.AddMessage(
                        $"Tried converting spline(s) with TCB interpolation. This is not currently supported.{SupportedTypesMessage}",
                        MessageType.Error
                    )
                );

            requests.Where(r => r.Source is { Interpolation: CurvyInterpolation.CatmullRom })
                .Select(r => r.Source)
                .ForEach(
                    s => messages.AddMessage(
                        $"Converted spline(s) with Catmull-Rom interpolation. Differences may occur because Unity uses the Centripetal variant, while Curvy Splines uses the Uniform one.{SupportedTypesMessage}",
                        MessageType.Warning
                    )
                );
        }
#endif
    }
}