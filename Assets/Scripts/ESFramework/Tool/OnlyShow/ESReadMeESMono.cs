using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{
    [TypeRegistryItem("ES提示文字")]
    public class ESReadMeESMono : MonoBehaviour
    {
        [LabelText("---开始编辑---")]
        public bool edit = true;
        [DisplayAsString(fontSize:20,EnableRichText =true ),ShowIf("@!edit"),GUIColor("showColor"),ShowInInspector,HideLabel]
        public string ReadMe = "编写提示文件"; 
        [TextArea(3,10),ShowIf("edit"),OnValueChanged("SetString")]
        public string readMe = "编写提示文件";
        [ColorPalette, ShowIf("edit")]
        public Color showColor = Color.white;
        private void SetString(string edit)
        {
            ReadMe = edit;
        }
      
    }
    [Serializable]
    public class ESReadMeClass
    {
        [LabelText("---开始编辑---")]
        public bool edit = true;
        [DisplayAsString(fontSize: 20, EnableRichText = true), ShowIf("@!edit"), GUIColor("showColor"), ShowInInspector, HideLabel]
        public string ReadMe = "编写提示文件";
        [TextArea(3, 10), ShowIf("edit"), OnValueChanged("SetString")]
        public string readMe = "编写提示文件";
        [ColorPalette, ShowIf("edit")]
        public Color showColor = Color.white;
        private void SetString(string edit)
        {
            ReadMe = readMe;
        }
    }
}
