using System;
using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using BlackTemple.EpicMine.Dto;
using CommonDLL.Static;
namespace BlackTemple.EpicMine.Core
{
    public class Effect
    {
        public List<Buff> BuffList;

 

        public Effect(CommonDLL.Dto.Effect data)
        {
            BuffList = new List<Buff>();

            if (data.BuffList == null || data.BuffList.Count <= 0)
                return;

            foreach (var dataBuff in data.BuffList)
            {
                var staticBuff = App.Instance.StaticData.Buffs.Find(x => x.Id == dataBuff.Id);

                BuffList.Add(new Buff(staticBuff, dataBuff));
            }
        }

        public void BroadCastEffects()
        {
            foreach (var buff in BuffList)
            {
                if (buff.IsActive)
                {
                    EventManager.Instance.Publish(new EffectAddBuffEvent(buff));
                }
            }
        }

        public void CheckBuff(string buffId, Action onCompleted = null, Action onFailed = null)
        {
            var buff = BuffList.Find(x => x.Id == buffId);

            if (buff == null)
                return;

            var buffStatic = buff.StaticBuff;

            var dateNow = TimeManager.Instance.NowUnixSeconds;
            var nextDay = dateNow + 24 * 60 * 60;

            var crystalsToAdd = 0;

            if (buff.Time < dateNow || buff.NextCheck > dateNow)
                return ;

            if (buff.Type == BuffType.Currency)
            {
                foreach (var f in buffStatic.Value)
                {
                    if (f.Key == BuffValueType.CrystalsByDay)
                    {
                        crystalsToAdd += (int)f.Value;
                        buff.SetNextCheck(nextDay);
                    }
                }
            }

            var crystalsGet = new Currency(CurrencyType.Crystals, crystalsToAdd);

            if (crystalsGet.Amount > 0)
            {
                App.Instance.Player.Wallet.Add(crystalsGet, IncomeSourceType.FromBuff, buffId);

                WindowManager.Instance.Show<WindowCustomGift>()
                    .Initialize(crystalsGet, LocalizationHelper.GetLocale("window_shop_crystals"), false);
            }

            onCompleted?.Invoke();
        }



        public bool IsBuffExist(BuffType type, BuffValueType val)
        {
            var buff = BuffList.Find(x => x.Type == type && x.Value.ContainsKey(val));
            return buff !=null;
        }

        public Buff GetBuff(BuffType type)
        {
            return BuffList.Find(x => x.Type == type);
        }

        public Buff GetBuff(string id)
        {
            return BuffList.Find(x => x.Id == id);
        }

        public Buff GetBuff(BuffType type, BuffValueType val)
        {
            var allSuiteBuffs = BuffList.FindAll(x => x.IsActive && x.Type == type && x.Value.ContainsKey(val));
            if (allSuiteBuffs.Count == 0)
                return null;

            var maxPriority = allSuiteBuffs.Max(x => x.StaticBuff.Priority);
            var allSameBuffs = allSuiteBuffs.Where(x => x.StaticBuff.Priority == maxPriority)
                .OrderBy(x => x.StaticBuff.Time);

             return allSameBuffs.FirstOrDefault();
        }

        public Buff AddExchangeBuff(string buffId)
        {
           var staticBuff = App.Instance.StaticData.Buffs.Find(x => x.Id == buffId);
           var currentBuff = BuffList.Find(x => x.Id == staticBuff.Id);

            long sameBuffTime = 0;
            long extraTime = (staticBuff.Time * 60 * 60);

            foreach (var buff in BuffList)
            {
                if (buff.Id != staticBuff.Id)
                {
                    if (buff.StaticBuff.Filter == staticBuff.Filter &&
                        staticBuff.Priority >= buff.StaticBuff.Priority)
                    {
                        var time = buff.Time;
                        time = time + extraTime;
                        buff.SetNewTime(time);
                    }
                }
            }

            if (currentBuff != null)
            {
                var time = currentBuff.Time;
                if (time > TimeManager.Instance.NowUnixSeconds)
                {
                    sameBuffTime = time - TimeManager.Instance.NowUnixSeconds;
                }

                currentBuff.SetNewTime(TimeManager.Instance.NowUnixSeconds + sameBuffTime + extraTime);
            }
            else
            {
                currentBuff = new Buff(staticBuff, TimeManager.Instance.NowUnixSeconds + extraTime);
                BuffList.Add(currentBuff);
            }

            EventManager.Instance.Publish(new EffectAddBuffEvent(currentBuff));

            return currentBuff;
        }

        public Buff GetMaxTimeBuff(Buff buff)
        {
            var allSuiteBuffs = BuffList.FindAll(x => x.IsActive && x.Type == buff.Type && x.StaticBuff.Filter == buff.StaticBuff.Filter);
            if (allSuiteBuffs.Count == 0)
                return buff;
            //
            var maxTime = allSuiteBuffs.Max(x => x.Time);
            var allSameBuffs = allSuiteBuffs.Where(x => x.Time == maxTime)
                .OrderBy(x => x.StaticBuff.Time);

            return allSameBuffs.FirstOrDefault();
        }


        public Buff AddExchangeBuff(string buffId, long timeEnd)
        {
            var currentBuff = BuffList.Find(x => x.Id == buffId);

            if (currentBuff != null)
            {
                currentBuff.Reset();
            }
            else
            {
                var staticBuff = App.Instance.StaticData.Buffs.Find(x => x.Id == buffId);
                currentBuff = new Buff(staticBuff, timeEnd);
                BuffList.Add(currentBuff);
            }

            EventManager.Instance.Publish(new EffectAddBuffEvent(currentBuff));

            return currentBuff;
        }

        public void AddBuffUntilTime(List<Buff> splitBuffs, long expireDate)
        {
            if (splitBuffs.Count == 0)
                return;

            var dateNow = TimeManager.Instance.NowUnixSeconds;

            foreach (var bufStatic in App.Instance.StaticData.Buffs)
            {
                foreach (var buffData in splitBuffs)
                {
                    if (bufStatic.Id == buffData.Id)
                    {
                        var buffSetData = new CommonDLL.Dto.Buff(bufStatic.Value)
                        {
                            Id = bufStatic.Id,
                            Type = bufStatic.Type,
                            Time = expireDate,
                        };

                        var buffOn = App.Instance.Player.Effect.BuffList.Find(x => x.Id == buffSetData.Id);

                        if (buffOn != null)
                            App.Instance.Player.Effect.BuffList.Remove(buffOn);

                        App.Instance.Player.Effect.BuffList.Add(new Buff(bufStatic, buffSetData));

                        if (buffData.Type == BuffType.Currency)
                        {
                            buffSetData.NextCheck = dateNow + (24 * 60 * 60);
                        }
                    }
                }
            }

        }

        public void AddBuffs(List<Buff> splitBuffs, float timeCoefficient = 1)
        {
            if (splitBuffs.Count == 0)
                return;

            var dateNow = TimeManager.Instance.NowUnixSeconds;

            foreach (var bufStatic in App.Instance.StaticData.Buffs)
            {
                var extraTime = (long)((bufStatic.Time * 60 * 60) * timeCoefficient);

                foreach (var buffData in splitBuffs)
                {
                    if (bufStatic.Id == buffData.Id)
                    {
                        long sameBuffTime = 0;
                        var userEffect = App.Instance.Player.Effect;

                        if (userEffect != null)
                        {
                            foreach (var element in userEffect.BuffList)
                            {
                                var elementStatic = App.Instance.StaticData.Buffs.Find(x => x.Id == element.Id);

                                if (element.Id != bufStatic.Id)
                                {
                                    if (elementStatic != null && elementStatic.Filter == bufStatic.Filter &&
                                        bufStatic.Priority >=
                                        elementStatic.Priority)
                                    {
                                        element.SetNewTime(element.Time + extraTime);
                                    }
                                }
                            }
                        }

                        var buffOn = App.Instance.Player.Effect.BuffList.Find(x => x.Id == bufStatic.Id);
                        if (buffOn != null)
                        {
                            if (buffOn.Time > dateNow)
                            {
                                sameBuffTime = buffOn.Time - dateNow;
                            }
                        }


                        var buffSetData = new CommonDLL.Dto.Buff(bufStatic.Value)
                        {
                            Id = bufStatic.Id,
                            Type = bufStatic.Type,
                            Time = dateNow + sameBuffTime + extraTime,
                        };

                        App.Instance.Player.Effect.BuffList.Add(new Buff(bufStatic, buffSetData));

                        if (buffSetData.Type == BuffType.Currency)
                        {
                            buffSetData.NextCheck = dateNow + (24 * 60 * 60);
                        }
                    }
                }
            }

            App.Instance.Player.Save();

            
        }

    }

}