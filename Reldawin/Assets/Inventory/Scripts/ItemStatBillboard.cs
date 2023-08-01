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
        //set billboard position
        rTransform.position = item.transform.position + Vector3.up * ( item.GetComponent<RectTransform>().sizeDelta.y / 2 );
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