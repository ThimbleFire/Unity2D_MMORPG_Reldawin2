using UnityEngine;

using UnityEngine.UI;

namespace LowCloud.Reldawin
{
    public class Tooltip : MonoBehaviour
    {
        public static Tooltip p;
        public Vector3 offset;

        private void Awake()
        {
            p = this;
            offset = Vector3.zero;
        }

        public Text textComponent;
        public Image imageComponent;
        public RectTransform rectTransform;

        public void MouseOver( string action, string name, string color)
        {
            int width = CalculateCharacterWidth( action, name );

            imageComponent.rectTransform.sizeDelta = new Vector2( width, 20 );
            textComponent.text = string.Format( "{0} {1}{2}{3}", 
                action, 
                color, 
                name, 
                color == string.Empty ? string.Empty : "</color>" );

            offset = new Vector2( width / 2, 12 );

            imageComponent.enabled = true;
            textComponent.enabled = true;
        }

        private int CalculateCharacterWidth(string action, string name)
        {
            int characterWidth = 12;
            string message = string.Format( "{0} {1}", action, name );
            for ( int i = 0; i < message.Length; i++ )
            {
                textComponent.font.GetCharacterInfo( message[i], out CharacterInfo info, textComponent.fontSize );
                characterWidth += info.advance;
            }
            return characterWidth;
        }

        public void MouseOff()
        {
            textComponent.text = string.Empty;

            imageComponent.enabled = false;
            textComponent.enabled = false;
        }

        private void Update()
        {
            transform.position = Input.mousePosition + offset;
        }
    }
}