using ES;
using ES.EvPointer;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[TypeRegistryItem("针播放器_停表", "针播放器")]
public class PointerPlayerAsStopWatch : PointerPlayer,IPointerForFloatCaster
{
    [ShowInInspector, ReadOnly, LabelText("已经计时")] public float timer = 0;
    [LabelText("正在计时")]public bool isUsing = false;

    public override IPointer Pointer => null;

    
    public PointerPlayerSystemObjectCaster playerCaster => playerCaster_;
    [LabelText("发起投射?", SdfIconType.At), GUIColor("@KeyValueMatchingUtility.ColorSelector.ColorForCaster")] public bool usePlayerCaster;
    [LabelText("发送时间到Caster", SdfIconType.At), ShowIf("usePlayerCaster"), GUIColor("@KeyValueMatchingUtility.ColorSelector.ColorForCaster")] public PointerPlayerSystemObjectCaster playerCaster_;

    public float Cast()
    {
        return timer;
    }

    // Start is called before the first frame update
    public void StopWatch_Start()
    {
        isUsing = true;
    }
    public void StopWatch_Pause()
    {
        isUsing = false;
    }
    public void StopWatch_ReStart()
    {
        isUsing = true;
        timer = 0;
    }
    public void StopWatch_Stop()
    {
        isUsing = false;
        timer = 0;
    }
    // Update is called once per frame
    void Update()
    {
        if (isUsing) { 
            timer += Time.deltaTime;
            if (usePlayerCaster&& playerCaster_!=null)
            {
                playerCaster_.Recieve(timer);
            }
        }
    }
}
