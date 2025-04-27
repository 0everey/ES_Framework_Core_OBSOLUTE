using ES.EvPointer;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace ES
{
    public interface ISoDataPack: IWithStringKey
    {
        string name_ { get; }
        Type getSoType();
        void AddGroup(ISoDataGroup group);
        void AddInfo(string s, SerializedScriptableObject so);
        IDictionary allInfo_ { get; }
        
    }
    public abstract class SoDataPack_<Info> : SerializedScriptableObject, ISoDataPack where Info : ScriptableObject,ISoDataInfo
    {
        [LabelText("包键")] public KeyString_Direct keyString = new KeyString_Direct();
        public KeyString key => keyString;
        [LabelText("已经应用过的数据组 列表 "),NonSerialized,OdinSerialize]
        public List<ISoDataGroup> applyingGroups = new List<ISoDataGroup>();
        [LabelText("预览全部数据")]
        public Dictionary<string, Info> allInfo = new Dictionary<string, Info>();
        public IDictionary allInfo_ => allInfo;
        public string name_ => keyString.str_direc;

        
        
        public Type getSoType()
        {
            return typeof(Info);
        }

        public void SetKey(object o)
        {
            keyString.str_direc = o.ToString();
        }

        public void AddInfo(string s, SerializedScriptableObject so)
        {
            if (allInfo.ContainsKey(s))
            {
                Debug.LogWarning($"发现重复的键{s}，默认跳过处理");

            }
            else if(so is Info info)
            {
                allInfo.Add(s, info);
            }
            else
            {
                Debug.LogWarning($"发现无效或者已经销毁的内容，键{s}，值{so}");
            }
        }

        public void AddGroup(ISoDataGroup group)
        {
            if (group.getSoType() != typeof(Info)) {
                Debug.LogError("试图加入不合法数据组:"+group.name_);
                return;
            } 
            var keys = group.keys;
            //加入已经缓存
            if (applyingGroups.Contains(group))
            {

            }
            else
            {
                applyingGroups.Add(group);
            }
            foreach (var k in keys)
            {
                ISoDataInfo use = group.GetOne(k);

                this.AddInfo(use.key.str_direc, use as SerializedScriptableObject);
            }
        }
    }

   
}
