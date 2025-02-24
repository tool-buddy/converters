// =====================================================================
// Copyright © 2023 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

#if CURVY_SPLINES

using System;
using System.Collections.Generic;
using FluffyUnderware.Curvy;
using FluffyUnderware.DevTools.Extensions;
using JetBrains.Annotations;
using ToolBuddy.Converter.Common.Requests;
using ToolBuddy.Converter.Editor.Common.Controllers;
using ToolBuddy.Converter.Unity.Converters;
using ToolBuddy.Converter.Unity.Listeners;
using ToolBuddy.Converter.Unity.Requests.Makers;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

namespace ToolBuddy.Converter.Editor.Unity.Controllers
{
    /// <summary>
    /// Handles the conversion of splines from Unity splines to Curvy splines.
    /// </summary>
    [InitializeOnLoad]
    public class UnityToCurvyConversionController : BaseConversionController<SplineContainer>
    {
        [UsedImplicitly]
        private static UnityToCurvyConversionController instance;

        static UnityToCurvyConversionController() =>
            instance = new UnityToCurvyConversionController();

        private UnityToCurvyConversionController() : base(
            new UnitySplineListener(),
            UnityToCurvyConverter.Id
        ) { }

        protected override void ConvertSpline(
            [NotNull] SplineContainer splineContainer)
        {
            IReadOnlyList<CurvySpline> curvySplines = RequestResources(splineContainer);

            List<SplineConversionRequest<Spline, CurvySpline>> splineRequests = UnityRequestMaker.MakeSplineConversionRequests(
                splineContainer,
                curvySplines
            );

            IEnumerable<ConnectionConversionRequest> connectionRequests = UnityRequestMaker.MakeConnectionConversionRequests(
                splineContainer,
                curvySplines
            );

            foreach (SplineConversionRequest<Spline, CurvySpline> request in splineRequests)
                ConversionRequestQueue<Spline, CurvySpline>.Instance.PushSplineRequest(
                    request
                );

            foreach (ConnectionConversionRequest request in connectionRequests)
                ConversionRequestQueue<Spline, CurvySpline>.Instance.PushConnectionRequest(
                    request
                );
        }

        /// <summary>
        /// Requests the necessary resources for spline conversion and cleans up the container's children.
        /// </summary>
        /// <param name="splineContainer">The container from which to request resources.</param>
        /// <returns>A list of <see cref="CurvySpline"/> objects that are required for the conversion.</returns>
        [NotNull]
        [ItemNotNull]
        private static List<CurvySpline> RequestResources(
            [NotNull] SplineContainer splineContainer)
        {
            int splinesCount = splineContainer.Splines.Count;

            TransformExt.DeleteChildrenOfType<CurvySpline>(
                splineContainer.transform,
                true,
                true,
                splinesCount,
                out List<Transform> skippedChildren
            );

            List<CurvySpline> curvySplines = new List<CurvySpline>(splinesCount);

            //Reuse existing CurvySplines
            for (int index = 0; index < skippedChildren.Count; index++)
            {
                Transform existingSpline = skippedChildren[index];
                CurvySpline curvySpline = existingSpline.GetComponent<CurvySpline>();
                curvySpline.name = GetResourceName(
                    splineContainer,
                    index
                );
                curvySpline.Clear();
                curvySplines.Add(curvySpline);
            }

            //Add new CurvySplines if needed
            for (int index = skippedChildren.Count; index < splinesCount; index++)
            {
                CurvySpline curvySpline = AddChildComponent<CurvySpline>(
                    splineContainer.gameObject,
                    GetResourceName(
                        splineContainer,
                        index
                    )
                );
                curvySpline.Start();
                curvySplines.Add(curvySpline);
            }

            return curvySplines;
        }

        /// <summary>
        /// Generates a resource name for a spline based on its index within the container.
        /// </summary>
        /// <param name="splineContainer">The container holding the spline.</param>
        /// <param name="index">The index of the spline within the container.</param>
        /// <returns>A string representing the name of the resource.</returns>
        [NotNull]
        private static string GetResourceName(
            [NotNull] SplineContainer splineContainer,
            int index)
        {
            string nameSuffix =
                    splineContainer.Splines.Count > 1
                        ? $" {index}"
                        : String.Empty
                ;
            return $"{splineContainer.name} - Curvy Spline{nameSuffix}";
        }
    }
}
#endif