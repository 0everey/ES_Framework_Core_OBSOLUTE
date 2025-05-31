using ES;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ES {
    public class ESSpawnMaster : SingletonAsMono<ESSpawnMaster>
    {
        [LabelText("默认生成机制")] public ESObjectTypeSpawnHandle ESObjectTypeSpawn;
        public enum GameObjectType
        {
            [InspectorName("常规物体")] NormalRunGameObject,
            [InspectorName("角色")] Actor,
            [InspectorName("子弹")] Bullet,
            [InspectorName("特效或者场景物体")] VFXOrScene,
            [InspectorName("流程控制")] Controller,
        }
        public enum SpawnOption
        {
            [InspectorName("仅本地")] NormalLocal,
            [InspectorName("双端发送—放养")] NormalSendSpawn,
            [InspectorName("常规联网")] NormalNetWork,
            [InspectorName("联网-强制客户端权威")] ForceClientNetWork,
            [InspectorName("联网-强制服务器权威")] ForceServerNetWork
        }
        public enum PoolOption
        {
            [InspectorName("普通池处理")] NormalPool,
            [InspectorName("不用池处理")] NonePool,
            [InspectorName("强制池化")] ForcePoolable
        }
        public GameObject InsGameObject(GameObject prefab, GameObjectType type= GameObjectType.NormalRunGameObject,
            SpawnOption? spawn = null, PoolOption? pool = null, 
            Vector3? pos = null, Transform parent = null, Quaternion? qq = null)
        {
            GameObject gg;
            if (ESObjectTypeSpawn != null)
            {
                spawn ??= ESObjectTypeSpawn.SpawnType.ContainsKey(type)?ESObjectTypeSpawn.SpawnType[type]: SpawnOption.NormalLocal;
                pool??= ESObjectTypeSpawn.PoolType.ContainsKey(type) ? ESObjectTypeSpawn.PoolType[type] : PoolOption.NormalPool;
            }
            if (pool == PoolOption.NormalPool) gg = ES_PoolMaster.Instance.GetInPool(prefab, pos);
            else {
                if (parent == null) gg = Instantiate(prefab);
                else gg = Instantiate(prefab,parent);
                if (pos != null) gg.transform.position=pos??default;
                if (qq != null) gg.transform.rotation = qq ?? default;
            }
            //网络
            if (GameCenterManager.Instance.NetSupport&&spawn!= SpawnOption.NormalLocal&&ESNetManager.Instance.IsNet)
            {
                if(spawn== SpawnOption.NormalSendSpawn)
                {

                }
            }
            return null;
        }
        public GameObject Ins(GameObject g, Vector3 pos, Transform parent, Quaternion? qq = null)
        {
            if (g == null) return null;

            if (parent != null) return Instantiate(g, pos, qq ?? Quaternion.identity, parent);
            else return Instantiate(g, pos, qq ?? Quaternion.identity);
        }

    }
}
