using ES.EvPointer;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{
    [TypeRegistryItem("针播放器_计时触发", "针播放器")]
    public class PointerPlayerWithTimer : PointerPlayer,IPointerNone
    {
        [LabelText("启用定时触发"), SerializeReference] public IPointerForBool_Only UseTimerPick = new PointerForBool_Direc();
        [LabelText("开始延迟"), SerializeReference] public IPointerForFloat_Only startDelay = new PointerForFloat_Random30() { float_range = new Vector2(5, 10) };
        [LabelText("触发间隔"), SerializeReference] public IPointerForFloat_Only pickTimeDis = new PointerForFloat_Random30() { float_range = new Vector2(3, 5) };
        [LabelText("定时触发"), SerializeReference] public IPointerNone pointerForTimer = new PointerPickerEveryThing();
        [ShowInInspector, ReadOnly, LabelText("计时器")] private float timer = 0;
        [ShowInInspector, ReadOnly, LabelText("已经开始运行")] private bool hasStart = false;
        [ShowInInspector, ReadOnly, LabelText("开始延迟")] private float startDelayGet = 3;
        [ShowInInspector, ReadOnly, LabelText("下一次计时触发时间")] private float nextTimerPick = 3;

        public override IPointer Pointer => pointerForTimer;

        // Start is called before the first frame update
        void Start()
        {
            timer = 0;
            hasStart = false;
            startDelayGet = startDelay?.Pick() ?? 3;
            nextTimerPick = pickTimeDis?.Pick() ?? 3;
        }

        // Update is called once per frame
        void Update()
        {
            bool useTimer = UseTimerPick?.Pick() ?? false;
            if (!useTimer) return;
            timer += Time.deltaTime;
            if (hasStart)
            {
                if (timer > nextTimerPick)
                {
                    pointerForTimer?.Pick();
                    //重制
                    timer = 0;
                    nextTimerPick = pickTimeDis?.Pick() ?? 3;
                }
            }
            else
            {
                if (timer > startDelayGet)
                {
                    hasStart = true;
                }
            }
        }
    }
}
