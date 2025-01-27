using System;
using BlackTemple.Common;
using DG.Tweening;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class TimeManager : Singleton<TimeManager>
    {
        public DateTime Now { get; private set; }

        public long NowUnixSeconds { get; private set; }

        private float _startTimeScale;

        private float _secondsTimerValue;

        private float _unscaledSecondsTimerValue;

        public void SetPause(bool pause)
        {

            Time.timeScale = pause ? 0 : _startTimeScale;
        }

        protected override void Awake()
        {
            base.Awake();
            _startTimeScale = Time.timeScale;
        }

        private void OnApplicationPause(bool isPause)
        {
            if (!isPause)
                Now = DateTime.UtcNow;
        }

        public void AddExtra(long t)
        {
            T = t;
        }

        public long T;
        private void Update()
        {
            if (_unscaledSecondsTimerValue <= 0)
            {
                Now = DateTime.UtcNow;
                NowUnixSeconds = ToDateTimeOffset(Now).ToUnixTimeSeconds() + T;
                EventManager.Instance.Publish(new UnscaledSecondsTickEvent());
                _unscaledSecondsTimerValue = 1f;
            }
            else
                _unscaledSecondsTimerValue -= Time.unscaledDeltaTime;

            if (_secondsTimerValue <= 0)
            {
                EventManager.Instance.Publish(new SecondsTickEvent());
                _secondsTimerValue = 1f;
            }
            else
            {  _secondsTimerValue -= Time.deltaTime;  }
        }
        public DateTimeOffset ToDateTimeOffset(DateTime dateTime)
        {
            return dateTime.ToUniversalTime() <= DateTimeOffset.MinValue.UtcDateTime
                ? DateTimeOffset.MinValue
                : new DateTimeOffset(dateTime);
        }

        public DateTime FromUnixToDateTime(long a)
        {
            var dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(a);
            return dateTimeOffset.UtcDateTime;
        }
        public void UnscaledTweenDelay(int time, Action action)
        {
            DOTween.Sequence()
                .SetDelay(2)
                .OnComplete(() => { action(); })
                .SetUpdate(true);
        }
    }
}