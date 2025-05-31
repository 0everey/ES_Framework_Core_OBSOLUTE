using ES.EvPointer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ES
{
    public static partial  class Extension
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
        public static bool InRange(this float f, Vector2 range)
        {
            if (f <= range.y && f >= range.x) return true;
            return false;
        }
        public static bool InRange(this int f, Vector2Int range)
        {
            if (f <= range.y && f >= range.x) return true;
            return false;
        }
        public static Vector3 WithY(this Vector3 v, float Y)
        {
            v.y = Y;
            return v;
        }
        public static Vector3 WithYMuti(this Vector3 v, float YMuti)
        {
            v.y = v.y * YMuti;
            return v;
        }
        public static Vector3 WithYFit(this Vector3 v, float Fit)
        {
            v += v.normalized.NoY() * Fit;
            return v;
        }
        public static Vector3 WithYCut(this Vector3 v, float Fit_)
        {
            v = Vector3.Lerp(v, v.NoY().normalized * v.magnitude, Fit_);
            return v;
        }
        public static Vector3 MutiVector3(this Vector3 v, Vector3 v2)
        {
            return new Vector3(v.x * v2.x, v.y * v2.y, v.z * v2.z);
        }
        public static float SafeDivide(this float f, float b)
        {
            if (b == 0) b = 1;
            return f / b;
        }
    }
}
