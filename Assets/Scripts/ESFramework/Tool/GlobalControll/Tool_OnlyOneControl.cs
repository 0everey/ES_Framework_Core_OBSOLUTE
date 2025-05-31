using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{
    
    [DefaultExecutionOrder(-3)]
    public class Tool_OnlyOneControl : MonoBehaviour
    {
        [LabelText("唯一标识")]
        public string Tag = "默认核心";
        [LabelText("加载不销毁")]
        public bool DontDes = true;
        [LabelText("可替换优先级")]
        public int PrioityForToReplace = 0;
        [LabelText("被替换优先级>")]
        public int PrioityForToBeReplaced = 0;
        public static Dictionary<string, Tool_OnlyOneControl> AllOnlyOne = new Dictionary<string, Tool_OnlyOneControl>();
        public static Tool_OnlyOneControl onlyOne;
        private void Awake()
        {
            if (onlyOne == null)
            {
                onlyOne = this;
                AllOnlyOne.Add(Tag,this);
                if(DontDes)DontDestroyOnLoad(gameObject);
            }
            else if(AllOnlyOne.ContainsKey(Tag))
            {
                var last = AllOnlyOne[Tag];
                if (last.PrioityForToBeReplaced < PrioityForToReplace)
                {
                    AllOnlyOne[Tag] = this;
                    if (DontDes) DontDestroyOnLoad(gameObject);
                }
                else
                {
                    DestroyImmediate(gameObject);
                } 
            }
            else
            {
                AllOnlyOne.Add(Tag, this);
                if (DontDes) DontDestroyOnLoad(gameObject);
            }
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
