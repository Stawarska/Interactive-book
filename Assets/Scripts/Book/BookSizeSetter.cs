using UnityEngine;
namespace Book
{
    [ExecuteInEditMode]
    public class BookSizeSetter : MonoBehaviour
    {
        private RectTransform rectTransform;
        
        public float Width => rectTransform.rect.width;
        public float Height => rectTransform.rect.height;
        
        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }
    }
}