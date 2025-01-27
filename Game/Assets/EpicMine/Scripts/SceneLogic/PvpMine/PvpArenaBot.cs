using System;
using System.Collections;
using System.Collections.Generic;
using BlackTemple.Common;
using CommonDLL.Dto;
using CommonDLL.Static;
using UnityEngine;
using Pickaxe = CommonDLL.Static.Pickaxe;
using Random = UnityEngine.Random;

// ReSharper disable StringLiteralTypo


namespace BlackTemple.EpicMine
{
    public class PvpArenaBot : MonoBehaviour
    {

        private PvpArenaNetworkController _photon;

        private PvpBot _botData;
        private Pickaxe  _pickaxe;

        private int _currentSection;
        private List<int> _wallSections;
        private int _currentSectionDamage;

        private int _statisticPassSpeed;

        private float _onStartEmoChance;
        private float _onPassEmoChance;
        private float _onEndEmoChance;

        private float _disconnectBufferSteps = 0;
        private bool _isNewbieMode;

        private bool _isActive;

        public void OnApplicationPause(bool pause)
        {
            if (_photon != null && _isActive)
            {
                PvpArenaNetworkController.I.Leave(() =>
                {

                });
                StopAllCoroutines();
                _isActive = false;
            }
        }


        public void Awake()
        {
            EventManager.Instance.Subscribe<PvpArenaStartGameEvent>(GameStart);
            EventManager.Instance.Subscribe<PvpArenaEndGameEvent>(EndGame);
            EventManager.Instance.Subscribe<PvpArenaOnClickLeaveRoomEvent>(OnLeaveGame);
        }

        
        public void OnDestroy()
        {
            if (EventManager.Instance != null)
            {
                EventManager.Instance.Unsubscribe<PvpArenaStartGameEvent>(GameStart);
                EventManager.Instance.Unsubscribe<PvpArenaEndGameEvent>(EndGame);
                EventManager.Instance.Unsubscribe<PvpArenaOnClickLeaveRoomEvent>(OnLeaveGame);
            }
        }

        public void Initialize(PvpArenaNetworkController photon,PvpArenaUserInfo info)
        {
            _photon = photon;

            _botData =  App.Instance.StaticData.PvpBots.Find(x => x.Id == info.Id);
            _pickaxe = App.Instance.StaticData.Pickaxes.Find(x => x.Id == info.Pickaxe);

            _onEndEmoChance = _botData.EmoEndChance;
            _onPassEmoChance = _botData.EmoPassWallChance;
            _onStartEmoChance = _botData.EmoStartChance;

            _isNewbieMode = App.Instance.Player.Pvp.Rating <= PvpLocalConfig.BotGuaranteeWeakRatingMax;

        }

        public void OnLeaveGame(PvpArenaOnClickLeaveRoomEvent eventData)
        {
            _isActive = false;
            StopAllCoroutines();
        }
        public void EndGame(PvpArenaEndGameEvent eventData)
        {
            _isActive = false;
            StopAllCoroutines();
        }

        public void GameStart(PvpArenaStartGameEvent eventData)
        {
            var matchData = PvpArenaNetworkController.GetMatchData();

            if (matchData == null)
            {
                return;
            }

            _wallSections = matchData?
                .Walls;
            _currentSection = 0;
            _currentSectionDamage = 0;

            _isActive = true;
            StartCoroutine(IdleState());

        }

   
        public IEnumerator IdleState()
        {
            yield return new WaitForSeconds(PvpLocalConfig.DefaultPvpMineStartTime+2);

            DoAppear();

            while (true)
            {
                var passTime = PvpBotHelper.GetSectionPassSpeed(_botData, _pickaxe);

                 passTime = passTime < _botData.BreakWallTimeMin
                    ? _botData.BreakWallTimeMinLimit == 1 ? passTime : _botData.BreakWallTimeMin : passTime;
  
                if (_isNewbieMode)
                    passTime *= PvpLocalConfig.NewbiePassCoefficient;
                

                var wallHealthCoefficient = App.Instance.StaticData.MineWalls[_currentSection].HealthCoefficient;
                passTime = (int) (passTime * wallHealthCoefficient);

                App.Instance.Services.LogService.Log($"Pass time : {passTime} wall coef {wallHealthCoefficient} newbie coef {(_isNewbieMode ? PvpLocalConfig.NewbiePassCoefficient : 1)}" );


                yield return new WaitForEndOfFrame();

                if (Application.internetReachability != NetworkReachability.NotReachable &&
                    _disconnectBufferSteps > 0)
                {
                    _disconnectBufferSteps--;
                    passTime = 0;
                }
                
                while (true)
                {
                    yield return new WaitForSeconds(passTime == 0 ? 0 : 1);

                    var randomDamage = (int) Random.Range(_wallSections[_currentSection] * 0.1f,
                        _wallSections[_currentSection] / (float)passTime);

                    _currentSectionDamage = randomDamage > _wallSections[_currentSection]
                        ? _wallSections[_currentSection]
                        : _currentSectionDamage + randomDamage;

                    passTime--;

                    if (passTime <= 0) break;
                }


                if (_currentSectionDamage < _wallSections[_currentSection])
                {
                    _currentSectionDamage = _wallSections[_currentSection];
                }

                try
                {
                    if (Application.internetReachability != NetworkReachability.NotReachable)
                    {
                        DoPassSection();

                        if (_currentSection >= PvpLocalConfig.DefaultPvpMineSectionCount)
                        {
                            DoEnd();
                            yield break;
                        }

                    }
                    else
                    {
                        _disconnectBufferSteps++;
                    }
                }

                catch (Exception)
                {
                    // ignored
                }
            }
        }

        public void SendEmo(EmoType type)
        {
            EventManager.Instance.Publish(new PvpArenaGetEmodjiEvent((int)type));
        }

        public void DoAppear()
        {

            if (Random.Range(0, 100) < _onStartEmoChance)
            {
                SendEmo((EmoType)Random.Range(0, 8));
            }
        }

        public void DoEnd()
        {
            if (Random.Range(0, 100) < _onEndEmoChance)
            {
                SendEmo((EmoType) Random.Range(0,8));
            }
        }

        public void DoPassSection()
        {
            if (Random.Range(0, 100) < _onPassEmoChance)
            {
                SendEmo((EmoType)Random.Range(0, 8));
            }

            _photon.SetOpponentProperty(new Dictionary<PvpArenaUserPropertyType, object>
            {
                {
                    PvpArenaUserPropertyType.Wall, _currentSection + 1
                },
                {
                    PvpArenaUserPropertyType.Damage,
                    _photon.GetOpponentProperty<int>(PvpArenaUserPropertyType.Damage) + _currentSectionDamage
                }
            });

            _currentSection++;
            _currentSectionDamage = 0;
            
        }

    }
}
