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
using ToolBuddy.Converter.Common.Requests;
using ToolBuddy.Converter.Common.SplineAbstraction;
using UnityEngine.Splines;

namespace ToolBuddy.Converter.Unity.Requests.Makers
{
    public static class UnityRequestMaker
    {
        [NotNull]
        public static List<SplineConversionRequest<Spline, CurvySpline>> MakeSplineConversionRequests(
            [NotNull] SplineContainer sources,
            [NotNull] [ItemNotNull] IReadOnlyList<CurvySpline> destinations)
        {
            List<SplineConversionRequest<Spline, CurvySpline>> conversionRequests =
                new List<SplineConversionRequest<Spline, CurvySpline>>(sources.Splines.Count);
            for (int splineIndex = 0; splineIndex < sources.Splines.Count; splineIndex++)
            {
                SplineConversionRequest<Spline, CurvySpline> request = new SplineConversionRequest<Spline, CurvySpline>(
                    sources.Splines[splineIndex],
                    destinations[splineIndex]
                );

                conversionRequests.Add(request);
            }

            return conversionRequests;
        }

        #region Connections

        [NotNull]
        public static IEnumerable<ConnectionConversionRequest> MakeConnectionConversionRequests(
            [NotNull] SplineContainer splineContainer,
            [NotNull] [ItemNotNull] IReadOnlyList<CurvySpline> curvySplines) =>
            UnityConnectionsToConnectionRequests(
                curvySplines,
                GetUnityConnections(splineContainer)
            );

        [NotNull]
        private static IEnumerable<ConnectionConversionRequest> UnityConnectionsToConnectionRequests(
            [NotNull] [ItemNotNull] IReadOnlyList<CurvySpline> curvySplines,
            [NotNull] HashSet<SplineConnection<int>> unityConnections)
        {
            List<ConnectionConversionRequest> connectionRequests = new List<ConnectionConversionRequest>(unityConnections.Count);
            foreach (SplineConnection<int> unityConnection in unityConnections)
            {
                ICollection<ControlPoint<int>> unityKnots = unityConnection.ControlPoints;
                IEnumerable<ControlPoint<CurvySpline>> curyvControlPoints = unityKnots.Select(
                    k => new ControlPoint<CurvySpline>(
                        curvySplines[k.Spline],
                        k.Index
                    )
                );
                connectionRequests.Add(new ConnectionConversionRequest(new SplineConnection<CurvySpline>(curyvControlPoints)));
            }

            return connectionRequests;
        }

        [NotNull]
        private static HashSet<SplineConnection<int>> GetUnityConnections(
            [NotNull] SplineContainer splineContainer)
        {
            HashSet<SplineConnection<int>> connections = new HashSet<SplineConnection<int>>();

            for (int splineIndex = 0; splineIndex < splineContainer.Splines.Count; splineIndex++)
                AddSplineConnectionsToHashset(
                    connections,
                    splineContainer,
                    splineIndex
                );
            return connections;
        }

        private static void AddSplineConnectionsToHashset(
            [NotNull] HashSet<SplineConnection<int>> setToFill,
            [NotNull] SplineContainer splineContainer,
            int splineIndex)
        {
            Spline spline = splineContainer.Splines[splineIndex];
            for (int knotIndex = 0; knotIndex < spline.Count; knotIndex++)
            {
                List<SplineKnotIndex> connectedKnots;
                {
                    SplineKnotIndex cpInfo = new SplineKnotIndex(
                        splineIndex,
                        knotIndex
                    );
                    connectedKnots = splineContainer.KnotLinkCollection.GetKnotLinks(cpInfo).ToList();
                }
                if (connectedKnots.Count > 1)
                {
                    IEnumerable<ControlPoint<int>> controlPoints =
                        connectedKnots.Select(
                            k => new ControlPoint<int>(
                                k.Spline,
                                k.Knot
                            )
                        );
                    SplineConnection<int> connection = new SplineConnection<int>(controlPoints);
                    setToFill.Add(connection);
                }
            }
        }

        #endregion
    }
}
#endif