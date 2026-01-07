using System;
using KBCore.Refs;
using UnityEngine;

namespace Book.Animation
{
    public class NonePageAnimation : MonoBehaviour, ISwapPageAnimation
    {
        public Action<FlipMode> OnPageSwapped { get; set; }
        
        [SerializeField, Self] private RectTransform bookPanel;
        private RectTransform currentSpawnedPage;
        private Page nextPage;

        public void FlipPage(Page next)
        {
            if(currentSpawnedPage)
                Destroy(currentSpawnedPage.gameObject);
            
            nextPage = next;
            currentSpawnedPage = Instantiate(nextPage, bookPanel).GetComponent<RectTransform>();
        }

        public void SpawnWithoutAnimation(Page next) => FlipPage(next);

        private void OnValidate() => this.ValidateRefs();
    }
}