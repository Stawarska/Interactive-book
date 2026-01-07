using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Book.Animation
{
    public class DissolvePageAnimation : MonoBehaviour, ISwapPageAnimation
    {
        public Action<FlipMode> OnPageSwapped { get; set; }
        [SerializeField] private RectTransform prefabParent;
        [SerializeField] private float pageFlipTime;
        [SerializeField] private Image pageImage;
        private RectTransform currentSpawnedPage;
        private Page nextPage;

        public void FlipPage(Page next)
        {
            pageImage.gameObject.SetActive(true);
            PrepareToFlip(next);
            StartCoroutine(FadeImageAlpha(pageImage, 0f, pageFlipTime));
        }

        public void SpawnWithoutAnimation(Page next)
        {
            pageImage.gameObject.SetActive(false);
            PrepareToFlip(next);
        }

        private void PrepareToFlip(Page page)
        {
            if(nextPage != null)
                pageImage.sprite = GeneratePreview.GeneratePageSprite(nextPage.gameObject, GeneratePreview.PageSection.Full);
            
            if(currentSpawnedPage)
                Destroy(currentSpawnedPage.gameObject);
            
            nextPage = page;
            currentSpawnedPage = Instantiate(nextPage, prefabParent).GetComponent<RectTransform>();
        }

        private IEnumerator FadeImageAlpha(Image image, float targetAlpha, float duration)
        {
            if (image == null)
                yield break;

            var startColor = image.color;
            var startAlpha = startColor.a;
            var elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                var t = Mathf.Clamp01(elapsedTime / duration);
        
                var newColor = startColor;
                newColor.a = Mathf.Lerp(startAlpha, targetAlpha, t);
                image.color = newColor;

                yield return null;
            }

            var finalColor = image.color;
            finalColor.a = targetAlpha;
            image.color = finalColor;
            
            pageImage.gameObject.SetActive(false);
            
            pageImage.color = startColor;
        }
    }
}