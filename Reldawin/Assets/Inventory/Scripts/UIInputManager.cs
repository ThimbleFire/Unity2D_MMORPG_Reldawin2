using UnityEngine;

public class UIInputManager : MonoBehaviour
{
    public GameObject inventoryWindow;
    public GameObject characterWindow;

    private void Update() {
        if( Input.GetKeyDown( KeyCode.I ) ) {
            AudioDevice.PlayGeneric( AudioDevice.Sound.WindowOpen );
            inventoryWindow.SetActive( !inventoryWindow.activeInHierarchy );
        }

        if( Input.GetKeyDown( KeyCode.C ) ) {
            AudioDevice.PlayGeneric( AudioDevice.Sound.WindowOpen );
            if( characterWindow != null ) {
                characterWindow.SetActive( !characterWindow.activeInHierarchy );
            }
        }
    }
}