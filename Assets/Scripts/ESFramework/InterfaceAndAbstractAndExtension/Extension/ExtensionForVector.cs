using ES;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{

    public static class ExtensionForVector
    {
        public static Vector3 EX_NoY(this Vector3 v)
        {
            v.y = 0;
            return v;
        }
        public static Vector3 EX_WithYFit(this Vector3 v, float Fit)
        {
            v += v.normalized.EX_NoY() * Fit;
            return v;
        }
        public static Vector3 EX_WithYCut(this Vector3 v, float Fit_)
        {
            v = Vector3.Lerp(v, v.EX_NoY().normalized * v.magnitude, Fit_);
            return v;
        }
        public static Vector3 EX_MutiVector3(this Vector3 v, Vector3 v2)
        {
            return new Vector3(v.x * v2.x, v.y * v2.y, v.z * v2.z);
        }
        // 11. 设置Vector3的Y值
        public static Vector3 EX_WithY(this Vector3 v, float y)
        {
            v.y = y;
            return v;
        }

        // 12. 设置Vector3的X值
        public static Vector3 EX_WithX(this Vector3 v, float x)
        {
            v.x = x;
            return v;
        }

        // 13. 设置Vector3的Z值
        public static Vector3 EX_WithZ(this Vector3 v, float z)
        {
            v.z = z;
            return v;
        }

        // 14. 乘以Vector3的Y值
        public static Vector3 EX_WithYMuti(this Vector3 v, float yMulti)
        {
            v.y *= yMulti;
            return v;
        }

        // 15. 乘以Vector3的X值
        public static Vector3 EX_WithXMuti(this Vector3 v, float xMulti)
        {
            v.x *= xMulti;
            return v;
        }

        // 16. 乘以Vector3的Z值
        public static Vector3 EX_WithZMuti(this Vector3 v, float zMulti)
        {
            v.z *= zMulti;
            return v;
        }

        // 17. 忽略Y轴的Vector3
        public static Vector3 EX_IgnoreY(this Vector3 v)
        {
            return new Vector3(v.x, 0, v.z);
        }

        // 18. 计算方向向量
        public static Vector3 EX_DirectionTo(this Vector3 from, Vector3 to)
        {
            return (to - from).normalized;
        }

        // 19. 计算水平距离
        public static float EX_HorizontalDistanceTo(this Vector3 from, Vector3 to)
        {
            return Vector2.Distance(new Vector2(from.x, from.z), new Vector2(to.x, to.z));
        }

        // 20. 检查Vector3是否近似为零
        public static bool EX_IsApproximatelyZero(this Vector3 v, float threshold = 0.001f)
        {
            return v.sqrMagnitude < threshold * threshold;
        }
    }
}

