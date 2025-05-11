using System;
using Cinemachine;
using ES.EvPointer;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ESFramework.InterfaceAndAbstractAndExtension.Pointer.PointForCinemachine
{
    public abstract class PointerSetCinemachine_Abstract : IPointerNone
    {
        [LabelText("直接引用CinemachineVirtualCamera")]
        public CinemachineVirtualCamera CinemachineVirtualCamera;
        public object Pick(object by = null, object yarn = null, object on = null)
        {
            if (CinemachineVirtualCamera != null)
                PickTruely(CinemachineVirtualCamera);
            return -1;
        }
        public abstract void PickTruely(CinemachineVirtualCamera camera);
    }
    
    [Serializable]
    [TypeRegistryItem("Cinemachine_To_设置LookAt", "Cinemachine")]
    public class PointerSetCinemachine_LookAt : PointerSetCinemachine_Abstract
    {
        [LabelText("LookAt目标游戏物体")]public Transform lookAt;
        public override void PickTruely(CinemachineVirtualCamera camera)
        {
            camera.LookAt=lookAt;
        }
    }
    [Serializable]
    [TypeRegistryItem("Cinemachine_To_设置Follow", "Cinemachine")]
    public class PointerSetCinemachine_Follow : PointerSetCinemachine_Abstract
    {
        [LabelText("LookAt目标游戏物体")]public Transform follow;
        public override void PickTruely(CinemachineVirtualCamera camera)
        {
            camera.Follow=follow;
        }
    }
    
    [Serializable]
    [TypeRegistryItem("Cinemachine_To_设置Priority", "Cinemachine")]
    public class PointerSetCinemachine_Priority : PointerSetCinemachine_Abstract
    {
        [LabelText("优先级设置")]public int value;
        public override void PickTruely(CinemachineVirtualCamera camera)
        {
            camera.m_Priority = value;
        }
    }

    [Serializable]
    [TypeRegistryItem("Cinemachine_To_设置StandbyUpdateMode", "Cinemachine")]
    public class PointerSetCinemachine_StandbyUpdateMode : PointerSetCinemachine_Abstract
    {
        [LabelText("更新频率设置")] public CinemachineVirtualCameraBase.StandbyUpdateMode value;
        public override void PickTruely(CinemachineVirtualCamera camera)
        {
            camera.m_StandbyUpdate = value;
        }
    }
    [Serializable]
    [TypeRegistryItem("Cinemachine_To_设置LensSettings", "Cinemachine")]
    public class PointerSetCinemachine_LensSettings : PointerSetCinemachine_Abstract
    {
        [LabelText("摄像机的镜头属性设置")] public LensSettings value;
        public override void PickTruely(CinemachineVirtualCamera camera)
        {
            camera.m_Lens = value;
        }
    }
    [Serializable]
    [TypeRegistryItem("Cinemachine_To_设置TransitionParams", "Cinemachine")]
    public class PointerSetCinemachine_TransitionParams : PointerSetCinemachine_Abstract
    {
        [LabelText("Transition设置")] public CinemachineVirtualCameraBase.TransitionParams value;
        public override void PickTruely(CinemachineVirtualCamera camera)
        {
            camera.m_Transitions = value;
        }
    }
}