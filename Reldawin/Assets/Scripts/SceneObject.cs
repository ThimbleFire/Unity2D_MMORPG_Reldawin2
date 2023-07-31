using AlwaysEast;
using UnityEngine;

namespace AlwaysEast
{
    public class SceneObjectData
    {
        public int x, y, Type;
    }

    public class SceneObject : MonoBehaviour
    {
        [SerializeField] 
        private new SpriteRenderer renderer;

        public void Setup( Sprite sprite ) {

            // setup sprite
            renderer.sprite = sprite;

            // enable the game object
            gameObject.SetActive( true );

            gameObject.name = sprite.name;
        }
    }
}
