using ES;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ES
{

    public static class ExtensionForFloatAndInt
    {
        public static float EX_SafeDivide(this float f, float b)
        {
            if (b == 0) b = 1;
            return f / b;
        }
        // 1. 检查float是否在范围内
        public static bool EX_InRange(this float f, Vector2 range)
        {
            return f >= range.x && f <= range.y;
        }

        // 2. 检查int是否在范围内
        public static bool EX_InRange(this int i, Vector2Int range)
        {
            return i >= range.x && i <= range.y;
        }

        // 3. 限制数值在最小最大值之间
        public static float EX_Clamp(this float value, float min, float max)
        {
            return Mathf.Clamp(value, min, max);
        }

        // 4. 检查float是否接近0
        public static bool EX_IsApproximatelyZero(this float f, float threshold = 0.001f)
        {
            return Mathf.Abs(f) < threshold;
        }

        // 5. 将角度限制在0-360度
        public static float EX_AsNormalizeAngle(this float angle)
        {
            angle %= 360f;
            if (angle < 0) angle += 360f;
            return angle;
        }

        // 6. 将角度限制在-180到180度
        public static float EX_AsNormalizeAngle180(this float angle)
        {
            angle %= 360f;
            if (angle > 180f) angle -= 360f;
            if (angle < -180f) angle += 360f;
            return angle;
        }

        // 7. 检查两个float是否近似相等
        public static bool EX_IsApproximately(this float a, float b, float threshold = 0.001f)
        {
            return Mathf.Abs(a - b) < threshold;
        }

        // 8. 将值映射到新范围
        public static float EX_Remap(this float value, float fromMin, float fromMax, float toMin=0, float toMax=1)
        {
            return toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin);
        }

        // 9. 检查int是否为偶数
        public static bool EX_IsEven(this int i)
        {
            return i % 2 == 0;
        }

        // 10. 检查int是否为奇数
        public static bool EX_IsOdd(this int i)
        {
            return i % 2 != 0;
        }
    }
}

