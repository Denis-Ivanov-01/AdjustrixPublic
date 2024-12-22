namespace LSA_Base
{

    public enum TraverseType
    {
        Closed = 0,
        Linked = 1
    }

    internal static class SupersetRemover
    {
        /// <summary>
        /// Removes supersets if the parts of a traverse are found in the other traverses.
        /// </summary>
        /// <param name="traverses">All distinct traverses</param>
        /// <param name="containedtraverseFromBreakdown">The contained traverses that are result of breaking down closed traverses.
        /// They are the only ones that are being tested for being composite supersets. 
        /// They are not compared to other such traverses.</param>
        /// <returns></returns>
        public static List<List<PointBase>> RemoveCompositeSupersets(List<List<PointBase>> traverses, List<List<PointBase>> containedtraversesFromBreakdown)
        {
            traverses = traverses.OrderByDescending(item => item.Count).ToList();
            bool supersetFound = true;
            while (supersetFound)
            {
                traverses = traverses.OrderByDescending(item => item.Count).ToList();
                traverses = RemoveCompositeSuperset(traverses, ref supersetFound, containedtraversesFromBreakdown);
            }
            return traverses;
        }

        public static List<List<PointBase>> RemoveCompositeSuperset(List<List<PointBase>> traverses,
            ref bool supersetFound,
            List<List<PointBase>> containedtraversesFromBreakdown)
        {
            supersetFound = false;
            for (int i = 0; i < traverses.Count; i++)
            {
                List<PointBase> currtraverse = traverses[i];
                if (containedtraversesFromBreakdown.Contains(currtraverse)) { continue; }
                List<List<PointBase>> traversesToCompare = new();
                traversesToCompare.AddRange(containedtraversesFromBreakdown);
                traversesToCompare.AddRange(traverses.Where(item => item.Count < currtraverse.Count));
                if (CheckIfCurrtraverseIsSubsetOf(currtraverse, traversesToCompare))
                {
                    supersetFound = true;
                    traverses.RemoveAt(i);
                    break;
                }
            }
            return traverses;
        }

        public static bool CheckIfCurrtraverseIsSubsetOf(List<PointBase> currTraverse,
            List<List<PointBase>> traversesToCompare)
        {
            List<Tuple<PointBase, PointBase>> currPairs = GetPointPairs(currTraverse);
            int initialPairsCount = currPairs.Count;
            int pairsFoundInOtherTraverses = 0;


            for (int i = 0; i < traversesToCompare.Count; i++)
            {
                if (i >= traversesToCompare.Count) { break; } //invalid index
                List<PointBase> traverseToCompare = traversesToCompare[i];

                List<int> indicesToRemove = GetIndicesToRemove(currPairs, ref pairsFoundInOtherTraverses, traverseToCompare);
                currPairs.RemoveAtIndices(indicesToRemove);

            }
            return pairsFoundInOtherTraverses == initialPairsCount; //True means that all pairs exist in other traverses
        }

        private static List<int> GetIndicesToRemove(List<Tuple<PointBase, PointBase>> currPairs, ref int pairsFoundInOtherTraverses, List<PointBase> traverseToCompare)
        {
            List<int> indicesToRemove = new();

            foreach (Tuple<PointBase, PointBase> pair in currPairs)
            {
                Tuple<PointBase, PointBase> pairReverse = new(pair.Item2, pair.Item1);
                if (ContainsTupleInOrder(traverseToCompare, pair) ||
                    ContainsTupleInOrder(traverseToCompare, pairReverse))
                {
                    pairsFoundInOtherTraverses += 1;
                    indicesToRemove.Add(currPairs.IndexOf(pair));
                }
            }

            return indicesToRemove;
        }

        private static List<Tuple<PointBase, PointBase>> GetPointPairs(List<PointBase> currTraverse)
        {
            List<Tuple<PointBase, PointBase>> currPairs = new();
            for (int j = 0; j < currTraverse.Count - 1; j++)
            {
                currPairs.Add(new Tuple<PointBase, PointBase>(currTraverse[j], currTraverse[j + 1]));
            }

            return currPairs;
        }

        public static bool ContainsTupleInOrder<T>(List<T> list, Tuple<T, T> tuple)
        {
            for (int i = 0; i < list.Count - 1; i++)
            {
                if (EqualityComparer<T>.Default.Equals(list[i], tuple.Item1) &&
                    EqualityComparer<T>.Default.Equals(list[i + 1], tuple.Item2))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes the supersets in a list containing both closed and contained polygons.
        /// </summary>
        public static List<List<PointBase>> RemoveSupersetsTwoTypes(List<List<PointBase>> traverses)
        {
            bool supersetFound = true;
            while (supersetFound)
            {
                traverses = traverses.OrderByDescending(item => item.Count).ToList();
                traverses = RemoveSupersetTwoTypes(traverses, ref supersetFound);
            }
            return traverses;
        }

        private static List<List<PointBase>> RemoveSupersetTwoTypes(List<List<PointBase>> traverses, ref bool supersetFound)
        {
            supersetFound = false;
            for (int i = 0; i < traverses.Count; i++)
            {
                List<PointBase> traverse = traverses[i];
                for (int j = 0; j < traverses.Count; j++)
                {
                    List<PointBase> traverse2 = traverses[j];
                    if (i == j) { continue; }
                    if (traverse.ToHashSet().SetEquals(traverse2.ToHashSet()))
                    {
                        supersetFound = true;
                        traverses.RemoveAt(i);
                        break;
                    }
                }
                if (supersetFound) { break; }
            }
            return traverses;
        }

        private static TraverseType GettraverseType(List<PointBase> traverse)
        {
            if (traverse[0].Number == traverse[^1].Number)
            {
                return TraverseType.Closed;
            }

            return TraverseType.Linked;
        }

        /// <summary>
        /// Removes the supersets in a list with only closed traverses.
        /// </summary>
        public static List<List<PointBase>> RemoveSupersetsOneType(List<List<PointBase>> traverses)
        {
            bool supersetFound = true;
            while (supersetFound)
            {
                traverses = traverses.OrderByDescending(item => item.Count).ToList();
                traverses = RemoveSupersetOneType(traverses, ref supersetFound);
            }
            return traverses;
        }

        private static List<List<PointBase>> RemoveSupersetOneType(List<List<PointBase>> traverses, ref bool supersetFound)
        {
            supersetFound = false;
            foreach (List<PointBase> traverse in traverses)
            {
                foreach (List<PointBase> traverse2 in traverses)
                {
                    if (traverse == traverse2) { continue; }
                    if (traverse.ToHashSet().IsSupersetOf(traverse2) && CanBeSupersetOf(traverse, traverse2))
                    {
                        supersetFound = true;
                        traverses.Remove(traverse);
                        break;
                    }
                }
                if (supersetFound) { break; }
            }
            return traverses;
        }

        /// <summary>
        /// Checks if traverse and traverse1 are linked traverses that are 
        /// a result of the break down of the same closed traverse.
        /// E.g: 3 4 5 3 -> 4 5; 5 3 4 -> false
        /// </summary>
        private static bool CanBeSupersetOf(List<PointBase> traverse, List<PointBase> traverse1)
        {
            foreach (List<List<PointBase>> resultFromBreakdown in NetworkAnalyzer<RelativeMeasurement>.traversesFromBreakDown)
            {
                List<PointBase> traverseReversed = new(traverse);
                traverseReversed.Reverse();
                List<PointBase> traverse1Reversed = new(traverse1);
                traverse1Reversed.Reverse();

                if ((resultFromBreakdown.Any(item => item.SequenceEqual(traverse)) ||
                    resultFromBreakdown.Any(item => item.SequenceEqual(traverseReversed))) &&
                    (resultFromBreakdown.Any(item => item.SequenceEqual(traverse1)) ||
                    resultFromBreakdown.Any(item => item.SequenceEqual(traverse1Reversed))))
                {// returns false if they were in the same closed traverse
                    return false;
                }
            }
            return true;
        }
    }
}
