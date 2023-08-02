using System;
using Unity.VisualScripting;
using UnityEngine;
public class ItemStatBillboard : MonoBehaviour
{
    private static UnityEngine.UI.Image image;
    private static TMPro.TMP_Text textBody;
    private static UnityEngine.RectTransform rTransform;

    public static void Draw( ItemStats item ) {
        if( Inventory.Dragging )
            return;
        if( item == null ) {
            Hide();
            return;
        }
        image.enabled = true;
        textBody.enabled = true;
        textBody.text = item.Tooltip;
        Canvas.ForceUpdateCanvases();

        // offset billboard position from the item's central pivot to the top of the item
        float itemSpriteHalfHeight = item.GetComponent<RectTransform>().sizeDelta.y / 2;
        rTransform.position = item.transform.position + Vector3.up * itemSpriteHalfHeight; //75
        //rTransform.position = 447, 202. textBody.preferred height = 309.38,
        bool billboardExceedsTopOfScreen = rTransform.position.y + textBody.preferredHeight > Screen.height;
        bool billboardExceedsRightOfScreen = rTransform.position.x + textBody.preferredWidth / 2 > Screen.width;
        if( billboardExceedsTopOfScreen ) {
            rTransform.position += Vector3.up * ( Screen.height - ( rTransform.position.y + textBody.preferredHeight ) );
        }
        if( billboardExceedsRightOfScreen ) {
            rTransform.position += Vector3.right * ( Screen.width - ( rTransform.position.x + textBody.preferredWidth / 2 ) );
        }
    }
    public static void Hide() {
        image.enabled = false;
        textBody.enabled = false;
    }
    private void Awake() {
        image = GetComponent<UnityEngine.UI.Image>();
        textBody = transform.GetComponentInChildren<TMPro.TMP_Text>();
        rTransform = GetComponent<RectTransform>();
    }
}