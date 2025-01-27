using System;
using System.Collections.Generic;
using CommonDLL.Static;


namespace BlackTemple.EpicMine.Core
{
    public class Buff
    {
        public CommonDLL.Static.Buff StaticBuff { get; }

        public string Id { get; }

        public long Time { get ; private set; }

        public long NextCheck { get; private set; }

        public BuffType Type { get; }

        public Dictionary<BuffValueType, float> Value { get; }

        public long TimeLeft => Time - TimeManager.Instance.NowUnixSeconds;

        public long TimeLeftToCheck => NextCheck - TimeManager.Instance.NowUnixSeconds;

        public bool IsCheckTime
        {
            get
            {
                if (Type != BuffType.Currency)
                    return false;

                return TimeManager.Instance.NowUnixSeconds - NextCheck > 0;
            }
        }

        public bool IsActive => Time - TimeManager.Instance.NowUnixSeconds > 0;

        public Buff(CommonDLL.Static.Buff staticBuff)
        {
            StaticBuff = staticBuff;
            Id = staticBuff.Id;
            Time = TimeManager.Instance.NowUnixSeconds + staticBuff.Time * 60 * 60 ;
            NextCheck = TimeManager.Instance.NowUnixSeconds + 24 * 60 * 60;
            Type = staticBuff.Type;
            Value = staticBuff.Value;
        }

        public Buff(CommonDLL.Static.Buff staticBuff, long timeEnd)
        {
            StaticBuff = staticBuff;
            Id = staticBuff.Id;
            Time = timeEnd;
            NextCheck = TimeManager.Instance.NowUnixSeconds + 24 * 60 * 60;
            Type = staticBuff.Type;
            Value = staticBuff.Value;
        }

        public void SetNextCheck(long d)
        {
            NextCheck = d;
        }
        public void Reset()
        {
            Time = TimeManager.Instance.NowUnixSeconds + StaticBuff.Time * 60 * 60;
        }

        public void SetNewTime(long time)
        {
            Time = time;
        }

        public Buff(CommonDLL.Static.Buff staticBuff, Dto.Buff buff)
        {
            StaticBuff = staticBuff;
            Id = buff.Id;
            Time = buff.Time;
            Type = buff.Type;
            Value = staticBuff.Value;
            NextCheck = buff.NextCheck;
        }

        public Buff(CommonDLL.Static.Buff staticBuff, CommonDLL.Dto.Buff buff)
        {
            StaticBuff = staticBuff;
            Id = buff.Id;
            Time = buff.Time;
            Type = (BuffType)buff.Type;
            Value = staticBuff.Value;
            NextCheck = buff.NextCheck;
        }
    }

}