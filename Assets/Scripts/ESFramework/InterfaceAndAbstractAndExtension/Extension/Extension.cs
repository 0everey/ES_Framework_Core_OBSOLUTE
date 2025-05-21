using ES.EvPointer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ES
{
    public static class Extension
    {
        public static B BePicked<T, P, B>(this T t, P p, object yarn, object on) where P : IPointer<B, T, object, object>
        {
            return p.Pick(t, yarn, on);
        }
        public static B BePicked<T, P, B, Yarn>(this T t, P p, Yarn yarn, object on) where P : IPointer<B, T, Yarn, object>
        {
            return p.Pick(t, yarn, on);
        }
        public static B BePicked<T, P, B, Yarn, On>(this T t, P p, Yarn yarn, On on) where P : IPointer<B, T, Yarn, On>
        {
            return p.Pick(t, yarn, on);
        }

        public static Vector3 NoY(this Vector3 v)
        {
            v.y = 0;
            return v;
        }
        public static Vector3 WithY(this Vector3 v, float Y)
        {
            v.y = Y;
            return v;
        }
    }
}
