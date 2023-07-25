﻿using System;
using UnityEngine;

//Original version of the ConditionalHideAttribute created by Brecht Lecluyse (www.brechtos.com)
//Modified by: Sebastian Lague

namespace ProceduralPlanets
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
                    AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
    public class ConditionalHideAttribute : PropertyAttribute
    {
        public string ConditionalSourceField;
        public int EnumIndex;

        public ConditionalHideAttribute(string boolVariableName)
        {
            ConditionalSourceField = boolVariableName;
        }

        public ConditionalHideAttribute(string enumVariableName, int enumIndex)
        {
            ConditionalSourceField = enumVariableName;
            EnumIndex = enumIndex;
        }

    }
}



