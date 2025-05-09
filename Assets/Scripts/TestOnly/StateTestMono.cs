using ES;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateTestMono : ESHostingMono_BaseESModule, IWithESMachine
{
    public ReleaseableSkillClip clip;
    public ESStandardStateMachine_StringKey machine_ = new ESStandardStateMachine_StringKey();

    public BaseOriginalStateMachine TheMachine => machine_;



    public StateDataPack pack;
    [LabelText("不可打断")]public ESMicroStateSharedData TheShared1;
    [LabelText("容易被打断")] public ESMicroStateSharedData TheShared2;
    [LabelText("容易合并")] public ESMicroStateSharedData TheShared3;
    // Start is called before the first frame update
    void Start()
    {
       
        KeyValueMatchingUtility.DataApply.ApplyStatePackToMachine(pack,machine_);
        machine_.TrySubmitHosting(this,true);
        machine_.defaultStateKey = "静止";
        machine_.WithEnterState(machine_.GetStateByKey("攻击")); ;
        //第一层
       /* {
            machine_.RegisterNewState("不可打断",
               new BaseESMicroStateWithDelegateRunTimeLogic_StringKey()
                .WithStatePrepared((self) => { Debug.Log("进入1111"); })
                .WithStateUpdate((self) => { Debug.Log("更新1111"); })
                .WithStateExit((self) => { Debug.Log("退出1111"); })
                .WithSharedData(TheShared1)
                ); ;

            machine_.RegisterNewState("容易被打断",
               new BaseESMicroStateWithDelegateRunTimeLogic_StringKey()
                .WithStatePrepared((self) => { Debug.Log("进入2222"); })
                .WithStateUpdate((self) => { Debug.Log("更新2222"); })
                .WithStateExit((self) => { Debug.Log("退出2222"); })
                .WithSharedData(TheShared2)
                );
            machine_.RegisterNewState("容易合并",
              new BaseESMicroStateWithDelegateRunTimeLogic_StringKey()
               .WithStatePrepared((self) => { Debug.Log("进入2222"); })
               .WithStateUpdate((self) => { Debug.Log("更新2222"); })
               .WithStateExit((self) => { Debug.Log("退出2222"); })
               .WithSharedData(TheShared3)
               );
            //第二层：儿子
            {
                var 儿子 = new ESMicroStateMachine_StringKey();
                儿子.RegisterNewState("子不可打断",
                  new BaseESMicroStateWithDelegateRunTimeLogic_StringKey()
                   .WithStatePrepared((self) => { Debug.Log("进入1111"); })
                   .WithStateUpdate((self) => { Debug.Log("更新1111"); })
                   .WithStateExit((self) => { Debug.Log("退出1111"); })
                   .WithSharedData(TheShared1)
                   );

                儿子.RegisterNewState("子容易被打断",
                   new BaseESMicroStateWithDelegateRunTimeLogic_StringKey()
                    .WithStatePrepared((self) => { Debug.Log("进入2222"); })
                    .WithStateUpdate((self) => { Debug.Log("更新2222"); })
                    .WithStateExit((self) => { Debug.Log("退出2222"); })
                    .WithSharedData(TheShared2)
                    );
                //第三层:儿子/孙子
                {
                    var 孙子 = new ESMicroStateMachine_StringKey();
                    孙子.RegisterNewState("孙子容易被打断",
                      new BaseESMicroStateWithDelegateRunTimeLogic_StringKey()
                       .WithStatePrepared((self) => { Debug.Log("进入1111"); })
                       .WithStateUpdate((self) => { Debug.Log("更新1111"); })
                       .WithStateExit((self) => { Debug.Log("退出1111"); })
                       .WithSharedData(TheShared2)
                       );

                    孙子.RegisterNewState("子第二个",
                       new BaseESMicroStateWithDelegateRunTimeLogic_StringKey()
                        .WithStatePrepared((self) => { Debug.Log("进入2222"); })
                        .WithStateUpdate((self) => { Debug.Log("更新2222"); })
                        .WithStateExit((self) => { Debug.Log("退出2222"); })
                        .WithSharedData(TheShared3)
                        );

                    儿子.RegisterNewState("孙子", 孙子);
                }
                machine_.RegisterNewState("儿子层", 儿子);
            }

            machine_.TrySubmitHosting(this, true);
            machine_.TryActiveStateByKey("第一个");
        }*/
    }
    protected override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.O))
        {
            machine_.TryActiveStateByKey("第一个");
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            machine_.TryActiveStateByKey("第二个");
        }
    }
    // Update is called once per frame
}
