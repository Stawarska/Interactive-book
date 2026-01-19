using UnityEngine;

namespace Utils
{
    public class AspectRatioEnforcer : MonoBehaviour
    {
        [SerializeField] private Vector2 aspectRatio;
        
        private Vector2Int prevScreenSize;
        private float aspect;
        
        private void Awake()
        {
            aspect = aspectRatio.y / aspectRatio.x;
            SetAspectRatio();
        }

        private void Update()
        {
            if (prevScreenSize.x != Screen.width || prevScreenSize.y != Screen.height)
                SetAspectRatio();
        }

        private void SetAspectRatio()
        {
            Screen.SetResolution(Mathf.RoundToInt(Screen.height / aspect), Screen.height, Screen.fullScreenMode);
      
            prevScreenSize = new Vector2Int(Screen.width, Screen.height);
        }
    }
}