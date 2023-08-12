// Generated by Data Class Generator

using System;
using System.Collections.Generic;
using LiveLarson.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace DataTables
{
    [Serializable]
    public class DialogueInfo
    {
        public int ID;
        [JsonConverter(typeof(StringEnumConverter))]
        public SpeakerType SpeakerType;
        public string DisplayName;
        public string Line;
        public int Next;
        public bool HasChoice;
        public string ResponseA;
        public int NextForA;
        public string ResponseB;
        public int NextForB;
        public string AudioPath;
        public string ImagePath;
    }

}

