using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace DataTables
{
    public class DialogueInfos : ScriptableObject
    {
        public List<DialogueInfo> Values;

        public DialogueInfo Find(int id)
        {
            return Values.FirstOrDefault(p => p.ID == id);
        }
    }
}

