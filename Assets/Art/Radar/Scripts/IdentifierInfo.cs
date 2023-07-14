using UnityEngine;

namespace Kataner
{
    public struct IdentifierInfo
    {
        public GameObject identifier;
        public GameObject originObject;
        public float lastKnownActive;

        public IdentifierInfo(GameObject _identifier, GameObject _originObject)
        {
            lastKnownActive = Time.time;
            identifier = _identifier;
            originObject = _originObject;
        }
    }
}
