using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ES
{
    [CreateAssetMenu(fileName = "独立数据-ES类型表配置", menuName = "ES数据/ES类型表")]

    public class ESTypeSelecter : SerializedScriptableObject
    {
        /*[LabelText("创建标签(分组/标签)")]
        public Dictionary<string, List<Type>> LayerStrings = new  Dictionary<string, List<Type>>();*/
        [Header("生成带层级字符串")]
        public List<Type> TypeList = new List<Type>();
        /*[Button("生成无层级的字符串列表")]
        public void GenerateStringList()
        {
            TypeList = new List<string>();
            foreach (var i in LayerStrings)
            {
                foreach (var ii in i.Value)
                {
                    TypeList.Add(i.Key + "/" + ii);
                }
            }
            TypeList.Sort((left, right) => string.CompareOrdinal(left, right));

        }
        [PropertySpace(15)]*/
    }
}