using ES;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test501 : ESHostingMono_BaseESModule
{
   /* [ShowInInspector]
    public SafeUpdateSet_EasyQueue_SeriNot<string> listSafeUpdate_Easy = new SafeUpdateSet_EasyQueue_SeriNot<string>();*/
    public ESHostingMono_BaseESModule SelfHosting => this;
    public ESModule_WithDelegate 更子级别;
    // Start is called before the first frame update
    void Start()
    {
        var aModule =new ESModule_WithDelegate() { }.
            WithEnable((i) => { Debug.Log("启用");}).
            WithUpdate((i)=> { Debug.Log("运行中"); }).
            WithDisable((i) => { Debug.Log("停用"); });
        
        aModule.TrySubmitHosting(SelfHosting, true);

        var 直接属于脚本的=new ESHostingAndModule_WithDelegate().
            WithEnable((i) => { Debug.Log("直接属于脚本的启用"); }).
            WithUpdate((i) => { Debug.Log("直接属于脚本的运行中"); }).
            WithDisable((i) => { Debug.Log("直接属于脚本的停用"); });

        更子级别 = new ESModule_WithDelegate().
            WithEnable((i) => { Debug.Log("更子级别的启用"); }).
            WithUpdate((i) => { Debug.Log("更子级别的运行中"); }).
            WithDisable((i) => { Debug.Log("更子级别的停用"); });

        直接属于脚本的.TrySubmitHosting(SelfHosting, true);
        更子级别.TrySubmitHosting(直接属于脚本的,true);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log("测试"+更子级别._HasSubmit);
            更子级别.TryWithDrawHosting(null,false);
            Debug.Log("测试" + 更子级别._HasSubmit);
        }
    }
}
