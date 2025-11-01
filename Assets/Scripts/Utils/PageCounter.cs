using System.Collections.Generic;

namespace Utils
{
    public static class PageCounter
    {
        private static int currentIndex;
        private static Stack<int> returnedIndexes = new();

        public static int GetAnotherIndex()
        {
            if (returnedIndexes.Count == 0)
                return currentIndex += 1;
            return returnedIndexes.Pop();
        }

        public static void ReturnIndexToPool(int returnedIndex)
        {
            returnedIndexes.Push(returnedIndex);
        }
    }
}