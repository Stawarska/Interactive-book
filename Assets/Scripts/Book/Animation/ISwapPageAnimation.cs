using System;

namespace Book.Animation
{
    public interface ISwapPageAnimation
    {
        Action<FlipMode> OnPageSwapped { get; set; }
        //TODO: add possibility to choice if flip right or left 
        void FlipPage(Page next);
        void SpawnWithoutAnimation(Page next);
    }
}
