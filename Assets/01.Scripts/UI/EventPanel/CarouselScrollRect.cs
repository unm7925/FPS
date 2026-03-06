using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class CarouselScrollRect:ScrollRect
{
    public Action<PointerEventData> onEndDrag;
    public Action<PointerEventData> onBeginDrag;
    
    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        
        onEndDrag?.Invoke(eventData);
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        
        onBeginDrag?.Invoke(eventData);
    }
}
