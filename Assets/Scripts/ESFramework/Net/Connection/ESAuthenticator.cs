using ES;
using FishNet.Authenticating;
using FishNet.Connection;
using FishNet.Managing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{

    public class ESAuthenticator  : Authenticator
    {
        
        public override event Action<NetworkConnection, bool> OnAuthenticationResult=(who,end)=> { };
        
        public override void InitializeOnce(NetworkManager networkManager)
        {
            base.InitializeOnce(networkManager);
            
        }
        
        public override void OnRemoteConnection(NetworkConnection connection)
        {
            base.OnRemoteConnection(connection);
            bool b = TestConnection(connection);
            OnAuthenticationResult?.Invoke(connection,b);
            if (!b)
            {
               
            }
        }
        public bool TestConnection(NetworkConnection network)
        {
            if (ESNetManager.Instance.ConnectTest(network)) return true;
            return true;
        }
    }

    
}

