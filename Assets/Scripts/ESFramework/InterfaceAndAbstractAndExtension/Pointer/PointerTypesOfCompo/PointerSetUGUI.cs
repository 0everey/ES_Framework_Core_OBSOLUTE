using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
/*
扩展时 建议创建新的脚本 
修改文件时 使用 #region + 自己的名字
格式尽量统一 
多交流 --- Everey
*/
namespace ES.EvPointer
{
    //核心 Ev针支持 关于 对UGUI的支持 部分
    //基本实现IPointerNone或者IPointerOnlyAction
    #region 接口
    [TypeRegistryItem("设置XXX的XXX","UGUI")]
    public abstract class PointerSetGraphicXXX<T>: IPointerNone where T : Graphic
    {
        [LabelText("直接引用UI")]
        public T ui;

        public object Pick(object by = null, object yarn = null, object on = null)
        {
            if (ui != null)
            {
                PickTruely(ui);
            }
            return null;
        }
        public abstract void PickTruely(T ui);
    }
    [TypeRegistryItem("设置UI图片XXX","UGUI")]
    public abstract class PointerSetImageXXX : PointerSetGraphicXXX<Image>
    {
    }
    [TypeRegistryItem("设置UI原始图片XXX", "UGUI")]
    public abstract class PointerSetRawImageXXX : PointerSetGraphicXXX<RawImage>
    {
    }
    [TypeRegistryItem("设置UI文本XXX", "UGUI")]
    public abstract class PointerSetTextXXX : PointerSetGraphicXXX<Text>
    {
    }
    [TypeRegistryItem("设置UI文本MeshProXXX", "UGUI")]
    public abstract class PointerSetTMP_TextXXX : PointerSetGraphicXXX<TMP_Text>
    {

    }
    #endregion
    #region 常用内容
    [Serializable,TypeRegistryItem("设置UI图片颜色","UGUI")]
    public class PointerSetImageColor: PointerSetImageXXX
    {
        [LabelText("设置颜色"),SerializeReference] public IPointerForColor_Only pointerForColor_ = new PointerForColor_Direct_old();
        public override void PickTruely(Image image)
        {
            image.color = pointerForColor_?.Pick() ?? Color.white;
        }
    }
    [Serializable, TypeRegistryItem("设置UI图片透明度", "UGUI")]
    public class PointerSetImageAlpha : PointerSetImageXXX
    {
        [LabelText("设置透明度"), SerializeReference] public IPointerForFloat_Only forFloat_Only = new PointerForFloat_DirectClamp01() { @float = 1 };
        public override void PickTruely(Image image)
        {
            Color c = image.color;
            c.a = forFloat_Only?.Pick() ?? 1;
            image.color = c;
        }
    }
    [Serializable, TypeRegistryItem("设置UI图片精灵", "UGUI")]
    public class PointerSetImageSptite : PointerSetImageXXX
    {
        [LabelText("设置精灵")] public Sprite sprite;
        public override void PickTruely(Image image)
        {
            image.sprite = sprite;
        }
    }
    [Serializable, TypeRegistryItem("设置UI原始图片颜色", "UGUI")]
    public class PointerSetRawImageColor : PointerSetRawImageXXX
    {
        [LabelText("设置颜色"), SerializeReference] public IPointerForColor_Only pointerForColor_ = new PointerForColor_Direct_old();
        public override void PickTruely(RawImage image)
        {
            image.color = pointerForColor_?.Pick() ?? Color.white;
        }
    }
    [Serializable, TypeRegistryItem("设置UI原始图片透明度", "UGUI")]
    public class PointerSetRawImageAlpha : PointerSetRawImageXXX
    {
        [LabelText("设置透明度"), SerializeReference] public IPointerForFloat_Only forFloat_Only = new PointerForFloat_DirectClamp01() { @float = 1 };
        public override void PickTruely(RawImage image)
        {
            Color c = image.color;
            c.a = forFloat_Only?.Pick() ?? 1;
            image.color = c;
        }
    }
    [Serializable, TypeRegistryItem("设置UI原始图片纹理", "UGUI")]
    public class PointerSetRawImageTexture : PointerSetRawImageXXX
    {
        [LabelText("设置纹理")] public Texture texture;
        public override void PickTruely(RawImage image)
        {
            image.texture = texture;
        }
    }
    [Serializable, TypeRegistryItem("设置UI文本颜色", "UGUI")]
    public class PointerSetTextColor : PointerSetTextXXX
    {
        [LabelText("设置颜色"), SerializeReference] public IPointerForColor_Only pointerForColor_ = new PointerForColor_Direct_old();
        public override void PickTruely(Text text)
        {
            text.color = pointerForColor_?.Pick() ?? Color.white;
        }
    }
    [Serializable, TypeRegistryItem("设置UI文本内容", "UGUI")]
    public class PointerSetTextContent : PointerSetTextXXX
    {
        [LabelText("设置文本内容")] public IPointerForString_Only textContent=new PointerForString_Direc() { string_direc="文本内容" };
        public override void PickTruely(Text text)
        {
            text.text = textContent?.Pick();
        }
    }
    [Serializable, TypeRegistryItem("设置UI文本MeshPro颜色", "UGUI")]
    public class PointerSetTMP_TextColor : PointerSetTMP_TextXXX
    {
        [LabelText("设置颜色"), SerializeReference] public IPointerForColor_Only pointerForColor_ = new PointerForColor_Direct_old();
        public override void PickTruely(TMP_Text text)
        {
            text.color = pointerForColor_?.Pick() ?? Color.white;
        }
    }
    [Serializable, TypeRegistryItem("设置UI文本MeshPro文本内容", "UGUI")]
    public class PointerSetTMP_TextContent : PointerSetTMP_TextXXX
    {
        [LabelText("设置文本内容")] public IPointerForString_Only textContent = new PointerForString_Direc() { string_direc = "文本内容" };
        public override void PickTruely(TMP_Text text)
        {
            text.text = textContent?.Pick();
        }
    }
    #endregion

    #region 封装功能


    #endregion

    #region 特殊功能
    [Serializable, TypeRegistryItem("设置UI图片填充值", "UGUI")]
    public class PointerSetImageFillAmount : PointerSetImageXXX
    {
        [LabelText("设置填充值"), SerializeReference] public IPointerForFloat_Only forFloat_Only = new PointerForFloat_DirectClamp01() { @float = 1 };
        public override void PickTruely(Image image)
        {
            image.fillAmount = forFloat_Only?.Pick() ?? 1;
        }
    }
    #endregion

}
