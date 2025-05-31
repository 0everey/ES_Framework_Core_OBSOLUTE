using ES;
using FishNet.Connection;
using FishNet.Object;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{

    public class ESNetBehaviourCore : NetworkBehaviour
    {
        #region 默认支持
        [FoldoutGroup("ES默认功能"),LabelText("仅本地可运转")]
        public List<Transform> AllOnlyOwmer = new List<Transform>();
        
        #endregion

        private void Awake()
        {
            
        }
        public override void OnStartNetwork()
        {
            Debug.Log("NetWork测试");
            base.OnStartNetwork();
            
        }
        public override void OnStartClient()
        {
            Debug.Log("客户端测试");
            base.OnStartClient();
            if (IsOwner) {
                Debug.Log("客户端测试");
                ESNetManager.Instance.NetBehaviourCore = this;
                ESNetManager.Instance.OnSelfClientStart?.Invoke();
                ServerManager.Spawn(gameObject);
            }
            else
            {
                foreach(var i in AllOnlyOwmer)
                {
                    if (i != null) i.gameObject.SetActive(false);
                }
            }

        }
        public override void OnStopClient()
        {
            base.OnStopClient();
            if (IsOwner)
            {
                ESNetManager.Instance.NetBehaviourCore = null;
                ESNetManager.Instance.OnSelfClientStop?.Invoke();
            }
        }
        public override void OnStartServer()
        {
            base.OnStartServer();
            if (IsServerInitialized)
            {
                ESNetManager.Instance.OnSelfServerStart?.Invoke();
            }
        }
        public override void OnStopServer()
        {
            base.OnStopServer();
            if (IsServerInitialized)
            {
                ESNetManager.Instance.OnSelfServerStop?.Invoke();
            }
        }
        


        [ServerRpc(RequireOwnership =false)]
        public void Function_ClientSendStartWithPlayerAtServer(NetworkConnection connection,ESNetPlayer player)
        {
            if (ESNetManager.Instance.Need_RoomIDMatch)
            {
                if (player.RoomNumber == ESNetManager.Instance.NetPlayer.RoomNumber)
                {
                    Debug.Log("房间号匹配！" + player.RoomNumber);
                }
                else
                {
                    Debug.LogWarning("房间号不对！"+"试图加入" + player.RoomNumber+"实际上这里是"+ ESNetManager.Instance.NetPlayer.RoomNumber);
                    connection.Kick( FishNet.Managing.Server.KickReason.ExploitExcessiveData);
                }
            }
        }

        
    }
}

