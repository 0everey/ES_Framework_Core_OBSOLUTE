using ES;
using FishNet.Managing;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ES
{
    
    public class  ESNetRemoteDomain : BaseDomain<ESNetManager, RemoteClipForESNetManager>
    {
        protected override void CreatRelationship()
        {
            base.CreatRelationship();
            core.RemoteDomain = this;
        }
    }
    [Serializable]
    public abstract class RemoteClipForESNetManager : Clip<ESNetManager, ESNetRemoteDomain>
    {

    }


    [Serializable, TypeRegistryItem("房间支持：客户端发送验证消息")]
    public class ClipRemote_ClientSendThisAuthentiacationMessage : RemoteClipForESNetManager
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            ESNet.OnSelfClientStart += SendMessage;
        }
        private void SendMessage()
        {
            Debug.Log("发送哈");
            ESNet.NetBehaviourCore.Function_ClientSendStartWithPlayerAtServer(ESNet.SelfConnection, ESNet.NetPlayer);
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            ESNet.OnSelfClientStart -= SendMessage;
        }
        protected override void Update()
        {

        }
        protected override void CreateRelationship()
        {
            base.CreateRelationship();
            //Domain.Module_XXX = this;  显性引用
        }
    }

}
