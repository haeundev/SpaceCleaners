using System.Collections;
using UnityEngine;

namespace DaiMangou.ProRadarBuilder
{
    /// <summary>
    ///     an example script of how you cn use the lockon "Finction Trigger" Locon condition
    ///     we are simply goingto assign a value to the "ObjectToLockonTo" value inside of the LockonManager.cs code which is
    ///     attached to our radar.
    ///     in this particular sctipt we are checking is a bool named "playerAimedAtMe" is true, if it is , we will assign this
    ///     gameobject to "ObjectToLockonTo", else we will set "ObjectToLockonTo" to null
    /// </summary>
    public class MonitorCharacter : MonoBehaviour
    {
        public LockonManager lockonManager;

        public bool playerAimedAtMe;

        private IEnumerator Start()
        {
            while (true)
            {
                while (PlayerHasPointedHisGunAtMe()) yield return StartCoroutine(UnderThreat());

                yield return null;
            }
        }

        public bool PlayerHasPointedHisGunAtMe()
        {
            if (playerAimedAtMe)
            {
                lockonManager.ObjectToLockonTo = gameObject;
                return true;
            }

            if (lockonManager.ObjectToLockonTo == gameObject)
                lockonManager.ObjectToLockonTo = null;

            return false;
        }

        private IEnumerator UnderThreat()
        {
            yield return null;
        }
    }
}