using System;
using UnityEngine;

namespace Book.Animation
{
    public class DissolvePageAnimation : MonoBehaviour, ISwapPageAnimation
    {
        public Action<FlipMode> OnPageSwapped { get; set; }
        public void FlipRightPage()
        {
            throw new NotImplementedException();
        }

        public void FlipLeftPage()
        {
            throw new NotImplementedException();
        }

        public void FlipPage(Page next)
        {
            throw new NotImplementedException();
        }
    }
}