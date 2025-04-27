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
    //核心 Ev针支持 关于 颜色 渲染 材质 部分
    #region 颜色,渲染和材质设置支持
    #region 接口和抽象定义
    public interface IPointerForRenderer<By, Yarn, On> : IPointer<Renderer, By, Yarn, On>
    {

    }
    public interface IPointerForRenderer_Only : IPointerOnlyBack<Renderer>, IPointerForRenderer<object, object, object>
    {
        object IPointer.Pick(object a, object b, object c)
        {
            return Pick();
        }
    }
    public interface IPointerForColor<By, Yarn, On> : IPointer<Color, By, Yarn, On>
    {

    }
    public interface IPointerForColor_Only : IPointerForColor<object, object, object>, IPointerOnlyBack<Color>
    {
        object IPointer.Pick(object a, object b, object c)
        {
            return Pick();
        }
    }
    #endregion

    #region 颜色,渲染和材质功能
    [Serializable, TypeRegistryItem("颜色获取_直接(旧)", "单值针/颜色针")]
    public class PointerForColor_Direct_old : IPointerForColor_Only
    {
        [LabelText("直接输入颜色"), ColorUsage(true)]
        public Color color = Color.white;
        public Color Pick(object by = null, object yarn = null, object on = null)
        {
            return color;
        }
    }
    [Serializable, TypeRegistryItem("颜色获取_直接(新)", "单值针/颜色针")]
    public class PointerForColor_Direct_new : IPointerForColor_Only
    {
        [LabelText("直接输入颜色"), ColorPalette("色盘", ShowAlpha = true)]
        public Color color = Color.white;
        public Color Pick(object by = null, object yarn = null, object on = null)
        {
            return color;
        }
    }
    [Serializable, TypeRegistryItem("颜色_直接输入", "单值针/颜色针")]
    public class PointerForColor_Direc : IPointerForColor_Only
    {
        [LabelText("直接输入颜色"), ColorPalette] public Color color;
        public Color Pick(object by = null, object yarn = null, object on = null)
        {
            return color;
        }
    }
    [Serializable, TypeRegistryItem("颜色_渐变(直接输入颜色)", "单值针/颜色针")]
    public class PointerForColor_DirectLerp : IPointerForColor_Only,IPointerForColorCaster
    {
        [LabelText("输入颜色起点"), ColorPalette] public Color color=Color.white;
        [LabelText("输入颜色终点"), ColorPalette] public Color color2=Color.black;
        [LabelText("Lerp值"), SerializeReference] public IPointerForFloat_Only lerp = new PointerForFloat_Direct();
        [LabelText("上次输出颜色")]public Color lastColor;
        public PointerPlayerSystemObjectCaster playerCaster => playerCaster_;
        [DetailedInfoBox("", "此处需要引用一个PointerPlayerCaster,把自己的值投射给他它", Message = @"@ ""【绑定投射目标备注："" + (playerCaster != null ? playerCaster.des : ""！未绑定"") ", VisibleIf = "@usePlayerCaster")]
        [LabelText("发起投射?", SdfIconType.At), GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_04")] public bool usePlayerCaster;
        [LabelText("发送到Caster", SdfIconType.At), ShowIf("usePlayerCaster"), GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_04")] public PointerPlayerSystemObjectCaster playerCaster_;

        public Color Cast()
        {
            return lastColor;
        }

        
        public Color Pick(object by = null, object yarn = null, object on = null)
        {
            lastColor= Color.Lerp(color, color2, lerp?.Pick()??0);
            return lastColor;
        }
    }

    [Serializable, TypeRegistryItem("渲染器_直接引用", "渲染颜色和材质")]
    public class PointerForRenderer_Direc : IPointerForRenderer_Only
    {
        [LabelText("引用渲染器")] public Renderer renderer;

        public Renderer Pick(object by = null, object yarn = null, object on = null)
        {
            return renderer;
        }
    }
    [Serializable, TypeRegistryItem("渲染器设置_设置材质", "渲染颜色和材质")]
    public class PointerSetRenderer_SetMaterial : PointerOnlyAction
    {
        [LabelText("渲染器来源"), SerializeReference]
        public IPointerForRenderer_Only renderer_Only = new PointerForRenderer_Direc();
        [LabelText("材质")] public Material material;
        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            Renderer rr = renderer_Only?.Pick();
            if (rr != null)
            {
                rr.material = material;
            }
            return base.Pick(by, yarn, on);
        }
    }
    [Serializable, TypeRegistryItem("渲染器设置材质块_设置颜色->_Color", "渲染颜色和材质")]
    public class PointerSetRenderer_SetColor_Color : PointerOnlyAction
    {
        [LabelText("渲染器来源"), SerializeReference]
        public IPointerForRenderer_Only renderer_Only = new PointerForRenderer_Direc();
        [LabelText("颜色"), ColorPalette] public Color color;
        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            Renderer rr = renderer_Only?.Pick();
            if (rr != null)
            {
                MaterialPropertyBlock block = new MaterialPropertyBlock();
                block.SetColor("_Color", color);
                rr.SetPropertyBlock(block);
            }
            return base.Pick(by, yarn, on);
        }
    }
    [Serializable, TypeRegistryItem("渲染器设置材质块_设置颜色->_MainColor", "渲染颜色和材质")]
    public class PointerSetRenderer_SetColor_MainColor : PointerOnlyAction
    {
        [LabelText("渲染器来源"), SerializeReference]
        public IPointerForRenderer_Only renderer_Only = new PointerForRenderer_Direc();
        [LabelText("颜色"), ColorPalette] public Color color;
        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            Renderer rr = renderer_Only?.Pick();
            if (rr != null)
            {
                MaterialPropertyBlock block = new MaterialPropertyBlock();
                block.SetColor("_MainColor", color);
                rr.SetPropertyBlock(block);
            }
            return base.Pick(by, yarn, on);
        }
    }
    [Serializable, TypeRegistryItem("渲染器设置材质块_设置浮点数", "渲染颜色和材质")]
    public class PointerSetRenderer_SetFloat : PointerOnlyAction
    {
        [LabelText("渲染器来源"), SerializeReference]
        public IPointerForRenderer_Only renderer_Only = new PointerForRenderer_Direc();
        [LabelText("参数名称")] public string paraName = "AFloat";
        [LabelText("浮点数")] public IPointerForFloat_Only float_ = new PointerForFloat_Direct();
        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            Renderer rr = renderer_Only?.Pick();
            if (rr != null)
            {
                MaterialPropertyBlock block = new MaterialPropertyBlock();
                block.SetFloat(paraName, float_?.Pick() ?? 0);
                rr.SetPropertyBlock(block);
            }
            return base.Pick(by, yarn, on);
        }
    }
    [Serializable, TypeRegistryItem("渲染器设置材质块_设置整数", "渲染颜色和材质")]
    public class PointerSetRenderer_SetInt : PointerOnlyAction
    {
        [LabelText("渲染器来源"), SerializeReference]
        public IPointerForRenderer_Only renderer_Only = new PointerForRenderer_Direc();
        [LabelText("参数名称")] public string paraName = "AInt";
        [LabelText("整数"), SerializeReference] public IPointerForInt_Only int_ = new PointerForInt_Direct();
        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            Renderer rr = renderer_Only?.Pick();
            if (rr != null)
            {
                MaterialPropertyBlock block = new MaterialPropertyBlock();
                block.SetInt(paraName, int_?.Pick() ?? 0);
                rr.SetPropertyBlock(block);
            }
            return base.Pick(by, yarn, on);
        }
    }
    [Serializable, TypeRegistryItem("渲染器设置材质块_设置颜色", "渲染颜色和材质")]
    public class PointerSetRenderer_SetColor : PointerOnlyAction
    {
        [LabelText("渲染器来源"), SerializeReference]
        public IPointerForRenderer_Only renderer_Only = new PointerForRenderer_Direc();
        [LabelText("参数名称")] public string paraName = "_Color";
        [LabelText("颜色值"), SerializeReference] public IPointerForColor_Only _color = new PointerForColor_Direc();
        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            Renderer rr = renderer_Only?.Pick();
            if (rr != null)
            {
                MaterialPropertyBlock block = new MaterialPropertyBlock();
                block.SetColor(paraName, _color?.Pick() ?? Color.white);
                rr.SetPropertyBlock(block);
            }
            return base.Pick(by, yarn, on);
        }
    }
    [Serializable, TypeRegistryItem("渲染器Mesh设置_设置Mesh", "渲染颜色和材质")]
    public class PointerSetFilter_SetMesh : PointerOnlyAction
    {
        [LabelText("模型来源")]
        public MeshFilter filter_Only;

        [LabelText("颜色值")] public Mesh mesh;
        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            MeshFilter rr = filter_Only;
            if (rr != null)
            {
                rr.mesh = mesh;
            }
            return base.Pick(by, yarn, on);
        }
    }
    [Serializable, TypeRegistryItem("精灵渲染器设置_设置Sprite", "渲染颜色和材质")]
    public class PointerSetSpriteRenderer_SetSprite : PointerOnlyAction
    {
        [LabelText("精灵渲染器来源")]
        public SpriteRenderer render_Only;

        [LabelText("颜色值")] public Sprite sprite;
        public override object Pick(object by = null, object yarn = null, object on = null)
        {
            SpriteRenderer rr = render_Only;
            if (rr != null)
            {
                if (rr.sprite == null || sprite != null)
                    rr.sprite = sprite;
            }
            return base.Pick(by, yarn, on);
        }
    }
    #endregion
    #endregion
}
