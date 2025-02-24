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
using JetBrains.Annotations;
using ToolBuddy.Converter.Common.Converters;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace ToolBuddy.Converter.Unity.Converters
{
    /// <summary>
    /// Provides methods for converting Curvy splines to Unity splines.
    /// </summary>
    public class CurvyToUnityConverter : BaseConverter<CurvySpline, Spline>
    {
        public const string Id = "Curvy To Unity";

        protected override string GetConverterId => Id;

        /// <summary>
        /// Converts a Curvy spline to a Unity spline.
        /// </summary>
        /// <param name="source">The source Curvy spline to convert.</param>
        /// <param name="destination">The destination Unity spline to populate.</param>
        public override void Convert(
            CurvySpline source,
            Spline destination)
        {
            destination.Closed = source.Closed;

            //todo optim update existing knots instead of creating a new list and clearing the old one
            List<BezierKnot> bezierKnots = GetKnotsFromControlPoints(source);

            SetSplineFromKnots(
                destination,
                bezierKnots
            );
            //todo find a way to force refresh the spline
        }

        /// <summary>
        /// Generates a list of Bezier knots from a collection of Curvy spline control points.
        /// </summary>
        /// <returns>A list of Bezier knots corresponding to the control points.</returns>
        [NotNull]
        private static List<BezierKnot> GetKnotsFromControlPoints(
            CurvySpline spline)
        {
            List<BezierKnot> knots = new List<BezierKnot>();

            switch (spline.Interpolation)
            {
                case CurvyInterpolation.Bezier:
                {
                    knots.AddRange(
                        spline.ControlPointsList.Select(
                            segment =>
                                new BezierKnot(
                                    segment.transform.localPosition,
                                    Quaternion.Inverse(segment.transform.localRotation) * segment.HandleIn,
                                    Quaternion.Inverse(segment.transform.localRotation) * segment.HandleOut,
                                    segment.transform.localRotation
                                )
                        )
                    );
                    break;
                }
                case CurvyInterpolation.Linear:
                {
                    knots.AddRange(
                        spline.ControlPointsList.Select(
                            cp =>
                                new BezierKnot(
                                    cp.transform.localPosition,
                                    Vector3.zero,
                                    Vector3.zero,
                                    cp.transform.localRotation
                                )
                        )
                    );
                    break;
                }
                case CurvyInterpolation.CatmullRom:
                {
                    //IEnumerable<CurvySplineSegment> catmullRomCPs = spline.AutoEndTangents == false
                    //    ? spline.ControlPointsList.Skip(1).Take(spline.ControlPointsList.Count - 2)
                    //    : spline.ControlPointsList;

                    IEnumerable<CurvySplineSegment> catmullRomCPs = spline.ControlPointsList;

                    knots.AddRange(
                        SplineFactory.CreateCatmullRom(
                            catmullRomCPs.Select(cp => new float3(cp.transform.localPosition)).ToList(),
                            //controlPoints.Select(cp => new quaternion(
                            //    cp.transform.localRotation.x,
                            //    cp.transform.localRotation.y,
                            //    cp.transform.localRotation.z,
                            //    cp.transform.localRotation.w
                            //    )).ToList(),
                            spline.Closed
                        ).Knots
                    );
                }
                    break;
                case CurvyInterpolation.TCB:
                case CurvyInterpolation.BSpline:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(spline.Interpolation),
                        spline.Interpolation,
                        null
                    );
            }

            return knots;
        }

        /// <summary>
        /// Sets the knots of a Unity spline from a list of Bezier knots.
        /// </summary>
        /// <param name="spline">The Unity spline to update.</param>
        /// <param name="knots">The list of Bezier knots to set on the spline.</param>
        private static void SetSplineFromKnots(
            [NotNull] Spline spline,
            [NotNull] IReadOnlyList<BezierKnot> knots)
        {
            spline.Clear();
            foreach (BezierKnot knot in knots)
                spline.Add(knot);
        }
    }
}
#endif