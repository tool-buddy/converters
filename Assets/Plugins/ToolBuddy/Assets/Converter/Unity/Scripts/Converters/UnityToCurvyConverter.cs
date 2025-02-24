// =====================================================================
// Copyright © 2023 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

#if CURVY_SPLINES

using System.Collections.Generic;
using System.Linq;
using FluffyUnderware.Curvy;
using JetBrains.Annotations;
using ToolBuddy.Converter.Common.Converters;
using Unity.Mathematics;
using UnityEngine.Assertions;
using UnityEngine.Splines;

namespace ToolBuddy.Converter.Unity.Converters
{
    public class UnityToCurvyConverter : BaseConverter<Spline, CurvySpline>
    {
        public const string Id = "Unity To Curvy";

        protected override string GetConverterId => Id;

        /// <summary>
        /// Converts a Unity Spline to a Curvy Spline.
        /// </summary>
        /// <param name="source">The Unity Spline to be converted.</param>
        /// <param name="destination">The Curvy Spline to be filled with the data from the Unity Spline.</param>
        public override void Convert(
            Spline source,
            [NotNull] CurvySpline destination)
        {
            destination.Interpolation = CurvyInterpolation.Bezier;
            destination.Closed = source.Closed;

            SetSplineFromKnots(
                source.Knots.ToList(),
                destination
            );

            destination.Refresh();

            //assert no connections to the spline
            Assert.IsTrue(destination.ControlPointsList.All(cp => cp.Connection == null));
        }

        private static void SetSplineFromKnots(
            [NotNull] IReadOnlyList<BezierKnot> from,
            [NotNull] CurvySpline to)
        {
            to.SetControlPointCount(from.Count());

            for (int index = 0; index < from.Count(); index++)
                SetControlPointFromKnot(
                    from.ElementAt(index),
                    to.ControlPointsList[index]
                );
        }

        private static void SetControlPointFromKnot(
            BezierKnot from,
            [NotNull] CurvySplineSegment to)
        {
            if (to.Connection != null)
                to.Connection.RemoveControlPoint(to);
            to.AutoHandles = false;
            to.SetLocalPosition(from.Position);
            to.SetLocalRotation(from.Rotation);
            to.HandleIn = math.mul(
                from.Rotation,
                from.TangentIn
            );
            to.HandleOut = math.mul(
                from.Rotation,
                from.TangentOut
            );
        }
    }
}
#endif