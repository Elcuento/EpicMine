using System;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public static class TimeHelper
    {
        public static string Format(TimeSpan time, bool detailed = false)
        {
            var secondsLocale = LocalizationHelper.GetLocale("seconds");
            var minutesLocale = LocalizationHelper.GetLocale("minutes");
            var hoursLocale = LocalizationHelper.GetLocale("hours");
            var daysLocale = LocalizationHelper.GetLocale("days");

            if (time.TotalSeconds < 60)
                return $"{time.TotalSeconds:F0}{secondsLocale}";

            if (time.TotalMinutes < 60)
            {
                return detailed 
                    ? $"{time.Minutes:F0}{minutesLocale}{time.Seconds:F0}{secondsLocale}"
                    : $"{time.TotalMinutes:F0}{minutesLocale}";
            }

            if (time.TotalHours < 24)
            {
                return $"{time.Hours:F0}{hoursLocale}{time.Minutes:F0}{minutesLocale}";
            }
           

            return $"{time.Days:F0}{daysLocale}{time.Hours:F0}{hoursLocale}";
        }

        public static string Format(DateTime time, bool dayAnnotation = false, bool hourAnnotation = false)
        {
           // var secondsLocale = LocalizationHelper.GetLocale("seconds");
           // var minutesLocale = LocalizationHelper.GetLocale("minutes");
        
            var result = "";

            var month = time.Month > 0 ? time.Month - 1 : 0;
            var day = time.Day > 0 ? time.Day - 1 : 0;

            if (month > 0)
            {
                var monthLocale = LocalizationHelper.GetLocale("month");
                result += dayAnnotation ? $"{month}{monthLocale} " : $"{month}:";
            }

            if (day > 0)
            {
                var daysLocale = LocalizationHelper.GetLocale("days");
                result += dayAnnotation ? $"{day:F0}{daysLocale} " : $"{day:F0}:";
            }

            if (time.Hour > 0)
            {
                var hoursLocale = LocalizationHelper.GetLocale("hours");
                result += hourAnnotation ? $"{time.Hour:D2}{hoursLocale} " : $"{time.Hour:D2}:";
            }

             result += $"{time.Minute:D2}:{time.Second:D2}";
            
            return result;
        }

        public static int GetDaysBetweenDates(DateTime date)
        {
            
            var dateNow = DateTime.Today;
            return (date - dateNow).Days;
        }

        public static string FormatSimple(DateTime time)
        {
            return $"{time.Day:F0}{time.Hour:F0}{time.Minute:F0}{time.Second:F0}";
        }

        public static string Format(int totalSeconds, bool detailed = false)
        {
            return Format(TimeSpan.FromSeconds(totalSeconds), detailed);
        }

        public static string Format(float totalSeconds, bool detailed = false)
        {
            return Format(TimeSpan.FromSeconds(totalSeconds), detailed);
        }


        public static string SecondsToDate(long totalSeconds, bool withMonth = false, bool withDays = false, bool withHour = true)
        {

            var time = new DateTime();
            time = time.AddSeconds(totalSeconds);

            var result = "";

            var month = time.Month > 0 ? time.Month - 1 : 0;
            var day = time.Day > 0 ? time.Day - 1 : 0;

            if (month > 0)
            {
                var monthLocale = LocalizationHelper.GetLocale("month");
                result += withMonth ? $"{month}{monthLocale} " : $"{month}:";
            }

            if (day > 0)
            {
                var daysLocale = LocalizationHelper.GetLocale("days");
                result += withDays ? $"{day:F0}{daysLocale} " : $"{day:F0}:";
            }

            if (time.Hour > 0)
            {
                var hoursLocale = LocalizationHelper.GetLocale("hours");
                result += withHour ? $"{time.Hour:D2}{hoursLocale} " : $"{time.Hour:D2}:";
            }

            return result;
        }
    }
}