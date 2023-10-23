using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Basis
{
    [HideDuplicateReferenceBox]
    [HideReferenceObjectPicker]
    [Serializable]
    public class GameTime
    {
        [LabelText("年"), MinValue(0)]
        [OnValueChanged(nameof(OnYearChanged))]
        public int year;
        [LabelText("月"), MaxValue(12), MinValue(0)]
        [OnValueChanged(nameof(OnMonthChanged))]
        public int month;
        [LabelText("日"), MaxValue(30), MinValue(0)]
        [OnValueChanged(nameof(OnDayChanged))]
        public int day;
        [LabelText("时"), MaxValue(24), MinValue(0)]
        [OnValueChanged(nameof(OnHourChanged))]
        public int hour;
        [LabelText("分"), MaxValue(60), MinValue(0)]
        [OnValueChanged(nameof(OnMinuteChanged))]
        public int minute;

        [LabelText("总月数")]
        [FoldoutGroup("信息"), ShowInInspector]
        public int totalMonth => month + year * 12;
        [LabelText("总天数")]
        [FoldoutGroup("信息"), ShowInInspector]
        public int totalDay => day + totalMonth * 30;
        [LabelText("总小时")]
        [FoldoutGroup("信息"), ShowInInspector]
        public int totalHour => hour + totalDay * 24;
        [LabelText("总分钟")]
        [FoldoutGroup("信息"), ShowInInspector]
        public int totalMinute => minute + totalHour * 60;

        #region GUI

        private void OnYearChanged()
        {
            if (year < 0)
            {
                year = 0;
            }
        }

        private void OnMonthChanged()
        {
            if (month >= 12)
            {
                month = 0;
                year++;
            }

            if (month < 0)
            {
                month = 11;
                year--;

                if (year < 0)
                {
                    year = 0;
                }
            }
        }

        private void OnDayChanged()
        {
            if (day >= 30)
            {
                day = 0;
                month++;
            }

            if (day < 0)
            {
                day = 29;
                month--;
            }
        }

        private void OnHourChanged()
        {
            if (hour >= 24)
            {
                hour = 0;
                day++;
            }

            if (hour < 0)
            {
                hour = 23;
                day--;
            }
        }

        private void OnMinuteChanged()
        {
            if (minute >= 60)
            {
                minute = 0;
                hour++;
            }

            if (minute < 0)
            {
                minute = 59;
                hour--;
            }
        }

        //private void OnSecondChanged()
        //{
        //    if (second >= 60)
        //    {
        //        second = 0;
        //        minute++;
        //    }

        //    if (second < 0)
        //    {
        //        second = 59;
        //        minute--;
        //    }
        //}

        #endregion

        public int Get(TimeType timeType) =>
            timeType switch
            {
                TimeType.Year => year,
                TimeType.Mouth => month,
                TimeType.Day => day,
                TimeType.Hour => hour,
                TimeType.Minute => minute,
                _ => throw new ArgumentOutOfRangeException(nameof(timeType), timeType, null)
            };

        public GameTime Add(GameTime other)
        {
            minute += other.minute;

            if (minute >= 60)
            {
                minute -= 60;
                hour++;
            }

            hour += other.hour;

            if (hour >= 24)
            {
                hour -= 24;
                day++;
            }

            day += other.day;

            if (day >= 30)
            {
                day -= 30;
                month++;
            }

            month += other.month;

            if (month >= 12)
            {
                month -= 12;
                year++;
            }

            year += other.year;

            return this;
        }
    }
}

