// Generated by Data Class Generator

using System;
using System.Collections.Generic;
using LiveLarson.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;
using UnityEngine.Serialization;

namespace DataTables
{
    [Serializable]
    public class TaskInfo
    {
        public int ID;
        [JsonConverter(typeof(StringEnumConverter))]
        public TaskType TaskType;
        public string Title;
        public string ValueStr;
        public int ValueInt;
        public string StartAction;
        public string EndAction;
        public string CompleteCondition;
    }

}

