using UnityEngine;
using UnityEngine.EventSystems;
public class UIItemOnClick : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public byte uiWidth;
    public byte uiHeight;
    public Rect GetRect {
        get {
            RectTransform rectTransform = GetComponent<UnityEngine.UI.Image>().rectTransform;
            return new Rect( (Vector2)rectTransform.position - ( rectTransform.sizeDelta / 2 ) + rectTransform.sizeDelta * 0.15f, rectTransform.sizeDelta * 0.7f );
        }
    }
    public bool Intersects( Rect r ) {
        if( GetRect.Overlaps( r ) )
            return true;
        else
            return false;
    }
    public void OnPointerClick( PointerEventData eventData ) {
        ExecuteEvents.ExecuteHierarchy<IPickupCursor>( gameObject, null, ( x, y ) => x.PickupCursor( this ) );
    }
    public void OnPointerEnter( PointerEventData eventData ) {
        ItemStatBillboard.Draw( GetComponent<ItemStats>() );
    }
    public void OnPointerExit( PointerEventData eventData ) {
        ItemStatBillboard.Hide();
    }
}