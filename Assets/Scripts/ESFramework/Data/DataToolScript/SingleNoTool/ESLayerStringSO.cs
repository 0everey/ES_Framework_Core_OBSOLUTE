using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ES
{
    [CreateAssetMenu(fileName = "独立数据-ES层级字符串配置", menuName = "ES数据/ES层级字符串")]

    public class ESLayerStringSO : SerializedScriptableObject
    {
        [LabelText("创建标签(分组/标签)")]
        public Dictionary<string, List<string>> LayerStrings = new Dictionary<string, List<string>>();
        [Header("生成带层级字符串")]
        public List<string> StringsList = new List<string>();
        [Button("生成无层级的字符串列表")]
        public void GenerateStringList()
        {
            StringsList = new List<string>();
            foreach (var i in LayerStrings)
            {
                foreach (var ii in i.Value)
                {
                    StringsList.Add(i.Key + "/" + ii);
                }
            }
            StringsList.Sort((left, right) => string.CompareOrdinal(left, right));

        }
        [PropertySpace(15)]
        [Button("从字符串列表生成字典")]
        public void BeGenerateFromStringList()
        {
#if UNITY_EDITOR
           bool b= EditorUtility.DisplayDialog("创建新的字典","这将会用当前缓冲的字符串列表创建出新的带层级字典,请注意别搞错","重新生成字典","取消");
            if (b)
            {
                LayerStrings = new Dictionary<string, List<string>>();
                foreach(var i in StringsList)
                {
                    string[] strings = i.Split('/',2);
                    Debug.Log(strings.Length);
                    if (strings.Length == 2)
                    {
                        if (LayerStrings.ContainsKey(strings[0]))
                        {
                            List<string> use = LayerStrings[strings[0]];
                            if (use == null)
                            {
                                List<string> newUse = new List<string>();
                                newUse.Add(strings[1]);
                                LayerStrings.Add(strings[0],newUse);
                            }
                            else
                            {
                                use.Add(strings[1]);
                            }
                        }
                        else
                        {
                            List<string> newUse = new List<string>();
                            newUse.Add(strings[1]);
                            LayerStrings.Add(strings[0], newUse);
                        }

                    }
                }
            }
#endif

        }
    }
}