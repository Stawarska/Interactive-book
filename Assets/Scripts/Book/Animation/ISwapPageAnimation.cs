using System;

namespace Book.Animation
{
    public interface ISwapPageAnimation
    {
        Action<FlipMode> OnPageSwapped { get; set; }
        void FlipPage(Page next);

    }
}
