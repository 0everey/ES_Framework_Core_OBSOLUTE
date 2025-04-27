using DG.Tweening;
using DG.Tweening.Plugins.Core;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using static ES.EvPointer.PointerForIPointerForTween_MakeSequence_PackerSelectSome;
using static UnityEngine.UIElements.UxmlAttributeDescription;

namespace ES.EvPointer
{
    public class PinForDotween : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            var i = transform.DOMove(transform.position, 1);
            //Tween Tween;

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
    //链接者

    //transform by 
    public interface IPointerForTweenByTransform : IPointer<Tween, Transform, object, object>
    {
        object IPointer.Pick(object a, object b, object c)
        {
            return Pick();
        }

    }
    public interface IPointerForTween : IPointer<Tween, object, object, object>
    {
        object IPointer.Pick(object a, object b, object c)
        {
            return Pick();
        }
    }
    public interface IPointerForTweenCallBack : IPointer<TweenCallback, object, object, object>
    {

    }
    public interface IPointerForTweenSome : IPointer<List<Tween>, object, object, object>, IPointerOnlyBackList<Tween>
    {

    }
    public interface IPointerChainForTween : IPointerChain<Tween>
    {
        object IPointer.Pick(object a, object b, object c)
        {
            return Pick();
        }
    }
    #region 包
    [Serializable, TypeRegistryItem("Dotween_<触发针包>_选中一个", "其他插件支持")]
    public class PointerForIPointerForTween_PackerSelect : PointerPackerForOnlySelectBackOne<Tween, IPointerForTween>, IPointerForTween
    {

    }
    [Serializable, TypeRegistryItem("Dotween_<触发针包>_选中几个", "其他插件支持")]
    public class PointerForIPointerForTween_PackerSelectSome : PointerPackerForSelectSomeBack<Tween, IPointerForTween>, IPointerForTweenSome
    {

    }
    //抽象 同By 包
    public abstract class PointerForTween_PackerSelectOneSameBy<By> : PointerPackForSameByOnlyOne<Tween, By, IPointerForTween>, IPointerForTween
    {

        //public abstract By byFrom { get; }
        public Tween Pick(object by = null, object yarn = null, object on = null)
        {
            return (this as PointerPackForSameByOnlyOne<Tween, By, IPointerForTween>)?.Pick(byFrom);
        }
    }

    [Serializable, TypeRegistryItem("Dotween_<触发针包>同By包_选中一个", "其他插件支持")]
    public class PointerForTween_PackerSelectOneSameByTransform : PointerForTween_PackerSelectOneSameBy<Transform>
    {
        [LabelText("同By 变换"), SerializeReference, PropertyOrder(-1), GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_03")] public IPointerForTransform_Only pointerForTransform = new PointerForTransform_Direct();
        public override Transform byFrom => pointerForTransform?.Pick();

        public override Tween PickOne(IPointerForTween use)
        {
            return use?.Pick(byFrom);
        }
    }
    [Serializable, TypeRegistryItem("Dotween_<链式针包>", "其他插件支持")]
    public class PointerForTween_PackerChainTween : PointerPackForChain<Tween, IPointerChainForTween>, IPointerForTween
    {
        [LabelText("链的头部"), SerializeReference, PropertyOrder(-1), GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_03")] public IPointerForTween headGetter;
        public override Tween head => headGetter?.Pick();
    }
    [Serializable, TypeRegistryItem("Dotween_<触发针包>->_序列生成", "其他插件支持")]
    public class PointerForIPointerForTween_MakeSequence_PackerSelectSome : PointerPackerForSelectSome_BackSelfDefine<Tween, IPointer, IPointerForSequenceClip>, IPointerForTween
    {
        public override Tween Addition(List<IPointer> backs)
        {
            Sequence s = DOTween.Sequence();
            foreach (var i in backs)
            {
                if (i != null)
                {
                    if (i is IPointerForTween tweenPointer)
                    {
                        var ttt = tweenPointer.Pick();
                        if (ttt != null)
                        {
                            s.Append(ttt);
                        }
                    }
                    else if (i is IPointerForFloat_Only floatPointer)
                    {
                        var fff = floatPointer.Pick();
                        if (fff > 0)
                        {
                            s.AppendInterval(fff);
                        }
                    }
                    else if (i is IPointerForTweenCallBack callbackPointer)
                    {
                        var bbb = callbackPointer.Pick();
                        if (bbb != null)
                        {
                            s.AppendCallback(bbb);
                        }
                    }

                }
            }
            return s;
        }
    }

    [Serializable, TypeRegistryItem("Dotween_转换_取消byTrans", "其他插件支持")]
    public class PointerForTween_TranByToNo : IPointerForTween
    {
        [SerializeReference, LabelText("带Trans的")]
        public IPointerForTweenByTransform forTrans;
        public Tween Pick(object by = null, object yarn = null, object on = null)
        {
            return forTrans?.Pick();
        }
    }
    public abstract class PointerForTweenByTransform_Reference : IPointerForTweenByTransform, IPointerForTween
    {
        [SerializeReference, LabelText("可选transform源")]
        public IPointerForTransform_Only transform_Only;
        [LabelText("是相对的")]
        public bool isRele = false;
        object IPointer.Pick(object a, object b, object c)
        {
            return Pick(transform_Only?.Pick());
        }
        public virtual Tween Pick(Transform by, object yarn = null, object on = null)
        {
            Transform tt = by;
            if (tt == null)
            {
                tt = transform_Only?.Pick();
            }
            if (tt == null) return null;
            return PickTruely(tt);
        }

        public Tween Pick(object by = null, object yarn = null, object on = null)
        {
            return (this as IPointerForTweenByTransform)?.Pick(transform_Only?.Pick());
        }

        protected abstract Tween PickTruely(Transform by = null, object yarn = null, object on = null);

    }
    public abstract class PointerChainForTween : IPointerChainForTween
    {
        public abstract Tween Pick(Tween by = null, object yarn = null, object on = null);

    }
    #endregion
    #region 协助
    [Serializable, TypeRegistryItem("Dotween_链_基类_引用Tween", "其他插件支持")]
    public class PointerChainForTween_DirectReference : PointerChainForTween, IPointerForTween
    {
        [LabelText("使用Tween"), SerializeReference] public IPointerForTween forTween;
        public override Tween Pick(Tween by, object yarn = null, object on = null)
        {
            return by;
        }
        object IPointer.Pick(object a, object b, object c)
        {
            return Pick(forTween?.Pick());
        }
        //转换 不需要重写了
        public Tween Pick(object by = null, object yarn = null, object on = null)
        {
            return (this as PointerChainForTween)?.Pick(forTween?.Pick());
        }
    }
    [Serializable, TypeRegistryItem("<核>Dotween_调用(byTrans的)", "其他插件支持")]
    public class PickTweenDirect : IPointerNone
    {
        [LabelText("使用补间(ByTrans)"), SerializeReference] public PointerForTweenByTransform_Reference pointerFor;
        public object Pick(object by = null, object yarn = null, object on = null)
        {
            return pointerFor?.Pick();
        }

    }
    [Serializable, TypeRegistryItem("<核>Dotween_调用(纯back的取消trans)", "其他插件支持")]
    public class PickTweenDirect_onlyBack : IPointerNone
    {
        [LabelText("使用补间(for)"), SerializeReference] public IPointerForTween pointerFor;
        public object Pick(object by = null, object yarn = null, object on = null)
        {
            return pointerFor?.Pick();
        }
    }
    [Serializable, TypeRegistryItem("<核>Dotween_执行并携带回调", "其他插件支持")]
    public class PointerForDelegate_DotweenWithCallBack : IPointerForTween
    {
        [LabelText("回调类型")] public EnumCollect.CallBackType callBackType;
        [LabelText("Tween源"), SerializeReference] public IPointerForTween forTween;
        [LabelText("回调内容"), SerializeReference] public IPointerForAction_Only action_Only = new PointerForAction_FromPointerNone();

        public Tween Pick(object by = null, object yarn = null, object on = null)
        {
            var use = forTween?.Pick();
            if (use == null) return null;
            Action action = action_Only?.Pick();
            if (action == null) return use;
            KeyValueMatchingUtility.Function.SetCallBackFromTween(use, callBackType, new TweenCallback(() => { action?.Invoke(); }));
            return use;
        }
    }
    #endregion
    #region ForTweenByTrans And ForTween单值针
    [Serializable, TypeRegistryItem("Dotween_变换_简单位移", "其他插件支持")]
    public class PointerForTweenByTransform_DoMoveEasy : PointerForTweenByTransform_Reference
    {
        [LabelText("空间")]
        public EnumCollect.TransformHandle_ValueRele space;
        [LabelText("Vector3数值"), SerializeReference]
        public IPointerForVector3_Only vector3_Only = new PointerForVector3_Direct();
        [LabelText("持续时间(默认1)"), SerializeReference]
        public IPointerForFloat_Only float_Only = new PointerForFloat_Direct() { float_ = 1 };

        protected override Tween PickTruely(Transform by = null, object yarn = null, object on = null)
        {

            if (by == null) return null;
            Tween tt;
            Vector3 v3 = vector3_Only?.Pick() ?? default;
            float d = float_Only?.Pick() ?? 1;
            if (space == EnumCollect.TransformHandle_ValueRele.WorldSpace)
            {
                tt = by.DOMove(v3, d).SetRelative(isRele);

            }
            else
            {
                tt = by.DOLocalMove(v3, d).SetRelative(isRele);
            }
            return tt;
        }
    }
    [Serializable, TypeRegistryItem("Dotween_变换_简单 位置振动Vector3 ", "其他插件支持")]
    public class PointerForTweenByTransform_DoPosShake_Vector3Easy : PointerForTweenByTransform_Reference
    {
        [LabelText("强度值三方向"), SerializeReference]
        public IPointerForVector3_Only v3_value = new PointerForVector3_Direct();
        [LabelText("持续时间(默认1)"), SerializeReference]
        public IPointerForFloat_Only float_Only = new PointerForFloat_Direct() { float_ = 1 };
        [LabelText("淡入淡出")]
        public bool fade = true;

        protected override Tween PickTruely(Transform by = null, object yarn = null, object on = null)
        {
            if (by == null) return null;
            Tween tt;
            Vector3 f = v3_value?.Pick() ?? Vector3.one;
            float d = float_Only?.Pick() ?? 1;
            tt = by.DOShakePosition(d, f, fadeOut: fade).SetRelative(isRele);
            return tt;
        }
    }
    [Serializable, TypeRegistryItem("Dotween_变换_简单位移振动Float ", "其他插件支持")]
    public class PointerForTweenByTransform_DoPosShake_FloatEasy : PointerForTweenByTransform_Reference
    {

        [LabelText("强度值"), SerializeReference]
        public IPointerForFloat_Only float_value = new PointerForFloat_Direct();
        [LabelText("持续时间(默认1)"), SerializeReference]
        public IPointerForFloat_Only float_Only = new PointerForFloat_Direct() { float_ = 1 };
        [LabelText("淡入淡出")]
        public bool fade = true;

        protected override Tween PickTruely(Transform by = null, object yarn = null, object on = null)
        {
            if (by == null) return null;
            Tween tt;
            float f = float_value?.Pick() ?? 1;
            float d = float_Only?.Pick() ?? 1;
            tt = by.DOShakePosition(d, f, fadeOut: fade).SetRelative(isRele);
            return tt;
        }
    }
    [Serializable, TypeRegistryItem("Dotween_变换_简单旋转四元 ", "其他插件支持")]
    public class PointerForTweenByTransform_DoRotQuaterEasy : PointerForTweenByTransform_Reference
    {
        [LabelText("空间")]
        public EnumCollect.TransformHandle_ValueRele space;
        [LabelText("四元数数值"), SerializeReference]
        public IPointerForQuaternion_Only vector3_Only = new PointerForQuaternion_Direc();
        [LabelText("持续时间(默认1)"), SerializeReference]
        public IPointerForFloat_Only float_Only = new PointerForFloat_Direct() { float_ = 1 };

        protected override Tween PickTruely(Transform by = null, object yarn = null, object on = null)
        {
            if (by == null) return null;
            Tween tt;
            Quaternion q = vector3_Only?.Pick() ?? default;
            float d = float_Only?.Pick() ?? 1;
            if (space == EnumCollect.TransformHandle_ValueRele.WorldSpace)
            {
                tt = by.DORotateQuaternion(q, d).SetRelative(isRele);
            }
            else
            {
                tt = by.DOLocalRotateQuaternion(q, d).SetRelative(isRele);
            }
            return tt;
        }
    }
    [Serializable, TypeRegistryItem("Dotween_变换_简单旋转振动Vector3 ", "其他插件支持")]
    public class PointerForTweenByTransform_DoRotShake_Vector3Easy : PointerForTweenByTransform_Reference
    {
        [LabelText("强度值三方向"), SerializeReference]
        public IPointerForVector3_Only v3_value = new PointerForVector3_Direct();
        [LabelText("持续时间(默认1)"), SerializeReference]
        public IPointerForFloat_Only float_Only = new PointerForFloat_Direct() { float_ = 1 };
        [LabelText("淡入淡出")]
        public bool fade = true;

        protected override Tween PickTruely(Transform by = null, object yarn = null, object on = null)
        {
            if (by == null) return null;
            Tween tt;
            Vector3 f = v3_value?.Pick() ?? Vector3.one;
            float d = float_Only?.Pick() ?? 1;
            tt = by.DOShakeRotation(d, f, fadeOut: fade).SetRelative(isRele);
            return tt;
        }
    }
    [Serializable, TypeRegistryItem("Dotween_变换_简单旋转振动Float ", "其他插件支持")]
    public class PointerForTweenByTransform_DoRotShake_FloatEasy : PointerForTweenByTransform_Reference
    {

        [LabelText("强度值"), SerializeReference]
        public IPointerForFloat_Only float_value = new PointerForFloat_Direct();
        [LabelText("持续时间(默认1)"), SerializeReference]
        public IPointerForFloat_Only float_Only = new PointerForFloat_Direct() { float_ = 1 };
        [LabelText("淡入淡出")]
        public bool fade = true;

        protected override Tween PickTruely(Transform by = null, object yarn = null, object on = null)
        {
            if (by == null) return null;
            Tween tt;
            float f = float_value?.Pick() ?? 1;
            float d = float_Only?.Pick() ?? 1;
            tt = by.DOShakeRotation(d, f, fadeOut: fade).SetRelative(isRele);
            return tt;
        }
    }
    [Serializable, TypeRegistryItem("Dotween_变换_简单旋转Vector3 ", "其他插件支持")]
    public class PointerForTweenByTransform_DoRotEasy : PointerForTweenByTransform_Reference
    {
        [LabelText("空间")]
        public EnumCollect.TransformHandle_ValueRele space;
        [LabelText("Vector3数值"), SerializeReference]
        public IPointerForVector3_Only vector3_Only = new PointerForVector3_Direct();
        [LabelText("持续时间(默认1)"), SerializeReference]
        public IPointerForFloat_Only float_Only = new PointerForFloat_Direct() { float_ = 1 };

        protected override Tween PickTruely(Transform by = null, object yarn = null, object on = null)
        {
            if (by == null) return null;
            Tween tt;
            Vector3 v3 = vector3_Only?.Pick() ?? default;
            float d = float_Only?.Pick() ?? 1;
            if (space == EnumCollect.TransformHandle_ValueRele.WorldSpace)
            {
                tt = by.DORotate(v3, d).SetRelative(isRele);
            }
            else
            {
                tt = by.DOLocalRotate(v3, d).SetRelative(isRele);
            }
            return tt;
        }
    }
    [Serializable, TypeRegistryItem("Dotween_变换_简单缩放Vector3(局部) ", "其他插件支持")]
    public class PointerForTweenByTransform_DoScaleEasy : PointerForTweenByTransform_Reference
    {
        [LabelText("Vector3数值"), SerializeReference]
        public IPointerForVector3_Only vector3_Only = new PointerForVector3_Direct();
        [LabelText("持续时间(默认1)"), SerializeReference]
        public IPointerForFloat_Only float_Only = new PointerForFloat_Direct() { float_ = 1 };

        protected override Tween PickTruely(Transform by = null, object yarn = null, object on = null)
        {
            if (by == null) return null;
            Tween tt;
            Vector3 v3 = vector3_Only?.Pick() ?? default;
            float d = float_Only?.Pick() ?? 1;
            tt = by.DOScale(v3, d).SetRelative(isRele);
            return tt;
        }
    }
    [Serializable, TypeRegistryItem("Dotween_变换_简单缩放Float(局部) ", "其他插件支持")]
    public class PointerForTweenByTransform_DoScale_FloatEasy : PointerForTweenByTransform_Reference
    {
        [LabelText("空间")]
        public EnumCollect.TransformHandle_ValueRele space;
        [LabelText("浮点数数值"), SerializeReference]
        public IPointerForFloat_Only float_value = new PointerForFloat_Direct();
        [LabelText("持续时间(默认1)"), SerializeReference]
        public IPointerForFloat_Only float_Only = new PointerForFloat_Direct() { float_ = 1 };

        protected override Tween PickTruely(Transform by = null, object yarn = null, object on = null)
        {
            if (by == null) return null;
            Tween tt;
            float f = float_value?.Pick() ?? 1;
            float d = float_Only?.Pick() ?? 1;
            if (space == EnumCollect.TransformHandle_ValueRele.WorldSpace)
            {
                tt = by.DOScale(f, d).SetRelative(isRele);
            }
            else
            {
                tt = by.DOScale(f, d).SetRelative(isRele);
            }
            return tt;
        }
    }
    [Serializable, TypeRegistryItem("Dotween_变换_简单缩放振动Float(局部) ", "其他插件支持")]
    public class PointerForTweenByTransform_DoScaleShake_FloatEasy : PointerForTweenByTransform_Reference
    {

        [LabelText("强度值"), SerializeReference]
        public IPointerForFloat_Only float_value = new PointerForFloat_Direct();
        [LabelText("持续时间(默认1)"), SerializeReference]
        public IPointerForFloat_Only float_Only = new PointerForFloat_Direct() { float_ = 1 };
        [LabelText("淡入淡出")]
        public bool fade = true;

        protected override Tween PickTruely(Transform by = null, object yarn = null, object on = null)
        {
            if (by == null) return null;
            Tween tt;
            float f = float_value?.Pick() ?? 1;
            float d = float_Only?.Pick() ?? 1;
            tt = by.DOShakeScale(d, f, fadeOut: fade).SetRelative(isRele);
            return tt;
        }
    }
    [Serializable, TypeRegistryItem("Dotween_变换_简单缩放振动Vector3(局部) ", "其他插件支持")]
    public class PointerForTweenByTransform_DoScaleShake_Vector3Easy : PointerForTweenByTransform_Reference
    {

        [LabelText("强度值三方向"), SerializeReference]
        public IPointerForVector3_Only v3_value = new PointerForVector3_Direct();
        [LabelText("持续时间(默认1)"), SerializeReference]
        public IPointerForFloat_Only float_Only = new PointerForFloat_Direct() { float_ = 1 };
        [LabelText("淡入淡出")]
        public bool fade = true;

        protected override Tween PickTruely(Transform by = null, object yarn = null, object on = null)
        {
            if (by == null) return null;
            Tween tt;
            Vector3 f = v3_value?.Pick() ?? Vector3.one;
            float d = float_Only?.Pick() ?? 1;
            tt = by.DOShakeScale(d, f, fadeOut: fade).SetRelative(isRele);
            return tt;
        }
    }
    [Serializable, TypeRegistryItem("Dotween_变换_混合位移", "其他插件支持")]
    public class PointerForTweenByTransform_DoMoveBlend : PointerForTweenByTransform_Reference
    {
        [LabelText("空间")]
        public EnumCollect.TransformHandle_ValueRele space;
        [LabelText("Vector3数值"), SerializeReference]
        public IPointerForVector3_Only vector3_Only = new PointerForVector3_Direct();
        [LabelText("持续时间(默认1)"), SerializeReference]
        public IPointerForFloat_Only float_Only = new PointerForFloat_Direct() { float_ = 1 };

        protected override Tween PickTruely(Transform by = null, object yarn = null, object on = null)
        {

            if (by == null) return null;
            Tween tt;
            Vector3 v3 = vector3_Only?.Pick() ?? default;
            float d = float_Only?.Pick() ?? 1;
            if (space == EnumCollect.TransformHandle_ValueRele.WorldSpace)
            {
                tt = by.DOBlendableMoveBy(v3, d).SetRelative(isRele);

            }
            else
            {
                tt = by.DOBlendableLocalMoveBy(v3, d).SetRelative(isRele);
            }
            return tt;
        }
    }
    /* [Serializable, TypeRegistryItem("Dotween_变换_混合位置振动Vector3 ", "其他插件支持")]
     public class PointerForTweenByTransform_DoPosShake_Vector3Blend : PointerForTweenByTransform_Reference
     {
         [LabelText("强度值三方向"), SerializeReference]
         public IPointerForVector3_Only v3_value = new PointerForVector3_Direct();
         [LabelText("持续时间(默认1)"), SerializeReference]
         public IPointerForFloat_Only ob = new PointerForFloat_Direct() { float_ = 1 };
         [LabelText("淡入淡出")]
         public bool fade = true;

         protected override Tween PickTruely(Transform by = null, object yarn = null, object on = null)
         {
             if (by == null) return null;
             Tween tt;
             Vector3 f = v3_value?.Pick() ?? Vector3.one;
             float d = ob?.Pick() ?? 1;
             tt = by.DOShakePosition(d, f, fadeOut: fade).SetRelative(isRele);
             return tt;
         }
     }*/
    /* [Serializable, TypeRegistryItem("Dotween_变换_混合位移振动Float ", "其他插件支持")]
     public class PointerForTweenByTransform_DoPosShake_FloatBlend : PointerForTweenByTransform_Reference
     {

         [LabelText("强度值"), SerializeReference]
         public IPointerForFloat_Only float_value = new PointerForFloat_Direct();
         [LabelText("持续时间(默认1)"), SerializeReference]
         public IPointerForFloat_Only ob = new PointerForFloat_Direct() { float_ = 1 };
         [LabelText("淡入淡出")]
         public bool fade = true;

         protected override Tween PickTruely(Transform by = null, object yarn = null, object on = null)
         {
             if (by == null) return null;
             Tween tt;
             float f = float_value?.Pick() ?? 1;
             float d = ob?.Pick() ?? 1;
             tt = by.DOShakePosition(d, f, fadeOut: fade).SetRelative(isRele);
             return tt;
         }
     }*/
    [Serializable, TypeRegistryItem("Dotween_变换_混合旋转Vector3 ", "其他插件支持")]
    public class PointerForTweenByTransform_DoRotBlend : PointerForTweenByTransform_Reference
    {
        [LabelText("空间")]
        public EnumCollect.TransformHandle_ValueRele space;
        [LabelText("Vector3数值"), SerializeReference]
        public IPointerForVector3_Only vector3_Only = new PointerForVector3_Direct();
        [LabelText("持续时间(默认1)"), SerializeReference]
        public IPointerForFloat_Only float_Only = new PointerForFloat_Direct() { float_ = 1 };

        protected override Tween PickTruely(Transform by = null, object yarn = null, object on = null)
        {
            if (by == null) return null;
            Tween tt;
            Vector3 v3 = vector3_Only?.Pick() ?? default;
            float d = float_Only?.Pick() ?? 1;
            if (space == EnumCollect.TransformHandle_ValueRele.WorldSpace)
            {
                tt = by.DOBlendableRotateBy(v3, d).SetRelative(isRele);
            }
            else
            {
                tt = by.DOBlendableLocalRotateBy(v3, d).SetRelative(isRele);
            }
            return tt;
        }
    }
    [Serializable, TypeRegistryItem("Dotween_变换_混合缩放Vector3(局部) ", "其他插件支持")]
    public class PointerForTweenByTransform_DoScaleBlend : PointerForTweenByTransform_Reference
    {
        [LabelText("Vector3数值"), SerializeReference]
        public IPointerForVector3_Only vector3_Only = new PointerForVector3_Direct();
        [LabelText("持续时间(默认1)"), SerializeReference]
        public IPointerForFloat_Only float_Only = new PointerForFloat_Direct() { float_ = 1 };

        protected override Tween PickTruely(Transform by = null, object yarn = null, object on = null)
        {
            if (by == null) return null;
            Tween tt;
            Vector3 v3 = vector3_Only?.Pick() ?? default;
            float d = float_Only?.Pick() ?? 1;
            tt = by.DOBlendableScaleBy(v3, d).SetRelative(isRele);
            return tt;
        }
    }
    [Serializable, TypeRegistryItem("Dotween_变换_路径_抽象", "其他插件支持")]
    public abstract class PointerForTweenByTransform_Path : PointerForTweenByTransform_Reference
    {
        public abstract List<Vector3> path();
        [LabelText("是否为世界坐标")] public bool IsWorldSpace = true;
        [LabelText("路径处理设置")]public EnumCollect.PathSortType PathSort;
        //[LabelText("是否打乱终点")] public bool RandomSortPath = false;
        [LabelText("选择路径模式")] public PathMode pathMode;
        [LabelText("选择路径类型")] public PathType pathType;
        [LabelText("是否使用DebugGizmos")] public bool useGimoz = false;
        [ShowIfGroup("color", VisibleIf = "useGimoz"), LabelText("使用的颜色"), ColorUsage(true)] public Color color = Color.white;
        
        [LabelText("持续时间(默认1)"), SerializeReference]
        public IPointerForFloat_Only float_Only = new PointerForFloat_Direct() { float_ = 1 };

        protected override Tween PickTruely(Transform by = null, object yarn = null, object on = null)
        {
            if (by == null) return null;
            Vector3[] use = PathSort== EnumCollect.PathSortType.NoneSort? path().ToArray():KeyValueMatchingUtility.Sorter.SortPath(path(),PathSort,by.position,by).ToArray();
            float d = float_Only?.Pick() ?? 1;
            if (use != null && use.Length > 0)
            {
                if (IsWorldSpace)
                    return by.DOPath(use, d, pathType, pathMode, gizmoColor: useGimoz ? color : null);
                else return by.DOLocalPath(use, d, pathType, pathMode, gizmoColor: useGimoz ? color : null);

            }
            return null;
        }
    }
    [Serializable, TypeRegistryItem("Dotween_变换_路径_直接Transform列表")]
    public class PointerForTweenByTransform_Path_TransformList : PointerForTweenByTransform_Path
    {
        [LabelText("变换点列表")]
        public List<Transform> transforms = new List<Transform>();
        [LabelText("采样为世界坐标？")]
        public bool asWorld = true;
        public override List<Vector3> path()
        {
            List<Vector3> vector3s = new List<Vector3>(10);
            foreach(var i in transforms)
            {
                if (i != null)
                {
                    vector3s.Add(asWorld? i.position:i.localPosition);
                }
            }
            
            return vector3s;
        }
    }

    #endregion
    #region Chain Tween链式
    [Serializable, TypeRegistryItem("Dotween_链式_设置为相对的", "其他插件支持")]
        public class PointerChainForTween_SetReletive : PointerChainForTween
        {
            [LabelText("是相对的？")]
            public bool rele = false;
            public override Tween Pick(Tween by = null, object yarn = null, object on = null)
            {
                return by?.SetRelative(rele);
            }
        }
        [Serializable, TypeRegistryItem("Dotween_链式_可回收", "其他插件支持")]
        public class PointerChainForTween_SetRecyclable : PointerChainForTween
        {
            [LabelText("可被KIll回收？")]
            public bool recy = false;
            public override Tween Pick(Tween by = null, object yarn = null, object on = null)
            {
                return by?.SetRecyclable(recy);
            }
        }
        [Serializable, TypeRegistryItem("Dotween_链式_回退", "其他插件支持")]
        public class PointerChainForTween_SetRewind : PointerChainForTween
        {
            [LabelText("回退包含延迟？")]
            public bool includeDelay = false;
            public override Tween Pick(Tween by = null, object yarn = null, object on = null)
            {
                by?.Rewind(includeDelay);
                return by;
            }
        }
        [Serializable, TypeRegistryItem("Dotween_链式_重启", "其他插件支持")]
        public class PointerChainForTween_SetReStart : PointerChainForTween
        {
            [LabelText("回退包含延迟？")]
            public bool includeDelay = false;
            [LabelText("改变延迟为")]
            public float delay = -1;
            public override Tween Pick(Tween by = null, object yarn = null, object on = null)
            {
                by?.Restart(includeDelay, delay);
                return by;
            }
        }
        [Serializable, TypeRegistryItem("Dotween_链式_自动销毁", "其他插件支持")]
        public class PointerChainForTween_SetAutoKill : PointerChainForTween
        {
            [LabelText("销毁")]
            public bool autiKill = false;
            public override Tween Pick(Tween by = null, object yarn = null, object on = null)
            {
                by?.SetAutoKill(autiKill);
                return by;
            }
        }
        [Serializable, TypeRegistryItem("Dotween_链式_设置延迟", "其他插件支持")]
        public class PointerChainForTween_SetDelay : PointerChainForTween
        {
            [LabelText("延迟事件"), SerializeReference]
            public IPointerForFloat_Only float_Only = new PointerForFloat_Direct() { float_ = 1 };
            public override Tween Pick(Tween by = null, object yarn = null, object on = null)
            {
                return by?.SetDelay(float_Only?.Pick() ?? 1);
            }
        }
        [Serializable, TypeRegistryItem("Dotween_链式_设置循环(-1无限)", "其他插件支持")]
        public class PointerChainForTween_SetLoops : PointerChainForTween
        {
            [LabelText("循环次数"), SerializeReference]
            public IPointerForInt_Only int_Only = new PointerForInt_Direct() { int_ = 5 };
            [LabelText("循环类型")]
            public LoopType type;
            public override Tween Pick(Tween by = null, object yarn = null, object on = null)
            {
                return by?.SetLoops(int_Only?.Pick() ?? 5, type);
            }
        }
        [Serializable, TypeRegistryItem("Dotween_链式_设置ID", "其他插件支持")]
        public class PointerChainForTween_SetID_ : PointerChainForTween
        {
            [LabelText("ID 任意"), SerializeReference]
            public IPointer ob;

            public override Tween Pick(Tween by = null, object yarn = null, object on = null)
            {
                return by?.SetId(ob?.Pick() ?? 0);
            }
        }
        [Serializable, TypeRegistryItem("Dotween_链式_设置目标", "其他插件支持")]
        public class PointerChainForTween_SetTarget_ : PointerChainForTween
        {
            [LabelText("目标 任意"), SerializeReference]
            public IPointer ob;

            public override Tween Pick(Tween by = null, object yarn = null, object on = null)
            {
                return by?.SetTarget(ob?.Pick() ?? null);
            }
        }
        [Serializable, TypeRegistryItem("Dotween_链式_运行开关切换", "其他插件支持")]
        public class PointerChainForTween_TooglePause_ : PointerChainForTween
        {
            public override Tween Pick(Tween by = null, object yarn = null, object on = null)
            {
                by?.TogglePause();
                return by;
            }
        }
        [Serializable, TypeRegistryItem("Dotween_链式_立刻完成", "其他插件支持")]
        public class PointerChainForTween_Complete_ : PointerChainForTween
        {
            public override Tween Pick(Tween by = null, object yarn = null, object on = null)
            {
                by?.Complete();
                return by;
            }
        }
        [Serializable, TypeRegistryItem("Dotween_链式_翻转方向", "其他插件支持")]
        public class PointerChainForTween_Flip : PointerChainForTween
        {
            public override Tween Pick(Tween by = null, object yarn = null, object on = null)
            {
                by?.Flip();
                return by;
            }
        }
        [Serializable, TypeRegistryItem("Dotween_链式_强制初始化", "其他插件支持")]
        public class PointerChainForTween_ForceInit : PointerChainForTween
        {
            public override Tween Pick(Tween by = null, object yarn = null, object on = null)
            {
                by?.ForceInit();
                return by;
            }
        }
        [Serializable, TypeRegistryItem("Dotween_链式_目标为起点(From)", "其他插件支持")]
        public class PointerChainForTween_From : PointerChainForTween
        {
            public override Tween Pick(Tween by = null, object yarn = null, object on = null)
            {
                if (by is Tweener tweener)
                {
                    return tweener.From();
                }
                return by;
            }
        }
    [Serializable, TypeRegistryItem("Dotween_链式_添加回调(传递)", "其他插件支持")]
    public class PointerChainForTween_CallBack : PointerChainForTween
    {
        [LabelText("回调类型")] public EnumCollect.CallBackType callBackType;
        [LabelText("回调内容"), SerializeReference] public IPointerForAction_Only action_Only = new PointerForAction_FromPointerNone();
        public override Tween Pick(Tween by = null, object yarn = null, object on = null)
        {
            if (by is Tweener tweener&& action_Only!=null)
            {
               Action action = action_Only?.Pick();
               KeyValueMatchingUtility.Function.SetCallBackFromTween(tweener, callBackType,new TweenCallback(() => { action?.Invoke(); }));
            }
            return by;
        }
    }
    #endregion
    #region Sequence
    public interface IPointerForSequenceClip : IPointerForIPointer_Only
        {

        }
        [Serializable, TypeRegistryItem("Dotween_序列剪影_Tween")]
        public class PointerForSequenceClip_Tween : IPointerForSequenceClip
        {
            [LabelText("嵌入Tween"), SerializeReference] public IPointerForTween forTween;


            public IPointer Pick(object by = null, object yarn = null, object on = null)
            {
                return forTween;
            }
        }
        [Serializable, TypeRegistryItem("Dotween_序列剪影_回调", "其他插件支持")]
        public class PointerForSequenceClip_TweenCallBack : IPointerForSequenceClip
        {
            [LabelText("嵌入TweenCallBack"), SerializeReference] public IPointerForTweenCallBack forTweenCallBack;

            public IPointer Pick(object by = null, object yarn = null, object on = null)
            {
                return forTweenCallBack;
            }
        }
        [Serializable, TypeRegistryItem("Dotween_序列剪影_时间间隔", "其他插件支持")]
        public class PointerForSequenceClip_Interval : IPointerForSequenceClip
        {
            [LabelText("嵌入时间间隔"), SerializeReference] public IPointerForFloat_Only forInterval = new PointerForFloat_Direct() { float_ = 1 };

            public IPointer Pick(object by = null, object yarn = null, object on = null)
            {
                return forInterval;
            }
        }
    #endregion
    #region 回调
    [Serializable,TypeRegistryItem("Dotween_获取携带回调", "其他插件支持")]
    public class PointerForDelegate_DotweenCallBack : IPointerForDelegate_Only
    {
        [LabelText("回调类型")] public EnumCollect.CallBackType callBackType;
        [LabelText("Tween源"), SerializeReference] public IPointerForTween forTween;
        public Delegate Pick(object by = null, object yarn = null, object on = null)
        {
            var use = forTween?.Pick();
            return KeyValueMatchingUtility.Function.GetCallBackFromTween(use,callBackType);
        }
    }

    #endregion
    #region 全局或者局部操作
    [Serializable, TypeRegistryItem("Dotween_局部_完成", "其他插件支持")]
    public class PointerNone_DoTweenHandle_Complete : IPointerNone
    {
        [LabelText("是否执行回调")]public bool withCallBack;
        [LabelText("施加的对象"), SerializeReference] public IPointer pointerForSystemObject_Only;

        public object Pick(object by = null, object yarn = null, object on = null)
        {
            object oo = pointerForSystemObject_Only?.Pick();
            if (oo != null)
            {
                return DOTween.Complete(oo, withCallBack);
            }
            return -1;
        }
    }
    [Serializable, TypeRegistryItem("Dotween_局部_翻转", "其他插件支持")]
    public class PointerNone_DoTweenHandle_Flip : IPointerNone
    {
        
        [LabelText("施加的对象"), SerializeReference] public IPointer pointerForSystemObject_Only;

        public object Pick(object by = null, object yarn = null, object on = null)
        {
            object oo = pointerForSystemObject_Only?.Pick();
            if (oo != null)
            {
                return DOTween.Flip(oo);
            }
            return -1;
        }
    }
    [Serializable, TypeRegistryItem("Dotween_局部_重启", "其他插件支持")]
    public class PointerNone_DoTweenHandle_ReStart: IPointerNone
    {

        [LabelText("施加的对象"), SerializeReference] public IPointer pointerForSystemObject_Only;

        public object Pick(object by = null, object yarn = null, object on = null)
        {
            object oo = pointerForSystemObject_Only?.Pick();
            if (oo != null)
            {
                return DOTween.Restart(oo);
            }
            return -1;
        }
    }
    [Serializable, TypeRegistryItem("Dotween_局部_回退", "其他插件支持")]
    public class PointerNone_DoTweenHandle_Rewind : IPointerNone
    {

        [LabelText("施加的对象"), SerializeReference] public IPointer pointerForSystemObject_Only;

        public object Pick(object by = null, object yarn = null, object on = null)
        {
            object oo = pointerForSystemObject_Only?.Pick();
            if (oo != null)
            {
                return DOTween.Rewind(oo);
            }
            return -1;
        }
    }
    [Serializable, TypeRegistryItem("Dotween_局部_平滑回退", "其他插件支持")]

    public class PointerNone_DoTweenHandle_SmoothRewind : IPointerNone
    {

        [LabelText("施加的对象"), SerializeReference] public IPointer pointerForSystemObject_Only;

        public object Pick(object by = null, object yarn = null, object on = null)
        {
            object oo = pointerForSystemObject_Only?.Pick();
            if (oo != null)
            {
                return DOTween.SmoothRewind(oo);
            }
            return -1;
        }
    }
    [Serializable, TypeRegistryItem("Dotween_局部_击杀销毁", "其他插件支持")]
    public class PointerNone_DoTweenHandle_Kill : IPointerNone
    {
        [LabelText("是否执行回调")] public bool withCallBack;
        [LabelText("施加的对象"), SerializeReference] public IPointer pointerForSystemObject_Only;
        public object Pick(object by = null, object yarn = null, object on = null)
        {
            object oo = pointerForSystemObject_Only?.Pick();
            if (oo != null)
            {
                return DOTween.Kill(oo, withCallBack);
            }
            return -1;
        }
    }
    [Serializable, TypeRegistryItem("Dotween_全局_完成全部", "其他插件支持")]
    public class PointerNone_DoTweenHandle_CompleteAll : IPointerNone
    {
        [LabelText("是否执行回调")] public bool withCallBack;
        public object Pick(object by = null, object yarn = null, object on = null)
        {
            return DOTween.CompleteAll(withCallBack);
        }
    }
    [Serializable, TypeRegistryItem("Dotween_全局_翻转全部", "其他插件支持")]
    public class PointerNone_DoTweenHandle_FlipAll : IPointerNone
    {
        [LabelText("是否执行回调")] public bool withCallBack;
        public object Pick(object by = null, object yarn = null, object on = null)
        {
            return DOTween.FlipAll();
        }
    }
    [Serializable, TypeRegistryItem("Dotween_全局_重启全部", "其他插件支持")]
    public class PointerNone_DoTweenHandle_ReStartAll : IPointerNone
    {
        [LabelText("是否执行回调")] public bool withCallBack;
        public object Pick(object by = null, object yarn = null, object on = null)
        {
            return DOTween.RestartAll();
        }
    }
    [Serializable, TypeRegistryItem("Dotween_全局_回退全部", "其他插件支持")]
    public class PointerNone_DoTweenHandle_RewindAll : IPointerNone
    {
        [LabelText("是否执行回调")] public bool withCallBack;
        public object Pick(object by = null, object yarn = null, object on = null)
        {
            return DOTween.RewindAll();
        }
    }
    [Serializable, TypeRegistryItem("Dotween_全局_平滑回退全部", "其他插件支持")]
    public class PointerNone_DoTweenHandle_SmoothRewindAll : IPointerNone
    {
        [LabelText("是否执行回调")] public bool withCallBack;
        public object Pick(object by = null, object yarn = null, object on = null)
        {
            return DOTween.SmoothRewindAll();
        }
    }
    [Serializable, TypeRegistryItem("Dotween_全局_击杀销毁全部", "其他插件支持")]
    public class PointerNone_DoTweenHandle_KillAll : IPointerNone
    {
        [LabelText("是否执行回调")] public bool withCallBack;
        public object Pick(object by = null, object yarn = null, object on = null)
        {
            return DOTween.KillAll(withCallBack);
        }
    }

    #endregion
    #region To相关——Caster 变域 反射器
    [Serializable, TypeRegistryItem("Dotween_To_单纯案例", "其他插件支持")]
    public abstract class PointerNone_DotweenTo_Example : IPointerForFloat_Only
    {
        [LabelText("是否执行回调")] public float caster;
        [LabelText("Get器"), SerializeReference] public IPointerForFloat_Only forFloat_Only = new PointerForFloat_Direct() { float_ = 0 };
        [LabelText("目标值"), SerializeReference] public IPointerForFloat_Only forFloat_Only_EndValue = new PointerForFloat_Direct() { float_ = 0 };
        [LabelText("持续时间"), SerializeReference] public IPointerForFloat_Only forFloat_Only_duation = new PointerForFloat_Direct() { float_ = 0 };

        public float Pick(object by = null, object yarn = null, object on = null)
        {

            if (forFloat_Only != null&& forFloat_Only_EndValue!=null)
            {
                DOTween.To(() => forFloat_Only.Pick(),(c)=>c++, forFloat_Only_EndValue.Pick(), forFloat_Only_duation?.Pick()??1);
            }
            return -1;
        }
    }

    //演示
    public abstract class PointerNone_Dotween_To<T> : IPointerForTween, IPointerCaster<T>
    {
        public abstract float durationGet { get; }
        public abstract T targetValue { get; }
        public abstract T getRunTimeValue { get; }
        public abstract T getStartValue { get; }
        public virtual void SetValue(T t) {
            caster = t;
            if (usePlayerCaster && playerCaster_ != null)
            {
                playerCaster_.Recieve(caster);
            }
        }
        public virtual void InitValue()
        {
            caster = getStartValue;
        }
        [LabelText("实时投射值")]public T caster;
        [DetailedInfoBox("", "此处需要引用一个PointerPlayerCaster,把自己的值投射给他它", Message = @"@ ""【绑定投射目标备注："" + (playerCaster != null ? playerCaster.des : ""！未绑定"") ", VisibleIf = "@usePlayerCaster")]
        [LabelText("发起投射?", SdfIconType.At), GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_04")] public bool usePlayerCaster;
        [LabelText("发送到Caster", SdfIconType.At), ShowIf("usePlayerCaster"), GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_04")] public PointerPlayerSystemObjectCaster playerCaster_;
        public PointerPlayerSystemObjectCaster playerCaster => playerCaster_;
        public T Cast()
        {
            return caster; 
        }
        public Tween Pick(object by = null, object yarn = null, object on = null) {

            InitValue();
            return MakeTween();
        }
        public abstract Tween MakeTween();
    }
    public abstract class PointerNone_Dotween_To_Float : PointerNone_Dotween_To<float>,IPointerForFloatCaster
    {
        public override Tween MakeTween()
        {
            return DOTween.To(() => getRunTimeValue, SetValue, targetValue, durationGet);
        }
    }
    //这里每次都重新写太吃操作了，常量可以抽象的，不过反正后面必须复写了懒得改了
    [Serializable,TypeRegistryItem("Dotween_To_浮点值_常量参数", "其他插件支持")]
    public class PointerNone_Dotween_To_Float_EasyConst : PointerNone_Dotween_To_Float
    {
        [LabelText("常量起点值")] public float startValue_ = 0;
        [LabelText("常量终点值")] public float targetValue_ = 0;
        [LabelText("常量持续时间值")] public float targetDuration_ = 1;
        public override float durationGet => targetDuration_;

        public override float targetValue => targetValue_;

        public override float getRunTimeValue => caster;

        public override float getStartValue => startValue_;
    }
    [Serializable, TypeRegistryItem("Dotween_To_浮点值_简单针参数", "其他插件支持")]
    public class PointerNone_Dotween_To_Float_EasyPointer : PointerNone_Dotween_To_Float
    {
        [LabelText("针起点值")] public IPointerForFloat_Only startValue_ = new PointerForFloat_Direct();
        [LabelText("针终点值")] public IPointerForFloat_Only targetValue_ =new PointerForFloat_Direct();
        [LabelText("针持续时间值")] public IPointerForFloat_Only targetDuration_ = new PointerForFloat_Direct() { float_ = 1 };
        public override float durationGet => targetDuration_?.Pick()??1;

        public override float targetValue => targetValue_?.Pick() ?? 1;

        public override float getRunTimeValue => caster;

        public override float getStartValue => startValue_?.Pick() ?? 1;
    }
    public abstract class PointerNone_Dotween_To_Int : PointerNone_Dotween_To<int>, IPointerForIntCaster
    {
        public override Tween MakeTween()
        {
            return DOTween.To(() => getRunTimeValue, SetValue, targetValue, durationGet);
        }
    }
    [Serializable, TypeRegistryItem("Dotween_To__整数值_常量参数", "其他插件支持")]
    public class PointerNone_Dotween_To_Int_EasyConst : PointerNone_Dotween_To_Int
    {
        [LabelText("常量起点值")] public int startValue_ = 0;
        [LabelText("常量终点值")] public int targetValue_ = 0;
        [LabelText("常量持续时间值")] public float targetDuration_ = 1;
        public override float durationGet => targetDuration_;

        public override int targetValue => targetValue_;

        public override int getRunTimeValue => caster;

        public override int getStartValue => startValue_;
    }
    [Serializable, TypeRegistryItem("Dotween_To_整数值_简单针参数", "其他插件支持")]
    public class PointerNone_Dotween_To_Int_EasyPointer : PointerNone_Dotween_To_Int
    {
        [LabelText("针起点值")] public IPointerForInt_Only startValue_ = new PointerForInt_Direct();
        [LabelText("针终点值")] public IPointerForInt_Only targetValue_ = new PointerForInt_Direct();
        [LabelText("针持续时间值")] public IPointerForFloat_Only targetDuration_ = new PointerForFloat_Direct() { float_ = 1  };
        public override float durationGet => targetDuration_?.Pick() ?? 1;

        public override int targetValue => targetValue_?.Pick() ?? 1;

        public override int getRunTimeValue => caster;

        public override int getStartValue => startValue_?.Pick() ?? 1;
    }
    public abstract class PointerNone_Dotween_To_Vector3 : PointerNone_Dotween_To<Vector3>, IPointerForVector3Caster
    {
        public override Tween MakeTween()
        {
            return DOTween.To(() => getRunTimeValue, SetValue, targetValue, durationGet);
        }
    }
    [Serializable, TypeRegistryItem("Dotween_To__Vector3_常量参数", "其他插件支持")]
    public class PointerNone_Dotween_To_Vector3_EasyConst : PointerNone_Dotween_To_Vector3
    {
        [LabelText("常量起点值")] public Vector3 startValue_ = Vector3.zero;
        [LabelText("常量终点值")] public Vector3 targetValue_ = Vector3.zero;
        [LabelText("常量持续时间值")] public float targetDuration_ = 1;
        public override float durationGet => targetDuration_;

        public override Vector3 targetValue => targetValue_;

        public override Vector3 getRunTimeValue => caster;

        public override Vector3 getStartValue => startValue_;
    }
    [Serializable, TypeRegistryItem("Dotween_To_Vector3_简单针参数", "其他插件支持")]
    public class PointerNone_Dotween_To_Vector3_EasyPointer : PointerNone_Dotween_To_Vector3
    {
        [LabelText("针起点值")] public IPointerForVector3_Only startValue_ = new PointerForVector3_Direct();
        [LabelText("针终点值")] public IPointerForVector3_Only targetValue_ = new PointerForVector3_Direct();
        [LabelText("针持续时间值")] public IPointerForFloat_Only targetDuration_ = new PointerForFloat_Direct() { float_ = 1 };
        public override float durationGet => targetDuration_?.Pick() ?? 1;

        public override Vector3 targetValue => targetValue_?.Pick() ?? default;

        public override Vector3 getRunTimeValue => caster;

        public override Vector3 getStartValue => startValue_?.Pick() ?? default;
    }
    public abstract class PointerNone_Dotween_To_Vector2 : PointerNone_Dotween_To<Vector2>, IPointerForVector2Caster
    {
        public override Tween MakeTween()
        {
            return DOTween.To(() => getRunTimeValue, SetValue, targetValue, durationGet);
        }
    }
    [Serializable, TypeRegistryItem("Dotween_To__Vector2_常量参数", "其他插件支持")]
    public class PointerNone_Dotween_To_Vector2_EasyConst : PointerNone_Dotween_To_Vector2
    {
        [LabelText("常量起点值")] public Vector2 startValue_ ;
        [LabelText("常量终点值")] public Vector2 targetValue_ ;
        [LabelText("常量持续时间值")] public float targetDuration_ = 1;
        public override float durationGet => targetDuration_;

        public override Vector2 targetValue => targetValue_;

        public override Vector2 getRunTimeValue => caster;

        public override Vector2 getStartValue => startValue_;
    }
    [Serializable, TypeRegistryItem("Dotween_To_Vector3_简单针参数", "其他插件支持")]
    public class PointerNone_Dotween_To_Vector2_EasyPointer : PointerNone_Dotween_To_Vector2
    {
        [LabelText("针起点值")] public IPointerForVector2_Only startValue_ = new PointerForVector2_Direct();
        [LabelText("针终点值")] public IPointerForVector2_Only targetValue_ = new PointerForVector2_Direct();
        [LabelText("针持续时间值")] public IPointerForFloat_Only targetDuration_ = new PointerForFloat_Direct() { float_ = 1 };
        public override float durationGet => targetDuration_?.Pick() ?? 1;

        public override Vector2 targetValue => targetValue_?.Pick() ?? default;

        public override Vector2 getRunTimeValue => caster;

        public override Vector2 getStartValue => startValue_?.Pick() ?? default;
    }
    public abstract class PointerNone_Dotween_To_Color : PointerNone_Dotween_To<Color>, IPointerForColorCaster
    {
        public override Tween MakeTween()
        {
            return DOTween.To(() => getRunTimeValue, SetValue, targetValue, durationGet);
        }
    }
    [Serializable, TypeRegistryItem("Dotween_To_颜色_常量参数", "其他插件支持")]
    public class PointerNone_Dotween_To_Color_EasyConst : PointerNone_Dotween_To_Color
    {
        [LabelText("常量起点值")] public Color startValue_ = Color.white;
        [LabelText("常量终点值")] public Color targetValue_ = Color.white;
        [LabelText("常量持续时间值")] public float targetDuration_ = 1;
        public override float durationGet => targetDuration_;

        public override Color targetValue => targetValue_;

        public override Color getRunTimeValue => caster;

        public override Color getStartValue => startValue_;
    }
    public abstract class PointerNone_Dotween_To_Quaternion : PointerNone_Dotween_To<Quaternion>, IPointerForQuaternionCaster
    {
        public override Tween MakeTween()
        {
            return DOTween.To(() => getRunTimeValue, SetValue, targetValue.eulerAngles, durationGet);
        }
    }
    [Serializable, TypeRegistryItem("Dotween_To_四元数_常量参数", "其他插件支持")]
    public class PointerNone_Dotween_To_Quaternion_EasyConst : PointerNone_Dotween_To_Quaternion
    {
        [LabelText("常量起点值")] public Quaternion startValue_ ;
        [LabelText("常量终点值")] public Quaternion targetValue_;
        [LabelText("常量持续时间值")] public float targetDuration_ = 1;
        public override float durationGet => targetDuration_;

        public override Quaternion targetValue => targetValue_;

        public override Quaternion getRunTimeValue => caster;

        public override Quaternion getStartValue => startValue_;
    }
    public abstract class PointerNone_Dotween_To_String: PointerNone_Dotween_To<string>, IPointerForStringCaster
    {
        public override Tween MakeTween()
        {
            return DOTween.To(() => getRunTimeValue, SetValue, targetValue, durationGet);
        }
    }
    [Serializable, TypeRegistryItem("Dotween_To_字符串_常量参数", "其他插件支持")]
    public class PointerNone_Dotween_To_String_EasyConst : PointerNone_Dotween_To_String
    {
        [LabelText("常量起点值")] public String startValue_;
        [LabelText("常量终点值")] public String targetValue_;
        [LabelText("常量持续时间值")] public float targetDuration_ = 1;
        public override float durationGet => targetDuration_;

        public override String targetValue => targetValue_;

        public override String getRunTimeValue => caster;

        public override String getStartValue => startValue_;
    }
    #endregion

}
