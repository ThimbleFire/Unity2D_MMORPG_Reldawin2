using UnityEngine;
using UnityEngine.UI;

namespace LowCloud.Reldawin
{
    public class GameplayWindowControls : MonoBehaviour
    {
        public Sprite active;
        public Sprite inactive;

        public void ToggleWindow( Image image )
        {
            switch ( image.sprite.name )
            {
                // Inactive
                case "icon_selected_0":
                    image.sprite = active;
                    break;

                // Active
                case "icon_selected_1":
                    image.sprite = inactive;
                    break;
            }

            switch ( image.gameObject.name )
            {
                case "Inventory":

                    //inventory.gameObject.SetActive( !inventory.gameObject.activeInHierarchy );
                    break;

                case "Gear":

                    //equipment.gameObject.SetActive( !equipment.gameObject.activeInHierarchy );
                    break;

                case "Crafting":

                    //crafting.gameObject.SetActive( !crafting.gameObject.activeInHierarchy );
                    break;
            }
        }
    }
}