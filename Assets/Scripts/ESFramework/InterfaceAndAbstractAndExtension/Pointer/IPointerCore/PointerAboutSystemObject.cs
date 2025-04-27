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
    //核心 Ev针支持 关于System.Object 也就是object 万物之源部分
    #region System.Object部分
        #region System.Object 接口抽象和包
    public interface IPointerForSystemObject<By, Yarn, On> : IPointer<object, By, Yarn, On>
    {

    }
    public interface IPointerForSystemObjectList : IPointerOnlyBackList<object>
    {

    }
    #endregion
    /*  ---等待制作
     *  [Serializable, TypeRegistryItem("系统物体针包_选中几个")]
public class PointerForSystemObject_PackerSelectSome : PointerPackerForSelectSomeBack<object, IPointer>, IPointerForSystemObjectList
{

}
[Serializable, TypeRegistryItem("系统物体针包_选中一个")]
public class PointerForSystemObject_PackerSelect : PointerPackerForOnlySelectBackOne<object, IPointer>, IPointer
{

}*/
        #region System.Object功能
    [Serializable, TypeRegistryItem("系统物体引用_来自针")]
    public class PointerForSystemObjectReference_FromIPointer : IPointer, IPointerForSystemObject<object, object, object>
    {
        [LabelText("任意针引用"), SerializeReference] public IPointer aP;
        public object Pick(object by = null, object yarn = null, object on = null)
        {

            return aP;
        }
    }
    [Serializable, TypeRegistryItem("系统物体引用_来自脚本")]
    public class PointerForSystemObjectReference_FromCompoent : IPointer, IPointerForSystemObject<object, object, object>
    {
        [LabelText("任意脚本引用")] public Component aCom;
        public object Pick(object by = null, object yarn = null, object on = null)
        {
            return aCom;
        }
    }
    [Serializable, TypeRegistryItem("系统物体引用_来自Unity物体")]
    public class PointerForSystemObjectReference_FromUnityObject : IPointer
    {
        [LabelText("任意Unity物体引用")] public UnityEngine.Object aOb;
        public object Pick(object by = null, object yarn = null, object on = null)
        {
            return aOb;
        }
    }
    [Serializable, TypeRegistryItem("系统物体引用_来自游戏物体")]
    public class PointerForSystemObjectReference_FromGameObject : IPointer
    {
        [LabelText("游戏物体引用")] public GameObject aOb;
        public object Pick(object by = null, object yarn = null, object on = null)
        {

            return aOb;
        }
    }

    [Serializable, TypeRegistryItem("系统物体值_来自投射播放器", "单值针/物体针")]
    public class PointerForSystemObject_FromPlayer : IPointer
    {
        [LabelText("系统物体投射播放器")] public PointerPlayerSystemObjectCaster obCaster;
        public object Pick(object by = null, object yarn = null, object on = null)
        {

            return (obCaster as IPointer)?.Pick() as object;
        }
    }
    [Serializable, TypeRegistryItem("系统物体值_来自Unity物体针")]
    public class PointerForSystemObjectValueFromUnityObject : IPointer
    {
        [LabelText("来源UnityObject"), SerializeReference] public IPointerForUnityObject_Only pointerForUO_;
        public object Pick(object by = null, object yarn = null, object on = null)
        {
            return pointerForUO_?.Pick() ?? default;
        }
    }
    [Serializable, TypeRegistryItem("系统物体值_来自游戏物体针")]
    public class PointerForSystemObjectValueFromGameObject : IPointer
    {
        [LabelText("来源游戏物体"), SerializeReference] public IPointerForGameObject_Only pointerGameO;
        public object Pick(object by = null, object yarn = null, object on = null)
        {
            return pointerGameO?.Pick() ?? default;
        }
    }
    [Serializable, TypeRegistryItem("系统物体值_来自针")]
    public class PointerForSystemObjectValueFromIPointer : IPointer
    {
        [LabelText("来源针"), SerializeReference] public IPointerForIPointer_Only pointerForiPointer;
        public object Pick(object by = null, object yarn = null, object on = null)
        {
            return pointerForiPointer?.Pick() ?? default;
        }
    }
    [Serializable, TypeRegistryItem("系统物体值_来自浮点数针")]
    public class PointerForSystemObjectValueFromFloat : IPointer
    {
        [LabelText("来源Float"), SerializeReference] public IPointerForFloat_Only pointerForFloat_;
        public object Pick(object by = null, object yarn = null, object on = null)
        {
            return pointerForFloat_?.Pick() ?? 1;
        }
    }
    [Serializable, TypeRegistryItem("系统物体值_来自整数针")]
    public class PointerForSystemObjectValueFromInt : IPointer
    {
        [LabelText("来源Int"), SerializeReference] public IPointerForInt_Only pointerForInt_;
        public object Pick(object by = null, object yarn = null, object on = null)
        {
            return pointerForInt_?.Pick() ?? 1;
        }
    }
    [Serializable, TypeRegistryItem("系统物体值_来自Vector3针")]
    public class PointerForSystemObjectValueFromVector3 : IPointer
    {
        [LabelText("来源Vector3"), SerializeReference] public IPointerForVector3_Only pointerForVector3_;
        public object Pick(object by = null, object yarn = null, object on = null)
        {
            return pointerForVector3_?.Pick() ?? default;
        }
    }
    [Serializable, TypeRegistryItem("系统物体值_来自Vector2针")]
    public class PointerForSystemObjectValueFromVector2 : IPointer
    {
        [LabelText("来源Vector2"), SerializeReference] public IPointerForVector2_Only pointerForVector2_;
        public object Pick(object by = null, object yarn = null, object on = null)
        {
            return pointerForVector2_?.Pick() ?? default;
        }
    }
    [Serializable, TypeRegistryItem("系统物体值_来自布尔针")]
    public class PointerForSystemObjectValueFromBool : IPointer
    {
        [LabelText("来源bool"), SerializeReference] public IPointerForBool_Only pointerForbool_;
        public object Pick(object by = null, object yarn = null, object on = null)
        {
            return pointerForbool_?.Pick() ?? default;
        }
    }
    [Serializable, TypeRegistryItem("系统物体值_来自字符串针")]
    public class PointerForSystemObjectValueFromString : IPointer
    {
        [LabelText("来源字符串"), SerializeReference] public IPointerForString_Only pointerForString_;
        public object Pick(object by = null, object yarn = null, object on = null)
        {
            return (pointerForString_?.Pick() ?? default).ToString();
        }
    }
    [Serializable, TypeRegistryItem("系统物体值_来自变换针")]
    public class PointerForSystemObjectValueFromTransform : IPointer
    {
        [LabelText("来源变换"), SerializeReference] public IPointerForTransform_Only pointerForTrans_;
        public object Pick(object by = null, object yarn = null, object on = null)
        {
            return (pointerForTrans_?.Pick() ?? default);
        }
    }
    [Serializable, TypeRegistryItem("系统物体值_来自四元数针")]
    public class PointerForSystemObjectValueFromQuaternion : IPointer
    {
        [LabelText("来源四元数"), SerializeReference] public IPointerForQuaternion_Only pointerForQuaternion_;
        public object Pick(object by = null, object yarn = null, object on = null)
        {
            return (pointerForQuaternion_?.Pick() ?? default);
        }
    }
    [Serializable, TypeRegistryItem("系统物体值_来自脚本针")]
    public class PointerForSystemObjectValueFromComponent : IPointer
    {
        [LabelText("来源脚本"), SerializeReference] public IPointerForComponent_Only pointerForCom_;
        public object Pick(object by = null, object yarn = null, object on = null)
        {
            return (pointerForCom_?.Pick() ?? default);
        }
    }
    [Serializable, TypeRegistryItem("系统物体值_来自类型针")]
    public class PointerForSystemObjectValueFromType : IPointer
    {
        [LabelText("来源类型"), SerializeReference] public IPointerForType_Only pointerForType_;
        public object Pick(object by = null, object yarn = null, object on = null)
        {
            return (pointerForType_?.Pick() ?? default);
        }
    }
    [Serializable, TypeRegistryItem("系统物体转针")]
    public class PointerForIPointer_FromSystemObject : IPointerForIPointer_Only
    {
        [SerializeReference, LabelText("源object")]
        public IPointer objectSource;
        public IPointer Pick(object by = null, object yarn = null, object on = null)
        {
            return objectSource?.Pick() as IPointer;
        }

    }
    [Serializable, TypeRegistryItem("系统物体转浮点数")]
    public class PointerForFloat_FromSystemObject : IPointerForFloat_Only
    {
        [SerializeReference, LabelText("源object")]
        public IPointer objectSource;
        public float Pick(object by = null, object yarn = null, object on = null)
        {
            return (Convert.ToSingle(objectSource?.Pick() ?? 1));
        }
    }
    [Serializable, TypeRegistryItem("系统物体转整数")]
    public class PointerForInt_FromSystemObject : IPointerForInt_Only
    {
        [SerializeReference, LabelText("源object")]
        public IPointer objectSource;
        public int Pick(object by = null, object yarn = null, object on = null)
        {
            return Convert.ToInt32(objectSource?.Pick() ?? 1);
        }
    }
    [Serializable, TypeRegistryItem("系统物体转Vector3")]
    public class PointerForVector3_FromSystemObject : IPointerForVector3_Only
    {
        [SerializeReference, LabelText("源object")]
        public IPointer objectSource;
        public Vector3 Pick(object by = null, object yarn = null, object on = null)
        {
            object oo = objectSource?.Pick() ?? default;
            if (oo is Vector2 v2)
            {
                return (Vector3)v2;
            }
            else if (oo is Vector3 v3)
            {
                return v3;
            }
            return default;
        }
    }
    [Serializable, TypeRegistryItem("系统物体转Vector2")]
    public class PointerForVector2_FromSystemObject : IPointerForVector2_Only
    {
        [SerializeReference, LabelText("源object")]
        public IPointer objectSource;
        public Vector2 Pick(object by = null, object yarn = null, object on = null)
        {
            object oo = objectSource?.Pick() ?? default;
            if (oo is Vector2 v2)
            {
                return v2;
            }
            else if (oo is Vector3 v3)
            {
                return (Vector2)v3;
            }
            return default;
        }
    }
    [Serializable, TypeRegistryItem("系统物体转布尔值")]
    public class PointerForBool_FromSystemObject : IPointerForBool_Only
    {
        [SerializeReference, LabelText("源object")]
        public IPointer objectSource;
        public bool Pick(object by = null, object yarn = null, object on = null)
        {
            return (bool)(objectSource?.Pick() ?? false);
        }
    }
    [Serializable, TypeRegistryItem("系统物体转字符串")]
    public class PointerForString_FromSystemObject : IPointerForString_Only
    {
        [SerializeReference, LabelText("源object")]
        public IPointer objectSource;
        public string Pick(object by = null, object yarn = null, object on = null)
        {
            return (objectSource?.Pick() ?? "").ToString();
        }
    }
    [Serializable, TypeRegistryItem("系统物体转Unity物体")]
    public class PointerForUnityObject_FromSystemObject : IPointerForUnityObject_Only
    {
        [SerializeReference, LabelText("源object")]
        public IPointer objectSource;
        public UnityEngine.Object Pick(object by = null, object yarn = null, object on = null)
        {
            return (objectSource?.Pick() ?? null) as UnityEngine.Object;
        }
    }
    [Serializable, TypeRegistryItem("系统物体转游戏物体")]
    public class PointerForGameObject_FromSystemObject : IPointerForGameObject_Only
    {
        [SerializeReference, LabelText("源object")]
        public IPointer objectSource;
        public GameObject Pick(object by = null, object yarn = null, object on = null)
        {
            return (objectSource?.Pick() ?? null) as GameObject;
        }
    }
    [Serializable, TypeRegistryItem("系统物体转变换")]
    public class PointerForTransform_FromSystemObject : IPointerForTransform_Only
    {
        [SerializeReference, LabelText("源object")]
        public IPointer objectSource;
        public Transform Pick(object by = null, object yarn = null, object on = null)
        {
            return (objectSource?.Pick() ?? null) as Transform;
        }
    }
    [Serializable, TypeRegistryItem("系统物体转四元数")]
    public class PointerForQuaternion_FromSystemObject : IPointerForQuaternion_Only
    {
        [SerializeReference, LabelText("源object")]
        public IPointer objectSource;
        public Quaternion Pick(object by = null, object yarn = null, object on = null)
        {
            object oo = objectSource?.Pick();
            if (oo is Quaternion q) { return q; }
            return Quaternion.identity;
        }
    }
    [Serializable, TypeRegistryItem("系统物体转脚本")]
    public class PointerForCompoent_FromSystemObject : IPointerForComponent_Only
    {
        [SerializeReference, LabelText("源object")]
        public IPointer objectSource;
        public Component Pick(object by = null, object yarn = null, object on = null)
        {

            return (objectSource?.Pick() ?? null) as Component;
        }
    }

    [Serializable, TypeRegistryItem("系统物体转类型")]
    public class PointerForType_FromSystemObject : IPointerForType_Only
    {
        [SerializeReference, LabelText("源object")]
        public IPointer objectSource;
        public Type Pick(object by = null, object yarn = null, object on = null)
        {
            return (objectSource?.Pick() ?? null) as Type;
        }
    }
    #endregion
    #endregion
}
