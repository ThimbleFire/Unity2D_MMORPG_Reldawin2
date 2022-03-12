using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Inventory : MonoBehaviour, IHasChanged
{
    [SerializeField]
    Transform slots;

    private void Start()
    {
        HasChanged();
    }

    public void HasChanged()
    {
        //foreach ( Transform slotTransform in slots )
        //{
        //    GameObject item = slotTransform.GetComponent<UI_Slot>().item;

        //    if ( item )
        //    {
        //        // do something
        //    }
        //}
    }

    public void ToggleActive( Button btn )
    {
        gameObject.SetActive( !gameObject.activeSelf );

        btn.image.sprite = gameObject.activeSelf ? LowCloud.Reldawin.SpriteLoader.GetDoodad( "btnPressed" ) :
                                                   LowCloud.Reldawin.SpriteLoader.GetDoodad( "btnNotPressed" );
    }
}

namespace UnityEngine.EventSystems
{
    public interface IHasChanged : IEventSystemHandler
    {
        void HasChanged();
    }
}