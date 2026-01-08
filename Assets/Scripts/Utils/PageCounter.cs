#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Utils
{
    [InitializeOnLoad]
    public static class PageCounter
    {
        static PageCounter()
        {
            occupiedIndexes.Clear();
            var pages = PrefabNameUtils.GetPrefabNames(Consts.PagePrefabsPath);
            for (var i = 0; i < pages.Count; i++)
            {
                if(!PrefabNameUtils.TryGetIndexFromName(pages[i], out int index))
                    continue;
                
                occupiedIndexes.Add(index);
            }
            occupiedIndexes.Sort();
        }
        
        private static readonly List<int> occupiedIndexes = new();
        private const int startIndex = 1;


        public static int GetAnotherIndex()
        {
            var index = NextIndex();
            if (occupiedIndexes.Count != 0 && occupiedIndexes.Count - 1 != occupiedIndexes[^1] - startIndex)
                index = FindMissingIndex();
            
            if(occupiedIndexes.Contains(index))
                Debug.LogError("Something go wrong. Indexes can't be duplicated. Fix it.");
            else
                occupiedIndexes.Add(index);
            occupiedIndexes.Sort();
            
            return index;
        }

        public static void ReturnIndexToPool(int returnedIndex)
        {
            occupiedIndexes.Remove(returnedIndex);
        }

        private static int FindMissingIndex()
        {
            var begin = 0;
            var end = occupiedIndexes.Count - 1;

            if (occupiedIndexes[0] != startIndex)
                return startIndex;
            
            while (true)
            {
                var middle = (begin + end) / 2;
                
                if (occupiedIndexes[middle] - startIndex != middle)
                {
                    if(end == middle)
                        break;
                    end = middle;
                }

                else
                {
                    if(begin == middle)
                        break;
                    begin = middle;
                }
            }
            
            return occupiedIndexes[begin] + 1;
        }
        
        private static int NextIndex()
        {
            return occupiedIndexes.Count + startIndex;
        }
    }
}
#endif