using ES;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateTestMono : ESHostingMono_BaseESModule
{
    [NonReorderable,OdinSerialize]
    public BaseStandardStateMachine stateMachine=new BaseStandardStateMachine();
    public StateDataPack dataPack;



    // Start is called before the first frame update
    void Start()
    {
        KeyValueMatchingUtility.DataApply.ApplyStatePackToMachine(dataPack,stateMachine);
        stateMachine.TrySubmitHosting(this,true);
    }

    // Update is called once per frame
}
