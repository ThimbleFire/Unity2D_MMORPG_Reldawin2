using UnityEngine.EventSystems;
using UnityEngine;

public class UI_PointerEnter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string HoverText;

    public void OnPointerEnter( PointerEventData eventData )
    {
        LowCloud.Reldawin.Tooltip.p.MouseOver( string.Empty, HoverText, string.Empty );
        LowCloud.Reldawin.Tooltip.p.gameObject.SetActive( true );
    }

    public void OnPointerExit( PointerEventData eventData )
    {
        LowCloud.Reldawin.Tooltip.p.gameObject.SetActive( false );
    }
}
