using System;
using BlackTemple.Common;
using CommonDLL.Static;


namespace BlackTemple.EpicMine
{
    public class AttackPointProbabilityController
    {
        public AttackPointTypeProbability Type { get; private set; }

        public int DonateWallSectionsPassedAmount { get; private set; }

        private const string FileName = "attackPointProbabilityData";

        private int _helpPickaxeDestroyedAmount;

        private int _brakingMineCompleteAmount;

        private int _brakingPickaxeDestroyAmount;

        private readonly IStorageService _storageService = new JsonDiskStorageService();


        public AttackPointProbabilityController()
        {
            if (_storageService.IsDataExists(FileName))
            {
                var data = _storageService.Load<Dto.AttackPointProbability>(FileName);
                if (data.Type == AttackPointTypeProbability.Donate)
                {
                    Type = AttackPointTypeProbability.Donate;
                    DonateWallSectionsPassedAmount = data.DonateWallSectionsPassedAmount;
                }
            }

            EventManager.Instance.Subscribe<CurrencyAddEvent>(OnCurrencyAdd);
            EventManager.Instance.Subscribe<MineSceneSectionPassedEvent>(OnSectionPassed);
            EventManager.Instance.Subscribe<MineScenePickaxeDestroyedEvent>(OnPickaxeDestroyed);
            EventManager.Instance.Subscribe<MineCompleteEvent>(OnMineComplete);
        }


        public void Save()
        {
            var data = new Dto.AttackPointProbability
            {
                Type = Type,
                DonateWallSectionsPassedAmount = DonateWallSectionsPassedAmount
            };

            _storageService.Save(FileName, data);
        }

        public void Clear()
        {
            Type = AttackPointTypeProbability.Default;
            _helpPickaxeDestroyedAmount = 0;
            _brakingMineCompleteAmount = 0;
            _brakingPickaxeDestroyAmount = 0;

            _storageService.Remove(FileName);
        }


        private void OnCurrencyAdd(CurrencyAddEvent eventData)
        {
            if (eventData.Currency.Type != CurrencyType.Crystals || eventData.IncomeSourceType != IncomeSourceType.FromBuy)
                return;

            OnProbabilityConditionPassed(AttackPointTypeProbability.Donate);
        }


        private void OnSectionPassed(MineSceneSectionPassedEvent eventData)
        {
            switch (Type)
            {
                case AttackPointTypeProbability.Default:
                    break;

                case AttackPointTypeProbability.Donate:
                    DonateWallSectionsPassedAmount++;
                    if (DonateWallSectionsPassedAmount >= App.Instance.StaticData.Configs.Dungeon.Mines.AttackPoints.Probabilities.Types.DonateWallSectionsPassedToDisable)
                        OnProbabilityPassed(AttackPointTypeProbability.Donate);
                    break;

                case AttackPointTypeProbability.Help:
                    if (eventData.Section.Number + 1 >= App.Instance.StaticData.Configs.Dungeon.Mines.AttackPoints.Probabilities.Types.HelpSectionNumberToDisable)
                        OnProbabilityPassed(AttackPointTypeProbability.Help);
                    break;
                case AttackPointTypeProbability.Braking:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnPickaxeDestroyed(MineScenePickaxeDestroyedEvent eventData)
        {
            switch (Type)
            {
                case AttackPointTypeProbability.Default:
                    var isHardcore = App.Instance
                        .Services
                        .RuntimeStorage
                        .Load<bool>(RuntimeStorageKeys.IsHardcoreMode);

                    if (isHardcore)
                        return;

                    if (_helpPickaxeDestroyedAmount >= App.Instance.StaticData.Configs.Dungeon.Mines.AttackPoints.Probabilities.Types.HelpPickaxeDestroyedAmountToEnable)
                        OnProbabilityConditionPassed(AttackPointTypeProbability.Help);
                    break;

                case AttackPointTypeProbability.Donate:
                    break;
                case AttackPointTypeProbability.Help:
                    break;
                case AttackPointTypeProbability.Braking:
                    _brakingPickaxeDestroyAmount++;
                    if (_brakingPickaxeDestroyAmount >= App.Instance.StaticData.Configs.Dungeon.Mines.AttackPoints.Probabilities.Types.BrakingPickaxeDestroyAmountToDisable)
                        OnProbabilityPassed(AttackPointTypeProbability.Braking);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnMineComplete(MineCompleteEvent eventData)
        {
            switch (Type)
            {
                case AttackPointTypeProbability.Default:
                    var isHardcore = App.Instance
                        .Services
                        .RuntimeStorage
                        .Load<bool>(RuntimeStorageKeys.IsHardcoreMode);

                    if (isHardcore)
                        return;

                    _brakingMineCompleteAmount++;
                    if (_brakingMineCompleteAmount >= App.Instance.StaticData.Configs.Dungeon.Mines.AttackPoints.Probabilities.Types.BrakingMineCompleteAmountToEnable)
                        OnProbabilityConditionPassed(AttackPointTypeProbability.Braking);
                    break;
                case AttackPointTypeProbability.Donate:
                    break;
                case AttackPointTypeProbability.Help:
                    break;
                case AttackPointTypeProbability.Braking:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnProbabilityConditionPassed(AttackPointTypeProbability probabilityType)
        {
            if (probabilityType == AttackPointTypeProbability.Donate)
            {
                Type = probabilityType;
                return;
            }

            if (Type == AttackPointTypeProbability.Donate)
                return;

            if (Type != AttackPointTypeProbability.Default)
                return;

            Type = probabilityType;
        }

        private void OnProbabilityPassed(AttackPointTypeProbability probabilityType)
        {
            Type = AttackPointTypeProbability.Default;

            DonateWallSectionsPassedAmount = 0;
            _helpPickaxeDestroyedAmount = 0;
            _brakingPickaxeDestroyAmount = 0;
            _brakingMineCompleteAmount = 0;
        }
    }
}