using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{
    //不融入全局配置
    public class ArchitectureDataInfo : SoDataInfo, IArchitectureKeyValuePoolTypeValue
    {
        [SerializeReference,LabelText("携带参数")]
        public IArchitectureKeyValuePoolTypeValue Value=new ArchitectureTypeValue_Float();
        public IArchitectureKeyValuePoolTypeValue getArch => Value;

        public object TheKey => Value?.TheKey;

        public object TheValue => Value?.TheValue;
        private void OnValidate()
        {
           Value.SetKey(key.str_direc);
        }
        public void SetValue(object o)
        {
            Value.SetValue(o); 
        }
    }
}
