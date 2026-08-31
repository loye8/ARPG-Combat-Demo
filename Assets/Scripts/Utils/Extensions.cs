using UnityEngine;

namespace ARPGCombat.Utils
{
    public static class Extensions
    {
        public static Vector3 Flatten(this Vector3 v)
        {
            return new Vector3(v.x, 0, v.z);
        }

        public static float HorizontalDistanceTo(this Vector3 from, Vector3 to)
        {
            Vector3 fromFlat = new Vector3(from.x, 0f, from.z);
            Vector3 toFlat = new Vector3(to.x, 0f, to.z);
            return Vector3.Distance(fromFlat, toFlat);
        }

        public static Vector3 ScreenPointToGroundPlane(this Camera cam, Vector3 screenPos)
        {
            Ray ray = cam.ScreenPointToRay(screenPos);

            float t= -ray.origin.y / ray.direction.y;

            if(t < 0)
            {
                return Vector3.zero;
            }

            return ray.GetPoint(t);
        }
    }
}
