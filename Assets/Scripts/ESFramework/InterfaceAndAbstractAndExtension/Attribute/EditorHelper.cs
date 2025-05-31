using ES;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{

    public class EditorHelper 
    {
        
    }
    public class ESDisplayNameKeyToTypeAttribute : Attribute
    {
        public string TeamCollect = "收集到";
        public string DisplayKeyName = "显示名与键";
        public ESDisplayNameKeyToTypeAttribute(string team,string display)
        {
            TeamCollect = team;
            DisplayKeyName = display;
        }
    }
}

