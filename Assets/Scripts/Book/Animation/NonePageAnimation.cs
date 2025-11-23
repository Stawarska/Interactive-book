using System;
using KBCore.Refs;
using UnityEngine;

namespace Book.Animation
{
    public class NonePageAnimation : MonoBehaviour, ISwapPageAnimation
    {
        public Action<FlipMode> OnPageSwapped { get; set; }
        
        [SerializeField, Self] private RectTransform bookPanel;
        [SerializeField] private float pageFlipTime;
        private RectTransform currentSpawnedPage;
        private Page nextPage;
        
        public void FlipRightPage()
        {
            
        }

        public void FlipLeftPage()
        {
            
        }

        public void FlipPage(Page next)
        {
            if(currentSpawnedPage)
                Destroy(currentSpawnedPage.gameObject);
            
            nextPage = next;
            currentSpawnedPage = Instantiate(nextPage, bookPanel).GetComponent<RectTransform>();
        }

        private void OnValidate() => this.ValidateRefs();
    }
}