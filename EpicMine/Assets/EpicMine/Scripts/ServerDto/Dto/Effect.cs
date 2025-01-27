using System.Collections.Generic;

namespace CommonDLL.Dto
{
    public class Effect
    {
        public List<Buff> BuffList;

        public Effect()
        {

        }
        public Effect(BlackTemple.EpicMine.Core.Effect data)
        {
            BuffList = new List<Buff>();

            foreach (var buff in data.BuffList)
            {
                var buffDto = new Buff()
                {
                    Id = buff.Id,
                    Type = buff.Type,
                    NextCheck = buff.NextCheck,
                    Time = buff.Time
                };

                buffDto.Values = new List<Buff.BuffValue>();

                foreach (var buffCore in buff.Value)
                {
                    buffDto.Values.Add(new Buff.BuffValue()
                    {
                        Type = buffCore.Key,
                        Value = buffCore.Value,
                    });
                }
                BuffList.Add(buffDto);
            }
        }
    }

}