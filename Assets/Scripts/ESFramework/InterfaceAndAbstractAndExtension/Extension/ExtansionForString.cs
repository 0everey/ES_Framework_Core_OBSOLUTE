using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ES {
    public static class ExtensionForString
    {

        public static StringBuilder EX_GetBuilder(this string selfStr)
        {
            return new StringBuilder(selfStr);
        }


        public static StringBuilder EX_AddPre(this StringBuilder self, string prefixString)
        {
            self.Insert(0, prefixString);
            return self;
        }
        public static int EX_AsInt(this string selfStr, int defaulValue = 0)
        {
            var retValue = defaulValue;
            return int.TryParse(selfStr, out retValue) ? retValue : defaulValue;
        }

        public static long EX_AsLong(this string self, long defaultValue = 0)
        {
            var retValue = defaultValue;
            return long.TryParse(self, out retValue) ? retValue : defaultValue;
        }


        public static DateTime EX_AsDateTime(this string selfStr, DateTime defaultValue = default(DateTime))
        {
            return DateTime.TryParse(selfStr, out var retValue) ? retValue : defaultValue;
        }



        public static float EX_AsFloat(this string selfStr, float defaultValue = 0)
        {
            return float.TryParse(selfStr, out var retValue) ? retValue : defaultValue;
        }


        public static bool EX_HasChinese(this string input)
        {
            return Regex.IsMatch(input, @"[\u4e00-\u9fa5]");
        }


        public static bool EX_AsHasSpace(this string input)
        {
            return input.Contains(" ");
        }


        public static string EX_AsRemoveString(this string str, params string[] targets)
        {
            return targets.Aggregate(str, (current, t) => current.Replace(t, string.Empty));
        }


        public static string EX_StringJoin(this IEnumerable<string> self, string separator)
        {
            return string.Join(separator, self);
        }
    }
}