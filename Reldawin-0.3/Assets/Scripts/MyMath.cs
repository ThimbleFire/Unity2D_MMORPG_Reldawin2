using UnityEngine;

namespace AlwaysEast
{
    public class MyMath
    {
        public static Vector2Int ClampVector2Int( Vector2Int toClamp, int xMin, int xMax, int yMin, int yMax )
        {
            toClamp.x = Mathf.Clamp( toClamp.x, xMin, xMax );
            toClamp.y = Mathf.Clamp( toClamp.y, yMin, yMax );

            return toClamp;
        }
        public static Vector2 ClampVector2( Vector2 toClamp, float xMin, float xMax, float yMin, float yMax )
        {
            toClamp.x = Mathf.Clamp( toClamp.x, xMin, xMax );
            toClamp.y = Mathf.Clamp( toClamp.y, yMin, yMax );

            return toClamp;
        }
    }
}