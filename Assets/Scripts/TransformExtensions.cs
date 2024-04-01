using UnityEngine;

namespace ExtensionsUtil
{
    public static class TransformExtensions
    {
        public static float MoveTowards(this Transform origin, Transform target, float duration, float elapsedTime, Vector3 initialPosition)
        {
            elapsedTime += Time.deltaTime;
            float part = Mathf.Clamp01(elapsedTime / duration);
            origin.position = Vector3.Lerp(initialPosition, target.position, part);

            if (part >= 1f)
            {
                Debug.Log("Obiekt dotarł do pozycji docelowej.");
            }

            return elapsedTime;
        }

        

    }

}



