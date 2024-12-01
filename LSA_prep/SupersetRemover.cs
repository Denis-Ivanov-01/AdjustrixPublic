namespace LSA_Base
{

    public enum TraverseType
    {
        Closed=0,
        Linked=1
    }

    internal static class SupersetRemover
    { //todo: think of dividing this into 3 different classes - one for each type of superset removal
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
        { // todo: refactor this
            List<Tuple<PointBase, PointBase>> currPairs = new();
            for (int j = 0; j < currTraverse.Count - 1; j++)
            {
                currPairs.Add(new Tuple<PointBase, PointBase>(currTraverse[j], currTraverse[j + 1]));
            }
            int initialPairsCount = currPairs.Count;
            int pairsFoundInOtherTraverses = 0;

            //todo: double check if this needs to be commented - most likely yes. If so, remove the fucker
            //int index1 = traversesSorted.Where(p => p.Count > currTraverse.Count).Count();
            //int currtraverseIndex = traversesSorted.IndexOf(currTraverse);
            //traversesSorted.RemoveAt(currtraverseIndex);
            //int targetIndex = traversesSorted.Where(p => p.Count > currTraverse.Count).Count();
            //// I have no fucking idea why the following line works, but it does...
            //// TODO: Figure out why the fuck I need to do this so that the algorithm doesn't remove
            //// traverses that shouldn't be removed.
            //// Notes: This is the last time in this method where traversesSorted is used.
            //// I don't understand why it changes the logic :/
            //traversesSorted.Insert(targetIndex, currTraverse);
            //for (int i = targetIndex + 1; i < traversesToCompare.Count; i++)

            for (int i = 0; i < traversesToCompare.Count; i++)
            {
                if (i >= traversesToCompare.Count) { break; } //invalid index
                List<PointBase> traverseToCompare = traversesToCompare[i];

                //if (linkedTraversesFromBreakdown.Contains(traverseToCompare)) { continue; }

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

                foreach(int index in indicesToRemove.OrderByDescending(x => x))
                {
                    currPairs.RemoveAt(index);
                }
            }
            return pairsFoundInOtherTraverses == initialPairsCount; //True means that all pairs exist in other traverses
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
            //foreach (List<PointBase> traverse in traverses)
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
            if (traverse[0].Number == traverse[^1].Number) return TraverseType.Closed;
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
            foreach(List<PointBase> traverse in traverses)
            {
                foreach(List<PointBase> traverse2 in traverses)
                {
                    if (traverse == traverse2) { continue; }
                    //if (traverse.ToHashSet().IsSupersetOf(traverse2))
                    // TEST!
                    //if (IsSupersetOneType(traverse, traverse2))
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

        //private static bool IsSupersetOneType(List<PointBase> traverse, List<PointBase> traverse1)
        //{
        //    for (int i = 0; i < traverse.Count - 1; i++)
        //    {
        //        PointBase p1 = traverse[i];
        //        PointBase p2 = traverse[i + 1];
        //        List<PointBase> straightPair = new() { p1, p2 };
        //        List<PointBase> reversePair = new() { p2, p1 };
        //        for (int j = 0; j < traverse1.Count; j++)
        //        {
        //            PointBase point1 = traverse1[j];
        //            PointBase point2 = traverse1[j + 1];
        //            List<PointBase> pair = new() { point1, point2 };
        //            //List<PointBase> reverse = new() { point2, point1 };
        //            if (!((straightPair[0] == pair[0] && straightPair[1] == pair[1]) ||
        //                (reversePair[0] == pair[0] && reversePair[1] == pair[1])))
        //            {
        //                return false;
        //            }
        //        }
        //    }
        //    return true;
        ////}
        
        /// <summary>
        /// Checks if traverse and traverse1 are linked traverses that are 
        /// a result of the break down of the same closed traverse.
        /// E.g: 3 4 5 3 -> 4 5; 5 3 4
        /// </summary>
        /// <param name="traverse"></param>
        /// <param name="traverse1"></param>
        /// <returns></returns>
        private static bool CanBeSupersetOf(List<PointBase> traverse, List<PointBase> traverse1)
        { //TODO: Rename this
            foreach(List<List<PointBase>> resultFromBreakdown in NetworkAnalyzer<RelativeMeasurement>.traversesFromBreakDown)
            {
                if (resultFromBreakdown.Contains(traverse) && resultFromBreakdown.Contains(traverse1))
                {// returns false if they were in the same closed traverse
                    return false;
                }
            }
            return true;
        }
    }
}
