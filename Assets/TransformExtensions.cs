using UnityEngine;

namespace ZiumperExtensions
{
    public static class TransformExtensions
    {
        public static void MoveTowards(this Transform origin, Transform target, float duration, float elapsedTime, Vector3 initialPosition)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            origin.position = Vector3.Lerp(initialPosition, target.position, t);

            if (t >= 1f)
            {
                Debug.Log("Obiekt dotarł do pozycji docelowej.");
            }
        }
    }

}
