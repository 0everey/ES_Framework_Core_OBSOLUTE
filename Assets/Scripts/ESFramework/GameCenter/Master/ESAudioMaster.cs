using ES;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ESAudioMaster : Singleton<ESAudioMaster>
{

    [LabelText("主BGM"),Required]public AudioSource MainBGM;
    [LabelText("主附加声"), Required] public AudioSource MainAddition;
    [LabelText("主音效"), Required] public AudioSource MainSound;

    [Button("直接播放BGM")]
    public void PlayDirect_BGM(AudioClip clip,float volume=-1)
    {
        MainBGM.clip = clip;
        if (volume >= 0) MainBGM.volume = volume;
        MainBGM.Play();
    }
    [Button("直接播放附加音")]
    public void PlayDirect_Addition(AudioClip clip, float volume = -1)
    {
        MainAddition.clip = clip;
        if (volume >= 0) MainBGM.volume = volume;
        MainAddition.Play();
    }
    [Button("直接播放音效(OneShot)")]
    public void PlayDirect_Sound_OneShot(AudioClip clip, float volume = -1)
    {
        if (volume >= 0) MainSound.PlayOneShot(clip, volume);
        else MainSound.PlayOneShot(clip,MainSound.volume);
    }
}
