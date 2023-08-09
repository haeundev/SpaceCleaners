using UnityEngine;
using System.Collections.Generic;

namespace DataTables
{
    public class TaskInfos : ScriptableObject
    {
        public List<TaskInfo> Values;
        
        public TaskInfo Find(int id)
        {
            return Values.Find(p => p.ID == id);
        }
    }
}

