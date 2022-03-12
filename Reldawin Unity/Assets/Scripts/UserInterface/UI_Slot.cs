using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Slot : MonoBehaviour, IDropHandler
{
    public GameObject item
    {
        get
        {
            if ( transform.childCount > 0 )
                return transform.GetChild( 0 ).gameObject;
            return null;
        }
    }

    public bool IsEmpty { get { return transform.childCount == 0; } }
    public LowCloud.Reldawin.Item GetItem { get { return GetComponentInChildren<LowCloud.Reldawin.Item>(); } }

    public void OnDrop( PointerEventData eventData )
    {
        if ( !item )
        {
            UI_Item_DragHandler.itemBeingDragged.transform.SetParent( transform );
            ExecuteEvents.ExecuteHierarchy<IHasChanged>( gameObject, null, ( x, y ) => x.HasChanged() );
        }
    }
}