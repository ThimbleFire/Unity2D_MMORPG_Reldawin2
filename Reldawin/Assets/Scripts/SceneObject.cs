using UnityEngine;

namespace AlwaysEast
{
    public class SceneObjectData
    { public int x, y, Type; }

    public class SceneObject : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer renderer;

        private SceneObjectData data;

        public void Setup( SceneObjectData _data ) {
            this.data = _data;
            Sprite sprite = ResourceRepository.GetSprite( data.Type );
            renderer.sprite = sprite;
            gameObject.SetActive( true );
            gameObject.name = sprite.name;
        }
    }
}