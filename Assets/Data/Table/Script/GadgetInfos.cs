using UnityEngine;
using System.Collections.Generic;

namespace DataTables
{
    public class GadgetInfos : ScriptableObject
    {
        public List<GadgetInfo> Values;
        
        public GadgetInfo Find(int id)
        {
            return Values.Find(p => p.ID == id);
        }
    }
}