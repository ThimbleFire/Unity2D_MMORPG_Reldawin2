using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class GearSlot : MonoBehaviour, IPointerClickHandler, IPickupCursor, IPointerEnterHandler, IPointerExitHandler
{
    public ItemStats.Type type;
    public GameObject character;
    public void OnPointerClick( PointerEventData eventData ) {
        if( Inventory.Dragging ) {
            ItemStats stats = Inventory.ItemBeingDragged.GetComponent<ItemStats>();
            if( stats.type != type )
                return;
            stats.transform.SetParent( transform );
            AudioDevice.Play( stats.soundEndDrag );
            ExecuteEvents.ExecuteHierarchy<IEquipped>( character, null, ( x, y ) => x.GearEquipped( stats, true ) );
            Inventory.ItemBeingDragged.GetComponent<CanvasGroup>().blocksRaycasts = true;
            Inventory.ItemBeingDragged = null;
            Inventory.Dragging = false;
            Cursor.visible = true;
            Unhighlight();
        }
    }
    public void PickupCursor( UIItemOnClick t ) {
        if( Inventory.Dragging == false ) {
            Inventory.ItemBeingDragged = t;
            Inventory.ItemBeingDragged.GetComponent<CanvasGroup>().blocksRaycasts = false;
            Inventory.ItemBeingDragged.transform.SetParent( Inventory.ItemBeingDragged.transform.parent.parent.parent );
            Inventory.ItemBeingDragged.GetComponent<RectTransform>().SetAsLastSibling();
            Inventory.Dragging = true;
            Cursor.visible = false;
            AudioDevice.PlayGeneric( AudioDevice.Sound.Pickup );
            ExecuteEvents.ExecuteHierarchy<IEquipped>( character, null, ( x, y ) => x.GearEquipped( Inventory.ItemBeingDragged.GetComponent<ItemStats>(), false ) );
        }
    }
    public void OnPointerEnter( PointerEventData pointerEventData ) {
        if( Inventory.Dragging ) {
            Inventory.MousedOverGearSlot = true;
            if( Inventory.ItemBeingDragged.GetComponent<ItemStats>().type != type ) {
                HighlightRed();
            }
            else {
                HighlightGreen();
            }
        }
    }
    public void OnPointerExit( PointerEventData pointerEventData ) {
        Inventory.MousedOverGearSlot = false;
        Unhighlight();
    }
    private void HighlightGreen() {
        GetComponent<Image>().color = new Color( 0, 1.0f, 0, 0.2f );
    }
    private void HighlightRed() {
        GetComponent<Image>().color = new Color( 1.0f, 0, 0, 0.2f );
    }
    private void Unhighlight() {
        GetComponent<Image>().color = new Color( 0, 0.0f, 0, 0.0f );
    }
}