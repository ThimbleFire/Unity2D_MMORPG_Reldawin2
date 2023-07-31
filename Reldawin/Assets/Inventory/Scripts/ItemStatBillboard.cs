using System.Collections;
using System.Collections.Generic;
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

        RectTransform itemTransform = item.GetComponent<RectTransform>();

        Vector3 position = itemTransform.position -
          Vector3.down * textBody.preferredHeight + Vector3.down * ( itemTransform.sizeDelta.y / 2 );

        rTransform.position = position;

        if( position.y + textBody.preferredHeight > 800 )
            position += Vector3.down * textBody.preferredHeight / 2;

        rTransform.position = position;
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