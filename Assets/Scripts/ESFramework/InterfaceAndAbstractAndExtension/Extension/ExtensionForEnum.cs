using ES;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

namespace ES
{

    public static class ExtensionForEnum 
    {

        // 81. 检查枚举是否包含特定标志
        public static bool HasFlag<T>(this T enumValue, T flag) where T : Enum
        {
            return enumValue.HasFlag(flag);
        }

        // 82. 添加枚举标志
        public static T EX_AddFlag<T>(this T enumValue, T flag) where T : Enum
        {
            return (T)(object)((int)(object)enumValue | (int)(object)flag);
        }

        // 83. 移除枚举标志
        public static T EX_RemoveFlag<T>(this T enumValue, T flag) where T : Enum
        {
            return (T)(object)((int)(object)enumValue & ~(int)(object)flag);
        }

        // 84. 切换枚举标志
        public static T EX_ToggleFlag<T>(this T enumValue, T flag) where T : Enum
        {
            return enumValue.HasFlag(flag) ? enumValue.EX_RemoveFlag(flag) : enumValue.EX_AddFlag(flag);
        }

        // 85. 获取枚举的所有值
        public static IEnumerable<T> EX_GetValues<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        // 86. 获取枚举的描述(如果有Description特性)
        public static string EX_GetDescription(this Enum enumValue)
        {
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
            var attributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
            return attributes.Length > 0 ? attributes[0].Description : enumValue.ToString();
        }

        // 87. 检查枚举值是否有效
        public static bool EX_IsDefined<T>(this T enumValue) where T : Enum
        {
            return Enum.IsDefined(typeof(T), enumValue);
        }

        // 88. 获取枚举的下一个值(循环)
        public static T EX_Next<T>(this T enumValue) where T : Enum
        {
            T[] values = (T[])Enum.GetValues(typeof(T));
            int index = Array.IndexOf(values, enumValue) + 1;
            return index >= values.Length ? values[0] : values[index];
        }

        // 89. 获取枚举的上一个值(循环)
        public static T EX_Previous<T>(this T enumValue) where T : Enum
        {
            T[] values = (T[])Enum.GetValues(typeof(T));
            int index = Array.IndexOf(values, enumValue) - 1;
            return index < 0 ? values[values.Length - 1] : values[index];
        }

        // 90. 随机获取枚举值
        public static T EX_Random<T>() where T : Enum
        {
            T[] values = (T[])Enum.GetValues(typeof(T));
            return values[UnityEngine.Random.Range(0, values.Length)];
        }
    }
}

