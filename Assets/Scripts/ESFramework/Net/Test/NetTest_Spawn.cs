using ES;
using FishNet.Object;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ES {
    public class NetTest_Spawn : NetworkBehaviour
    {
        [LabelText("预制件")]
        public GameObject prefab;

        [Button("直接生成")]
        public void InitDirec()
        {
            GameObject.Instantiate(prefab);
        }
        [Button("生成_网络")]
        public void Init_SpawnByClient()
        {
            GameObject f= GameObject.Instantiate(prefab);
            ServerManager.Spawn(f);
        }
        
    }
}
