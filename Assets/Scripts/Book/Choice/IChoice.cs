using System;
using UnityEngine.EventSystems;

namespace Book
{
    public interface IChoice : IPointerEnterHandler, IPointerExitHandler
    {
        event Action OnChoicePicked;
    }
}