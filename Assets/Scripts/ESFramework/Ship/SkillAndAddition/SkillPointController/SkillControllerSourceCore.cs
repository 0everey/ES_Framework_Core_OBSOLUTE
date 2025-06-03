using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

namespace ES
{
    public class SkillControllerSourceCore :BaseCore, IWithArchitecture
    {
        [SerializeReference]
        public BaseArchitectureWithKeyValuePool SkillControllerKeyValueArchitecture = new BaseArchitectureWithKeyValuePool();
        public IArchitecture GetArchitecture => SkillControllerKeyValueArchitecture;
        private void Start()
        {
            SkillControllerKeyValueArchitecture.Init();
            StartCoroutine(_CoroutineMaker_Obsolete.DelayOneFrameCoroutine(RefreshAll));
        }
        // Start is called before the first frame update
        [Button("刷新测试")]
        public void RefreshAll()
        {
            BroadcastMessage("ApplyArchitectureKeyValuePool", this);
        }
    }
}
