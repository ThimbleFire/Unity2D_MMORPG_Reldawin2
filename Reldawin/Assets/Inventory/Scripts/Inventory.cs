using AlwaysEast;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class Inventory : MonoBehaviour, IPickupCursor, IPointerClickHandler
{
    public static bool Dragging = false;
    public static bool MousedOverGearSlot = false;
    public static UIItemOnClick ItemBeingDragged = null;
    public List<RectTransform> availableHoverCells;
    private Vector3 lastPosition = Vector3.zero;
    private Dictionary<Vector2Int, RectTransform> unavailableHoverCells = new Dictionary<Vector2Int, RectTransform>( 12 );
    private List<Vector2Int> hovered = new List<Vector2Int>();
    private List<Vector2Int> occupied = new List<Vector2Int>();
    private Rect[,] slotBounds = new Rect[10, 4];
    private List<Vector2Int> Intersecting {
        get {
            List<Vector2Int> intersecting = new List<Vector2Int>();
            for( int y = 0; y < slotBounds.GetLength( 1 ); y++ ) {
                for( int x = 0; x < slotBounds.GetLength( 0 ); x++ ) {
                    if( ItemBeingDragged.Intersects( slotBounds[x, y] ) ) {
                        intersecting.Add( new Vector2Int( x, y ) );
                    }
                }
            }
            return intersecting;
        }
    }
    private bool AreHoveredOccupied {
        get {
            for( int i = 0; i < occupied.Count; i++ ) {
                if( hovered.Contains( occupied[i] ) ) {
                    return true;
                }
            }
            return false;
        }
    }
    public RectTransform inventoryPosition;
    public CameraFollow cameraFollow;
    public void OnPointerClick( PointerEventData eventData ) {
        if( !Dragging )
            return;
        if( ItemBeingDragged == null )
            return;
        if( hovered.Count < ItemBeingDragged.uiWidth * ItemBeingDragged.uiHeight )
            return;
        if( AreHoveredOccupied == true )
            return;
        Vector2Int topLeft = Intersecting[0];
        //snap the item to the hovered cells
        ItemBeingDragged.Position =
            new Vector2(
                inventoryPosition.position.x + ( ItemBeingDragged.uiWidth * 25 ) + ( topLeft.x * 50 ),
                inventoryPosition.position.y - ( ( ItemBeingDragged.uiHeight * 25 ) + ( topLeft.y * 50 ) ) );
        ItemBeingDragged.transform.SetParent( inventoryPosition );
        AudioDevice.Play( ItemBeingDragged.GetComponent<ItemStats>().soundEndDrag );
        foreach( Vector2Int item in Intersecting ) {
            occupied.Add( item );
        }
        ItemBeingDragged.GetComponent<CanvasGroup>().blocksRaycasts = true;
        ItemBeingDragged = null;
        Dragging = false;
        Cursor.visible = true;
        ReleaseHoverCellAll();
    }
    public void PickupCursor( UIItemOnClick t ) {
        if( Dragging == false ) {
            ItemBeingDragged = t;
            ItemBeingDragged.GetComponent<RectTransform>().SetAsLastSibling();
            ItemBeingDragged.GetComponent<CanvasGroup>().blocksRaycasts = false;
            ItemBeingDragged.transform.SetParent( Inventory.ItemBeingDragged.transform.parent.parent );
            foreach( Vector2Int item in Intersecting ) {
                occupied.Remove( item );
            }
            Cursor.visible = false;
            Dragging = true;
            AudioDevice.PlayGeneric( AudioDevice.Sound.Pickup );
            UpdateHoverCells();
        }
    }
    private void Awake() {
        for( int y = 0; y < slotBounds.GetLength( 1 ); y++ ) {
            for( int x = 0; x < slotBounds.GetLength( 0 ); x++ ) {
                slotBounds[x, y] = new Rect {
                    position = new Vector2( inventoryPosition.position.x + ( x * 50 ), inventoryPosition.position.y - 25 - ( y * 50 ) ),
                    size = new Vector2( 50, 50 )
                };
            }
        }
        gameObject.SetActive( false );
    }
    private void LateUpdate() {
        if( Dragging == false )
            return;
        if( ItemBeingDragged == null )
            return;
        if( Input.mousePosition == lastPosition )
            return;
        ItemBeingDragged.Position = Input.mousePosition;
        ReleaseHoverCellAll();
        if( Intersecting.Count == 0 )
            return;
        if( MousedOverGearSlot )
            return;
        UpdateHoverCells();
    }
    private void UpdateHoverCells() {
        if( Intersecting.Count == 0 )
            return;
        Vector2Int topLeft = Intersecting[0];
        for( int y = 0; y < ItemBeingDragged.uiHeight; y++ ) {
            for( int x = 0; x < ItemBeingDragged.uiWidth; x++ ) {
                if( topLeft.x + x > 9 || topLeft.x + x < 0 ||
                     topLeft.y + y > 3 || topLeft.y + y < 0 )
                    continue;
                if( ItemBeingDragged.Intersects( slotBounds[topLeft.x + x, topLeft.y + y] ) ) {
                    hovered.Add( new Vector2Int( topLeft.x + x, topLeft.y + y ) );
                    AssignHoverCell( new Vector2Int( topLeft.x + x, topLeft.y + y ) );
                }
            }
        }
        lastPosition = Input.mousePosition;
    }
    private void ReleaseHoverCellAll() {
        if( hovered.Count > 0 ) {
            foreach( Vector2Int item in hovered ) {
                ReleaseHoverCell( item );
            }
            hovered.Clear();
        }
    }
    private void ReleaseHoverCell( Vector2Int coordinates ) {
        unavailableHoverCells[coordinates].gameObject.SetActive( false );
        availableHoverCells.Add( unavailableHoverCells[coordinates] );
        unavailableHoverCells.Remove( coordinates );
    }
    private void AssignHoverCell( Vector2Int coordinates ) {
        availableHoverCells[0].gameObject.SetActive( true );
        availableHoverCells[0].position = new Vector2( inventoryPosition.position.x + 25 + ( coordinates.x * 50 ), inventoryPosition.position.y - 25 - ( coordinates.y * 50 ) );
        if( occupied.Contains( coordinates ) )
            availableHoverCells[0].GetComponent<UnityEngine.UI.Image>().color = new Color( 0.2980392f, 0, 0, 0.2f );
        else if( hovered.Contains( coordinates ) )
            availableHoverCells[0].GetComponent<UnityEngine.UI.Image>().color = new Color( 0, 0.2980392f, 0, 0.2f );
        unavailableHoverCells.Add( coordinates, availableHoverCells[0] );
        availableHoverCells.RemoveAt( 0 );
    }
    private void OnDisable() {
        cameraFollow.SetCameraMode( CameraFollow.CameraMode.NoOffset );
        ItemStatBillboard.Hide();
    }
    private void OnEnable() {
        cameraFollow.SetCameraMode( CameraFollow.CameraMode.InventoryOffset );
    }
}
namespace UnityEngine.EventSystems
{
    public interface IPickupCursor : IEventSystemHandler
    {
        void PickupCursor( UIItemOnClick t );
    }
}