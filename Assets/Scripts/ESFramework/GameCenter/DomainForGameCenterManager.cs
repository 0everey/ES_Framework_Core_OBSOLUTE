using ES;
using ES.EvPointer;

using Sirenix.OdinInspector;
#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor.TypeSearch;
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace ES
{
    public class DomainForGameCenterManager : BaseDomain<GameCenterManager, ClipForGamecenterManager> {
        protected override void AwakeRegesterAllClip()
        {
            base.AwakeRegesterAllClip();
        }
        protected override void CreatRelationship()
        {
            base.CreatRelationship();
            core.BaseDomain = this;
        }
    }

    #region 基本切片模范
    [Serializable]
    public abstract class ClipForGamecenterManager : Clip<GameCenterManager, DomainForGameCenterManager>
    {
        protected override string Description_ => "游戏管理器切片域";

       
    }
    [Serializable, TypeRegistryItem("模块01")]
    public class module1 : ClipForGamecenterManager
    {
        float f = 5;
        protected override void Update()
        {
            f -= Time.deltaTime;
            if (f < 0)
            {
                Domain.RemoveClip(this);
            }
            base.Update();
        }
    }
    [Serializable, TypeRegistryItem("模块00")]
    public class module0 : ClipForGamecenterManager
    {
        protected override void Update()
        {

            base.Update();
        }
    }
    [Serializable, TypeRegistryItem("模块02")]
    public class module2 : ClipForGamecenterManager
    {
        protected override void Update()
        {

            base.Update();
        }
    }
    [Serializable, TypeRegistryItem("模块03")]
    public class module3 : ClipForGamecenterManager
    {
        protected override void Update()
        {

            base.Update();
        }
    }


    [Serializable, TypeRegistryItem("模块04")]
    public class module4 : ClipForGamecenterManager
    {
        protected override void Update()
        {
            base.Update();
        }

        public override string[] allPreset => presetsForModule04;
        public static string[] presetsForModule04 = {"弱小的","强大的","特殊的" };
        protected override void SetupClipByPreset(string preset)
        {
            switch (preset)
            {
                case "弱小的":
                    Core.gameObject.AddComponent<Rigidbody>();
                    break;
                case "强大的":
                    Domain.transform.position = default;
                    break;
                case "特殊的":
                    if (Domain.GetComponent<Entity>() == null)
                    {
                        this.enabledSelf = true;
                    }
                    break;

            }
        }
    }
}


#endregion