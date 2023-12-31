using System;
using System.Collections.Generic;
using UnityEngine;

namespace LiveLarson.Util
{
    [Serializable]
    public class TransformList : WrapperList<Transform>
    {
        public TransformList() { }
        public TransformList(List<Transform> list) : base(list) { }
    }
}