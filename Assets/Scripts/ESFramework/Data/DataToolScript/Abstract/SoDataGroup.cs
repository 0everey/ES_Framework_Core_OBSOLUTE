using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

namespace ES
{
    //  [CreateAssetMenu(fileName = "SoDataInfoGroup",menuName = "EvData/SoGroup")]
    public interface ISoDataGroup {
    string name_ { get; }
        Type getSoType();
        bool CanStore(string s);
        void Add(string s,ScriptableObject o);
        List<string> keys { get; }
        ISoDataInfo GetOne(string k);
        void Remove(string s);
        void SetKey(string or, string now);
    }
    public abstract class SoDataGroup<SoType> : SerializedScriptableObject, ISoDataGroup where SoType:ScriptableObject,ISoDataInfo 
    {
        
        [LabelText("数据组名字"),GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_03")]public string name__="数据组";
        [LabelText("数据组字典")]
        public Dictionary<string, SoType> keyValues = new Dictionary<string, SoType>();

        public string name_ => name__;
        public List<string> keys => keyValues.Keys.ToList();
        

        public void Add(string s, ScriptableObject o)
        {
            if (keyValues.ContainsKey(s))
            {

            }
            else if(o is SoType typeMatch)
            {
                keyValues.Add(s, typeMatch);
            }
            
        }
        public bool CanStore(string s)
        {
            if (keyValues.ContainsKey(s)) return false;
            else return true;
        }

        public ISoDataInfo GetOne(string k)
        {
            if (keyValues.ContainsKey(k))
            {
                return keyValues[k];
            }
            else
            {
                return default;
            }
        }

        public Type getSoType()
        {
            return typeof(SoType);
        }

        public void Remove(string k)
        {
            if (keyValues.ContainsKey(k))
            {
                keyValues.Remove(k);
            }   
        }

        public void SetKey(string or, string now)
        {
            
        }

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
