using System;

namespace Book
{
    public interface IChoice
    {
        event Action OnChoicePicked;
    }
}