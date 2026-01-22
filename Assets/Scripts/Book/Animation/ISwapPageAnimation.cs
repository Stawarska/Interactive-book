using System;

namespace Book.Animation
{
    public interface ISwapPageAnimation
    {
        Action<FlipMode> OnPageSwapped { get; set; }
        //TODO: add possibility to choice if flip right or left 
        
        /// <summary>
        /// Start animation to flip page
        /// </summary>
        /// <param name="next">clone of next page, has to be destroyed by animation after use</param>
        void FlipPage(Page next);
        void SpawnWithoutAnimation(Page next);
    }
}
