using ES.EvPointer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{
    [RequireComponent(typeof(Collider))]
    public class PointerPlayerByTrigger3D : PointerPlayerByColliderOrTrigger<PointerForCollider3DCaster>
    {
        public override string title => "针播放器： 3D 触发器";
        private void OnTriggerEnter(Collider other)
        {
            Vector3 vv = (other.transform.position + transform.position) / 2;
            if (TagMatch(other.gameObject))
            {
                collider_ ??= new PointerForCollider3DCaster();
                collider_.cast = other;
                v3_ ??= new PointerForVector3Caster();
                v3_.cast = vv;
            }

            if ((!boolForColliderEnter?.Pick()) ?? true) return;

            if (TagMatch(other.gameObject))
            {
                collider_Match ??= new PointerForCollider3DCaster();
                collider_Match.cast = other;
                v3_Match ??= new PointerForVector3Caster();
                v3_Match.cast = vv;
            }
            pointerForEnter?.Pick();
        }
        

        private void OnTriggerStay(Collider other)
        {
            Vector3 vv = (other.transform.position + transform.position) / 2;
            if (TagMatch(other.gameObject))
            {
                collider_ ??= new PointerForCollider3DCaster();
                collider_.cast = other;
                v3_ ??= new PointerForVector3Caster();
                v3_.cast = vv;
            }

            if ((!boolForColliderStay?.Pick()) ?? true) return;

            if (TagMatch(other.gameObject))
            {
                collider_Match ??= new PointerForCollider3DCaster();
                collider_Match.cast = other;
                v3_Match ??= new PointerForVector3Caster();
                v3_Match.cast = vv;
            }
            pointerForStay?.Pick();
        }
        private void OnTriggerExit(Collider other)
        {
            Vector3 vv = (other.transform.position + transform.position) / 2;
            if (TagMatch(other.gameObject))
            {
                collider_ ??= new PointerForCollider3DCaster();
                collider_.cast = other;
                v3_ ??= new PointerForVector3Caster();
                v3_.cast = vv;
            }

            if ((!boolForColliderStay?.Pick()) ?? true) return;

            if (TagMatch(other.gameObject))
            {
                collider_Match ??= new PointerForCollider3DCaster();
                collider_Match.cast = other;
                v3_Match ??= new PointerForVector3Caster();
                v3_Match.cast = vv;
            }
            pointerForExit?.Pick();
        }
    }
}
