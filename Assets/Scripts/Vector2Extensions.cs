using UnityEngine;

namespace ExtensionsUtil
{
    public static class Vector2Extensions
    {
        public static Vector2 GetOpposite(this Vector2 vector)
        {
            return new Vector2(vector.x, vector.y) * -1;
        }
    }

}



