using System.Collections.Generic;
using UnityEngine;

namespace LiveLarson.UISystem
{
    [CreateAssetMenu(fileName = "UIContainer", menuName = "ScriptableObject/UIContainer")]
    public class UIContainer : ScriptableObject
    {
        public List<UIKeyValue> uis;
    }
}