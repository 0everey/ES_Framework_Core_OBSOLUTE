using ES.EvPointer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{
    [RequireComponent(typeof(Collider2D))]
    public class PointerPlayerByCollider2D : PointerPlayerByColliderOrTrigger<PointerForCollider2DCaster>
    {
        public override string title => "针播放器： 2D 碰撞器";
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (TagMatch(collision.gameObject))
            {
                collider_ ??= new PointerForCollider2DCaster();
                collider_.cast = collision.collider;
                v3_ ??= new PointerForVector3Caster();
                v3_.cast = collision.GetContact(0).point;

            }

            if ((!boolForColliderEnter?.Pick()) ?? true) return;

            if (TagMatch(collision.gameObject))
            {
                collider_Match ??= new PointerForCollider2DCaster();
                collider_Match.cast = collision.collider;
                v3_Match ??= new PointerForVector3Caster();
                v3_Match.cast = collision.GetContact(0).point;
            }
            pointerForEnter?.Pick();
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (TagMatch(collision.gameObject))
            {
                collider_ ??= new PointerForCollider2DCaster();
                collider_.cast = collision.collider;
                v3_ ??= new PointerForVector3Caster();
                v3_.cast = collision.GetContact(0).point;
            }

            if ((!boolForColliderStay?.Pick()) ?? true) return;

            if (TagMatch(collision.gameObject))
            {
                collider_Match ??= new PointerForCollider2DCaster();
                collider_Match.cast = collision.collider;
                v3_Match ??= new PointerForVector3Caster();
                v3_Match.cast = collision.GetContact(0).point;
            }
            pointerForStay?.Pick();
        }
        private void OnCollisionExit2D(Collision2D collision)
        {
            if (TagMatch(collision.gameObject))
            {
                collider_ ??= new PointerForCollider2DCaster();
                collider_.cast = collision.collider;
                v3_ ??= new PointerForVector3Caster();
                v3_.cast = collision.GetContact(0).point;
            }

            if ((!boolForColliderStay?.Pick()) ?? true) return;

            if (TagMatch(collision.gameObject))
            {
                collider_Match ??= new PointerForCollider2DCaster();
                collider_Match.cast = collision.collider;
                v3_Match ??= new PointerForVector3Caster();
                v3_Match.cast = collision.GetContact(0).point;
            }
            pointerForExit?.Pick();
        }
    }
}
