// =====================================================================
// Copyright © 2023 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

#if CURVY_SPLINES

using System;
using System.Collections.Generic;
using System.Linq;
using FluffyUnderware.Curvy;
using FluffyUnderware.DevTools.Extensions;
using FluffyUnderware.DevToolsEditor;
using JetBrains.Annotations;
using ToolBuddy.Converter.Common.Requests;
using ToolBuddy.Converter.Editor.Common.Controllers;
using ToolBuddy.Converter.Editor.Common.Models;
using ToolBuddy.Converter.Unity.Converters;
using ToolBuddy.Converter.Unity.Listeners;
using ToolBuddy.Converter.Unity.Requests.Makers;
using UnityEditor;
using UnityEngine.Splines;
using Object = UnityEngine.Object;

namespace ToolBuddy.Converter.Editor.Unity.Controllers
{
    /// <summary>
    /// Handles the conversion of splines from Unity splines to Curvy splines.
    /// </summary>
    [InitializeOnLoad]
    public class CurvyToUnityConversionController : BaseConversionController<CurvySpline>
    {
        [UsedImplicitly]
        private static CurvyToUnityConversionController instance;

        static CurvyToUnityConversionController() =>
            instance = new CurvyToUnityConversionController();


        private CurvyToUnityConversionController() : base(
            new CurvySplineListener(),
            CurvyToUnityConverter.Id
        ) { }

        protected override IEnumerable<CurvySpline> GetSplinesToConvert(
            ConversionSource conversionSource)
        {
            switch (conversionSource)
            {
                case ConversionSource.AllSplines:
                    return ObjectExt.FindObjectsOfTypeInCurrentStage<CurvySpline>(
                        false,
                        false
                    );
                case ConversionSource.SelectedSplines:
                {
                    List<CurvySpline> selectedSplines = DTSelection.GetAllAs<CurvySpline>();
                    List<CurvySplineSegment> selectedCPs = DTSelection.GetAllAs<CurvySplineSegment>();
                    selectedSplines.AddRange(
                        selectedCPs
                            .Select(cp => cp.Spline)
                            .Where(spline => spline != null)
                    );
                    return selectedSplines.Distinct().ToList();
                }
                case ConversionSource.None:
                    return Enumerable.Empty<CurvySpline>();
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(conversionSource),
                        conversionSource,
                        null
                    );
            }
        }

        protected override void ConvertSpline(
            [NotNull] CurvySpline spline)
        {
            bool requestFromSameSplineContainerExists =
                ConversionRequestQueue<CurvySpline, Spline>.Instance.GetSplineRequests()
                    .Any(r => r.Source is CurvySpline curvySpline && spline == curvySpline);

            if (requestFromSameSplineContainerExists)
                //A request with current spline is pending, so we ignore all further requests from the same spline.
                //The reason for this is to make implementation faster. Let me explain:
                //When creating a CurvySpline to SplineContainer conversion request, we do not reuse the destination SplineContainer, but delete it and create a new one. This is not the best way performance-wise, but it is the easiest way to implement. Because of this choice, any new request will deleter the destination of the previous request, leading to a request that needs to convert to a destroyed GO -> errors.
                //todo remove this restriction.
                return;

            Spline unitySpline = RequestResource(spline);

            SplineConversionRequest<CurvySpline, Spline> request = CurvyRequestMaker.MakeSplineConversionRequests(
                spline,
                unitySpline
            );

            ConversionRequestQueue<CurvySpline, Spline>.Instance.PushSplineRequest(
                request
            );
        }

        private static Spline RequestResource(
            [NotNull] CurvySpline curvySpline)
        {
            curvySpline.transform.DeleteChildrenOfType<SplineContainer>(
                true,
                true
            );

            SplineContainer splineContaienr = AddChildComponent<SplineContainer>(
                curvySpline.gameObject,
                $"{curvySpline.name} - Unity Spline Container"
            );

            return splineContaienr.Spline;
        }
    }
}
#endif