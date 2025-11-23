using System.Collections.Generic;
using Book.Animation;
using Graph;
using KBCore.Refs;
using UnityEngine;

namespace Book
{
    public class BookManager : MonoBehaviour
    {
        public static BookManager Instance;
        private Page currentPage;
        private Page targetPage;
        private RectTransform currentSpawnedPage;
        //[field: SerializeReference, SubclassSelector] public ISwapPageAnimation PageAnimation { get; private set; }
        [SerializeField] private FlipPageAnimation PageAnimation;
        [SerializeField] private BookGraph bookGraph;
        
        private Stack<Page> pagesHistory = new();

        public void Awake()
        {
            if (Instance == null)
                Instance = this;
            SelectNextPage(bookGraph.GetFirstPage());
        }

        public void SelectNextPage(Page page)
        {
            if(targetPage != null)
                pagesHistory.Push(targetPage);
            SelectPage(page);
        }
    
        public void SelectPreviousPage()
        {
            if (pagesHistory == null || pagesHistory.Count == 0)
                return;
    
            var nextPage = pagesHistory.Pop();
        
            SelectPage(nextPage);
        }
    
        private void SelectPage(Page page)
        {
            if(currentSpawnedPage)
                Destroy(currentSpawnedPage.gameObject);
    
            // bool moveRight = currentPage == null || targetPage == null || targetPage.PageId < page.PageId;
        
            currentPage = targetPage;
            targetPage = page;
        
            PageAnimation.FlipPage(page);
        
            // if(moveRight)
            //     PageAnimation.FlipRightPage();
            // else
            //     PageAnimation.FlipLeftPage();
        }
    }
}
