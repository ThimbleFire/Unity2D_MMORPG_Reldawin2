using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Container_DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public RectTransform rTransform;

    public void OnBeginDrag( PointerEventData eventData )
    {
    }

    public void OnDrag( PointerEventData eventData )
    {
        rTransform.position = Input.mousePosition + new Vector3( rTransform.rect.size.x / 2, -rTransform.rect.size.y );
    }

    public void OnEndDrag( PointerEventData eventData )
    {

    }
}