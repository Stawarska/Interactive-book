using System;

namespace Book.Animation
{
    public interface ISwapPageAnimation
    {
        Action<FlipMode> OnPageSwapped { get; set; }
        void FlipRightPage();
        void FlipLeftPage();
        void FlipPage(Page next);

    }
}
