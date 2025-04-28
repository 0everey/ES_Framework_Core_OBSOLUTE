using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
扩展时 建议创建新的脚本 
修改文件时 使用 #region + 自己的名字
格式尽量统一 
多交流 --- Everey
*/
namespace ES.EvPointer
{
    //核心 Ev针支持 关于 对音效播放的的支持 部分
    //基本实现IPointerNone或者IPointerOnlyAction
    //这里稍微写一点就  


    #region 直接利用全局音源和Clip文件
    [Serializable,TypeRegistryItem("直接播放BGM","音乐")]
    public class PointerSetAudio_BGM  : IPointerNone
    {
        [LabelText("使用音乐资源")]public AudioClip clip;
        [LabelText("音量(小于0用音源的)"),SerializeReference]public IPointerForFloat_Only volume = new PointerForFloat_Direct() { float_=1 };

        public object Pick(object by = null, object yarn = null, object on = null)
        {
            GameCenterManager.Instance.AudioMaster.PlayDirect_BGM(clip, volume?.Pick() ?? -1);
            return null;
        }
    }
    [Serializable, TypeRegistryItem("直接播放附加音乐", "音乐")]
    public class PointerSetAudio_Addition : IPointerNone
    {
        [LabelText("使用音乐资源")] public AudioClip clip;
        [LabelText("音量(小于0用音源的)"), SerializeReference] public IPointerForFloat_Only volume = new PointerForFloat_Direct() { float_ = 1 };

        public object Pick(object by = null, object yarn = null, object on = null)
        {
            GameCenterManager.Instance.AudioMaster.PlayDirect_Addition(clip, volume?.Pick() ?? -1);
            return null;
        }
    }
    [Serializable, TypeRegistryItem("直接播放音效", "音乐")]
    public class PointerSetAudio_Sound : IPointerNone
    {
        [LabelText("使用音乐资源")] public AudioClip clip;
        [LabelText("音量(小于0用音源的)"), SerializeReference] public IPointerForFloat_Only volume = new PointerForFloat_Direct() { float_ = 1 };

        public object Pick(object by = null, object yarn = null, object on = null)
        {
            GameCenterManager.Instance.AudioMaster.PlayDirect_Sound_OneShot(clip, volume?.Pick() ?? -1);
            return null;
        }
    }
    #endregion

}