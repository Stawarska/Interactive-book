using System;
using KBCore.Refs;
using UnityEngine;

namespace Book.Animation
{
    public class NonePageAnimation : MonoBehaviour, ISwapPageAnimation
    {
        public Action<FlipMode> OnPageSwapped { get; set; }
        
        [SerializeField, Self] private RectTransform bookPanel;
        private Page nextPage;

        public void FlipPage(Page next)
        {
            if(nextPage)
                Destroy(nextPage.gameObject);
            
            nextPage = next;
            nextPage.ParentAndFill(bookPanel);
        }

        public void SpawnWithoutAnimation(Page next) => FlipPage(next);

        private void OnValidate() => this.ValidateRefs();
    }
}