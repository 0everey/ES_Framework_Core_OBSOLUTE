using ES;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ES {
    public static class ExtensionForDateTime
    {
        // 71. 将秒数转换为"mm:ss"格式
        public static string EX_AsMinutesSeconds(this float seconds)
        {
            return TimeSpan.FromSeconds(seconds).ToString(@"mm\:ss");
        }

        // 72. 将秒数转换为"hh:mm:ss"格式
        public static string EX_AsHoursMinutesSeconds(this float seconds)
        {
            return TimeSpan.FromSeconds(seconds).ToString(@"hh\:mm\:ss");
        }

        // 73. 将时间戳转换为DateTime
        public static DateTime EX_AsDateTime(this long timestamp)
        {
            return DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime;
        }

        // 74. 将DateTime转换为时间戳
        public static long EX_AsTimestamp(this DateTime dateTime)
        {
            return new DateTimeOffset(dateTime).ToUnixTimeSeconds();
        }

        // 75. 检查DateTime是否在今天
        public static bool EX_IsToday(this DateTime dateTime)
        {
            return dateTime.Date == DateTime.Today;
        }

        // 76. 检查DateTime是否在昨天
        public static bool EX_IsYesterday(this DateTime dateTime)
        {
            return dateTime.Date == DateTime.Today.AddDays(-1);
        }

        // 77. 检查DateTime是否在明天
        public static bool EX_IsTomorrow(this DateTime dateTime)
        {
            return dateTime.Date == DateTime.Today.AddDays(1);
        }

        // 78. 获取DateTime的开始时间(00:00:00)
        public static DateTime EX_StartOfDay(this DateTime dateTime)
        {
            return dateTime.Date;
        }

        // 79. 获取DateTime的结束时间(23:59:59)
        public static DateTime EX_EndOfDay(this DateTime dateTime)
        {
            return dateTime.Date.AddDays(1).AddTicks(-1);
        }

        // 80. 计算两个DateTime之间的天数差
        public static int EX_DaysBetween(this DateTime from, DateTime to)
        {
            return (int)(to.Date - from.Date).TotalDays;
        }
    }
}
