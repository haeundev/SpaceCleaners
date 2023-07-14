using UnityEngine;

namespace Kataner
{
    public struct ViewCastInfo
    {
        public Transform transform;
        public Vector3 point;
        public float dst;
        public float angle;

        public ViewCastInfo(Transform _transform, Vector3 _point, float _dst, float _angle)
        {
            transform = _transform;
            point = _point;
            dst = _dst;
            angle = _angle;
        }
    }
}
