using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace UnityEngine.U2D.Animation
{
    [BurstCompile]
    internal static class MeshUtilities
    {
        /// <summary>
        /// Get the outline edges from a set of indices.
        /// This method expects the index array to be laid out with one triangle for every 3 indices.
        /// E.g. triangle 0: index 0 - 2, triangle 1: index 3 - 5, etc.
        /// </summary>
        /// <returns>Returns a NativeArray of sorted edges. It is up to the caller to dispose this array.</returns>
        public static NativeArray<int2> GetOutlineEdges(in NativeArray<ushort> indices)
        {
            UnsafeHashMap<int, int3> edges = new UnsafeHashMap<int, int3>(indices.Length, Allocator.Persistent);

            for (int i = 0; i < indices.Length; i += 3)
            {
                ushort i0 = indices[i];
                ushort i1 = indices[i + 1];
                ushort i2 = indices[i + 2];

                int2 edge0 = new int2(i0, i1);
                int2 edge1 = new int2(i1, i2);
                int2 edge2 = new int2(i2, i0);

                AddToEdgeMap(edge0, ref edges);
                AddToEdgeMap(edge1, ref edges);
                AddToEdgeMap(edge2, ref edges);
            }

#if COLLECTIONS_2_0_OR_ABOVE
            NativeList<int2> outlineEdges = new NativeList<int2>(edges.Count, Allocator.Temp);
#else
            var outlineEdges = new NativeList<int2>(edges.Count(), Allocator.Temp);
#endif
            foreach (KVPair<int, int3> edgePair in edges)
            {
                // If an edge is only used in one triangle, it is an outline edge.
                if (edgePair.Value.z == 1)
                    outlineEdges.Add(edgePair.Value.xy);
            }

            edges.Dispose();

            SortEdges(outlineEdges.AsArray(), out NativeArray<int2> sortedEdges);
            return sortedEdges;
        }

        [BurstCompile]
        static void AddToEdgeMap(in int2 edge, ref UnsafeHashMap<int, int3> edgeMap)
        {
            int2 tmpEdge = math.min(edge.x, edge.y) == edge.x ? edge.xy : edge.yx;
            int hashCode = tmpEdge.GetHashCode();

            // We store the hashCode as key, so that we can do less GetHashCode-calls.
            // Then we store the count the int3s z-value.
            if (!edgeMap.ContainsKey(hashCode))
                edgeMap[hashCode] = new int3(edge, 1);
            else
            {
                int3 val = edgeMap[hashCode];
                val.z++;
                edgeMap[hashCode] = val;
            }
        }

        [BurstCompile]
        static void SortEdges(in NativeArray<int2> unsortedEdges, out NativeArray<int2> sortedEdges)
        {
            NativeArray<int2> tmpEdges = new NativeArray<int2>(unsortedEdges.Length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            NativeList<int> shapeStartingEdge = new NativeList<int>(1, Allocator.Persistent);

            UnsafeHashMap<int, int> edgeMap = new UnsafeHashMap<int, int>(unsortedEdges.Length, Allocator.Persistent);
            NativeArray<bool> usedEdges = new NativeArray<bool>(unsortedEdges.Length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

            for (int i = 0; i < unsortedEdges.Length; i++)
            {
                edgeMap[unsortedEdges[i].x] = i;
                usedEdges[i] = false;
            }

            bool findStartingEdge = true;
            int edgeIndex = -1;
            int startingEdge = 0;
            for (int i = 0; i < unsortedEdges.Length; i++)
            {
                if (findStartingEdge)
                {
                    edgeIndex = GetFirstUnusedIndex(usedEdges);
                    startingEdge = edgeIndex;
                    findStartingEdge = false;
                    shapeStartingEdge.Add(i);
                }

                usedEdges[edgeIndex] = true;
                tmpEdges[i] = unsortedEdges[edgeIndex];
                int nextVertex = unsortedEdges[edgeIndex].y;
                edgeIndex = edgeMap[nextVertex];

                if (edgeIndex == startingEdge)
                    findStartingEdge = true;
            }

            int finalEdgeArrLength = unsortedEdges.Length;
            sortedEdges = new NativeArray<int2>(finalEdgeArrLength, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            int count = 0;
            for (int i = 0; i < shapeStartingEdge.Length; ++i)
            {
                int edgeStart = shapeStartingEdge[i];
                int edgeEnd = (i + 1) == shapeStartingEdge.Length ? tmpEdges.Length : shapeStartingEdge[i + 1];

                for (int m = edgeStart; m < edgeEnd; ++m)
                    sortedEdges[count++] = tmpEdges[m];
            }

            usedEdges.Dispose();
            edgeMap.Dispose();
            shapeStartingEdge.Dispose();
            tmpEdges.Dispose();
        }

        [BurstCompile]
        static int GetFirstUnusedIndex(in NativeArray<bool> usedValues)
        {
            for (int i = 0; i < usedValues.Length; i++)
            {
                if (!usedValues[i])
                    return i;
            }

            return -1;
        }
    }
}
