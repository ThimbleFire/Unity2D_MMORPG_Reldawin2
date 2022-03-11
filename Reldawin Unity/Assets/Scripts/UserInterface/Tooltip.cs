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

        // text.text = "Gather <color=#07F8E0>" + resource.ToString() + "</color>";

        public Text textComponent;

        public void MouseOver(string text)
        {
            GetComponent<Image>().enabled = true;
            textComponent.enabled = true;

            int characterWidth = 12;

            for ( int i = 0; i < text.Length; i++ )
            {
                textComponent.font.GetCharacterInfo( text[i], out CharacterInfo info, textComponent.fontSize );
                characterWidth += info.advance;
            }

            GetComponent<Image>().rectTransform.sizeDelta = new Vector2( characterWidth, 24 );            
            textComponent.text = text;

            offset = new Vector2( characterWidth / 2, 12 );
        }

        public void MouseOff()
        {
            GetComponent<Image>().enabled = false;
            textComponent.enabled = false;

            textComponent.text = string.Empty;
        }

        private void Update()
        {
            transform.position = Input.mousePosition + offset;
        }
    }
}