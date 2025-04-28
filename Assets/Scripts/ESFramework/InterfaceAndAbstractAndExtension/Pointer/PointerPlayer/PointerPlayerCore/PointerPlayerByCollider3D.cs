using ES.EvPointer;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{
    [RequireComponent(typeof(Collider))]
    public class PointerPlayerByCollider3D : PointerPlayerByColliderOrTrigger<PointerForCollider3DCaster>
    {
        public override string title => "针播放器： 3D 碰撞器";
        private void OnCollisionEnter(Collision collision)
        {
            if (TagMatch(collision.gameObject))
            {
                collider_ ??= new PointerForCollider3DCaster();
                collider_.cast = collision.collider;
                v3_ ??= new PointerForVector3Caster();
                v3_.cast = collision.GetContact(0).point;
            }
            
            if ((!boolForColliderEnter?.Pick()) ?? true) return;

            if (TagMatch(collision.gameObject))
            {
                collider_Match ??= new PointerForCollider3DCaster();
                collider_Match.cast = collision.collider;
                v3_Match ??= new PointerForVector3Caster();
                v3_Match.cast = collision.GetContact(0).point;
            }
            pointerForEnter?.Pick();
        }

        private void OnCollisionStay(Collision collision)
        {
            if (TagMatch(collision.gameObject))
            {
                collider_ ??= new PointerForCollider3DCaster();
                collider_.cast = collision.collider;
                v3_ ??= new PointerForVector3Caster();
                v3_.cast = collision.GetContact(0).point;
            }

            if ((!boolForColliderStay?.Pick()) ?? true) return;

            if (TagMatch(collision.gameObject))
            {
                collider_Match ??= new PointerForCollider3DCaster();
                collider_Match.cast = collision.collider;
                v3_Match ??= new PointerForVector3Caster();
                v3_Match.cast = collision.GetContact(0).point;
            }
            pointerForStay?.Pick();
        }
        private void OnCollisionExit(Collision collision)
        {
            if (TagMatch(collision.gameObject))
            {
                collider_ ??= new PointerForCollider3DCaster();
                collider_.cast = collision.collider;
                v3_ ??= new PointerForVector3Caster();
                v3_.cast = collision.GetContact(0).point;
            }

            if ((!boolForColliderStay?.Pick()) ?? true) return;

            if (TagMatch(collision.gameObject))
            {
                collider_Match ??= new PointerForCollider3DCaster();
                collider_Match.cast = collision.collider;
                v3_Match ??= new PointerForVector3Caster();
                v3_Match.cast = collision.GetContact(0).point;
            }
            pointerForExit?.Pick();
        }
    }
    public abstract class PointerPlayerByColliderOrTrigger<Cast> : PointerPlayer where Cast:IPointer
    {
       
        [DisplayAsString(fontSize:20),ShowInInspector,HideLabel, GUIColor("@KeyValueMatchingUtility.ColorSelector.Color_03"),PropertyOrder(-1)]
        public string ss=> title;
        public virtual string title => "针播放器：";
        [Header("启用开关"), FoldoutGroup("《启用开关》"),Indent(2)]
        [LabelText("是否启用Enter"), SerializeReference] public IPointerForBool_Only boolForColliderEnter = new PointerForBool_Direc();
        [LabelText("是否启用Stay"), FoldoutGroup("《启用开关》"), SerializeReference, Indent(2)] public IPointerForBool_Only boolForColliderStay = new PointerForBool_Direc();
        [LabelText("是否启用Exit"), FoldoutGroup("《启用开关》"), SerializeReference, Indent(2)] public IPointerForBool_Only boolForColliderExit = new PointerForBool_Direc();

        [Header("触发事件"), FoldoutGroup("《触发事件》"), Indent(2)]
        [LabelText("触发Enter时"), SerializeReference] public IPointerNone pointerForEnter = new PointerPickerByBool();
        [LabelText("触发Stay时"), FoldoutGroup("《触发事件》"), SerializeReference, Indent(2)] public IPointerNone pointerForStay = new PointerPickerByBool();
        [LabelText("触发Exit时"), FoldoutGroup("《触发事件》"), SerializeReference, Indent(2)] public IPointerNone pointerForExit = new PointerPickerByBool();

        [Header("筛选"),FoldoutGroup("《筛选》"), Indent(2)]
        [LabelText("启用Tag检查"), FoldoutGroup("《筛选》"), Indent(2)] public bool useTagCheck = false;
        [LabelText("全部可接受Tag"), FoldoutGroup("《筛选》"), Indent(2)] public PointerForStringList_Tag tags = new PointerForStringList_Tag();
        public override IPointer Pointer => collider_Match;
        [Header("数据返回投射"), FoldoutGroup("《数据返回投射》"), Indent(2)]
        [LabelText("碰撞器投射(标签验证)"),SerializeReference] public Cast collider_ ;
        [LabelText("碰撞位置投射(标签验证)"), FoldoutGroup("《数据返回投射》"), SerializeReference, Indent(2)] public PointerForVector3Caster v3_ = new PointerForVector3Caster();


        [LabelText("碰撞器投射(全验证通过)"), FoldoutGroup("《数据返回投射》"), SerializeReference, Indent(2)] public Cast collider_Match;
        [LabelText("碰撞位置投射(全验证通过)"), FoldoutGroup("《数据返回投射》"), SerializeReference, Indent(2)] public PointerForVector3Caster v3_Match = new PointerForVector3Caster();

        public bool TagMatch(GameObject g)
        {
            if (g == null) return false;
            if (useTagCheck)
            {
                var ls = tags?.Pick();
                if (ls == null) return true;
                else
                {
                    foreach (var i in ls)
                    {
                        if (g.CompareTag(i)) return true;
                    }
                    return false;
                }
            }
            return true;

        }
        
    }
}
