using System;
using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using CommonDLL.Static;
using UnityEngine;
using Random = UnityEngine.Random;

public class MineSceneDeveloperPanel : MonoBehaviour {

    private DeveloperController _controller;

    
    public struct MonsterExtraAttributes
    {
        public bool Edited;
        public float HealthPercent;
        public float PickDamagePercent;
        public float MonsterDamage;
        public float MonsterAttackDelay;
        public float MonsterAttackSpeed;
        public float TorchCostSec;
        public float TorchCostMoment;
        public float MaxPickaxeHitSecond;

    }

    private void Start()
    {
       // EventManager.Instance.Subscribe<MineSceneSectionReadyEvent>(OnMineSectionReady);
    }

    private void OnMineSectionReady(MineSceneSectionReadyEvent data)
    {
        var monsterSection = data.Section as MineSceneMonsterSection;
        if (monsterSection != null)
        {
            ApplyMonsterStats();
        }
    }


    public void Initialize(DeveloperController developerController)
    {
        _controller = developerController;
        Fill();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Y))
        {
          //  _controller.Show(false);
          // ScreenCapture.CaptureScreenshot(Random.Range(0, 999).ToString() + ".png", 1);
         //   _controller.Show(true);
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            var hero =
                FindObjectOfType<MineSceneHero>();
            if (hero != null)
            {
                hero.Pickaxe.AddHealth(5);
            }
        }

        if (Input.GetKeyUp(KeyCode.Q))
        {
            var hero =
                FindObjectOfType<MineSceneHero>();
            if (hero != null)
            {
                hero.EnergySystem.Add(20);
            }
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            var section = FindObjectsOfType<MineSceneAttackSection>().FirstOrDefault(x => x.IsReady);

            if (section == null)
                return;

            section.TakeDamage(999999999, AttackDamageType.Pickaxe);
        }
    }

    public static MonsterExtraAttributes GetMonsterStats()
    {
        return PlayerPrefs.GetString(PlayerPrefsType.DeveloperMonsterStats.ToString(),
            GetNewMonsterStats(App.Instance.StaticData.Monsters.Find(x => x.Id == "spider_3"))
                .ToJson())
            .FromJson<MonsterExtraAttributes>();
    }

    public static MonsterExtraAttributes GetNewMonsterStats(Monster staticMonster)
    {
        return new MonsterExtraAttributes
        {
            MonsterAttackDelay = staticMonster.AttackDelay,
            MonsterAttackSpeed = staticMonster.AttackTime,
            MonsterDamage = staticMonster.Damage,
            TorchCostMoment = MineLocalConfigs.TorchUseMomentCoast,
            TorchCostSec = MineLocalConfigs.TorchUseSecCoast,
            HealthPercent = 0,
            PickDamagePercent = MineLocalConfigs.PickaxeMonsterDamageCoefficient,
            MaxPickaxeHitSecond = MineLocalConfigs.MaxPickaxeHit
        };
    }

    private void SaveMonsterStats(MonsterExtraAttributes stats)
    {
        stats.Edited = true;

        PlayerPrefs.SetString(PlayerPrefsType.DeveloperMonsterStats.ToString(),
            stats.ToJson());

    }

    private void ApplyMonsterStats()
    {
        var monsterAttr = GetMonsterStats();

        var monsterSection = FindObjectsOfType<MineSceneMonsterSection>().FirstOrDefault(x => x.IsReady);
        if (monsterSection != null)
        {
            monsterSection.EditorSetAttributes(monsterAttr);
        }

        _controller.OnToggleStats();
        _controller.OnToggleStats();
}

    private DeveloperControlPanelVerticalScrollItem FillMonstersList(DeveloperControlPanelButtonItem source)
    {
        var monsterList = _controller.AddDropDown((a) => { }, "Select Monster", App.Instance.StaticData.Monsters.ConvertAll(x => x.Id));

        return _controller.CreateVerticalScroll(source, new List<DeveloperControlPanelItem>
        {
            _controller.AddControlItem("Select Monster"),

            monsterList,

            _controller.AddButton(
                (xx) =>
                {
                    var monster = App.Instance.StaticData.Monsters.Find(x =>
                        x.Id == App.Instance.StaticData.Monsters[monsterList.GetValue()].Id);

                    var mineController = FindObjectOfType<MineSceneController>();
                    mineController.SectionProvider.ExchangeLastSection(mineController.SectionProvider.Sections.Last(), SectionType.Monster, monster.Id);

                    ApplyMonsterStats();
                },
                "Spawn in the next wall"),

        });
    }

    private DeveloperControlPanelVerticalScrollItem FillMonsters(DeveloperControlPanelButtonItem source)
    {
       
        var monsterAttr = GetMonsterStats();

        var monsterHealthPercent = _controller.AddInputField((a) => { }, monsterAttr.HealthPercent.ToString());
        var monsterPickDamagePercent = _controller.AddInputField((a) => { }, monsterAttr.PickDamagePercent.ToString());
        var monsterDamagePercent = _controller.AddInputField((a) => { }, monsterAttr.MonsterDamage.ToString());
        var monsterDamageDelay = _controller.AddInputField((a) => { }, monsterAttr.MonsterAttackDelay.ToString());
        var monsterDamageCastSpeed = _controller.AddInputField((a) => { }, monsterAttr.MonsterAttackSpeed.ToString());
        var monsterTorchEnergyCostSec = _controller.AddInputField((a) => { }, monsterAttr.TorchCostSec.ToString());
        var monsterTorchEnergyCostMoment = _controller.AddInputField((a) => { }, monsterAttr.TorchCostMoment.ToString());
        var pickaxeMaxHitSecond = _controller.AddInputField((a) => { }, monsterAttr.MaxPickaxeHitSecond.ToString());

        return _controller.CreateVerticalScroll(source, new List<DeveloperControlPanelItem>
        {
            _controller.AddControlItem("% add health"),
            monsterHealthPercent,

            _controller.AddControlItem("% less damage"),
            monsterPickDamagePercent,

            _controller.AddControlItem("melee damage"),
            monsterDamagePercent,

            _controller.AddControlItem("melee damage delay"),
            monsterDamageDelay,

            _controller.AddControlItem("melee damage speed"),
            monsterDamageCastSpeed,

            _controller.AddControlItem("torch cost moment"),
            monsterTorchEnergyCostMoment,

            _controller.AddControlItem("torch cost sec"),
            monsterTorchEnergyCostSec,

            _controller.AddControlItem("max hit per second"),
            pickaxeMaxHitSecond,

            _controller.AddButton(
                (xx) =>
                {
                    SaveMonsterStats(new MonsterExtraAttributes
                    {
                        HealthPercent = float.Parse(monsterHealthPercent.GetText()),
                        PickDamagePercent = float.Parse(monsterPickDamagePercent.GetText()),
                        MonsterDamage = float.Parse(monsterDamagePercent.GetText()),
                        MonsterAttackDelay = float.Parse(monsterDamageDelay.GetText()),
                        MonsterAttackSpeed = float.Parse(monsterDamageCastSpeed.GetText()),
                        TorchCostSec = float.Parse(monsterTorchEnergyCostSec.GetText()),
                        TorchCostMoment = float.Parse(monsterTorchEnergyCostMoment.GetText()),
                        MaxPickaxeHitSecond = float.Parse(pickaxeMaxHitSecond.GetText()),
                    });
                    ApplyMonsterStats();
                },
                "Save/Apply"),
            _controller.AddButton(
                (xx) =>
                {
                    SaveMonsterStats(GetNewMonsterStats(App.Instance.StaticData.Monsters.Find(x => x.Id == "spider_3")));
                    ApplyMonsterStats();
                },
                "Reset/Save"),
        });

    }


    public void Fill()
    {
        _controller.CreateVerticalScroll(null, new List<DeveloperControlPanelItem>
        {
            _controller.AddButton((y) =>
            {
                var section = FindObjectsOfType<MineSceneAttackSection>().FirstOrDefault(x => x.IsReady);

                section.TakeDamage(999999999,AttackDamageType.Pickaxe);

            }, "9xxx Damage"),

            _controller.AddButton((x) =>
            {
                _controller.CreateVerticalScroll(x, new List<DeveloperControlPanelItem>
                {
                    _controller.AddButton(
                        (xx) =>
                        {
                            var control = FindObjectOfType<MineSceneController>();
                            if (control != null)
                            {
                                control.Hero.Pickaxe.RefillHealth();
                            }
                        },
                        "Health"),
                    _controller.AddButton(
                        () =>
                        {
                            var hero =
                                FindObjectOfType<MineSceneHero>();

                            if (hero != null)
                                hero.EnergySystem.Add(1000);
                        },
                        "Energy"),
                });
            }, "Stats"),

           /* _controller.AddButton((x) =>
            {
                FillMonsters(x);
            }, "Monsters"),*/

           _controller.AddButton((x) =>
            {
                _controller.CreateVerticalScroll(x, new List<DeveloperControlPanelItem>
                {
                    _controller.AddButton(
                        (xx) =>
                        {
                            var pointArea = FindObjectsOfType<MineSceneAttackAreaCoordinatesPoints>().First(y => y.Section != null && y.Section.IsReady);

                            pointArea.EditorReset();

                            for (var i = 0; i < pointArea.PointArea.LengthX; i+=2)
                            for (var j = 0; j < pointArea.PointArea.LengthY; j+=2)
                            {
                                pointArea.CreateAttackPoint(i, j, 2);
                            }
                        },
                        "Fill all"),
                    _controller.AddButton(
                        () =>
                        {
                            var pointArea = FindObjectsOfType<MineSceneAttackArea>();
                            foreach (var mineSceneAttackPointsArea in pointArea)
                            {
                                if (mineSceneAttackPointsArea.Section!=null && mineSceneAttackPointsArea.Section.IsReady)
                                {
                                    mineSceneAttackPointsArea.EditorReset();
                                }
                            }
                        },
                        "Clear all"),
                });
            }, "Field"),
            _controller.AddButton((x) =>
                {
                    if (SceneManager.Instance.CurrentScene != ScenesNames.PvpArena)
                    {
                        FillMine(x);
                    }
                    else FillPvpMine(x);

                }, SceneManager.Instance.CurrentScene != ScenesNames.PvpArena ? "Mine Etc" : "Pvp Mine Etc"),
                _controller.AddButton((x) =>
                {
                    if (SceneManager.Instance.CurrentScene != ScenesNames.PvpArena)
                    {
                        FillMonstersList(x);
                    }

                }, "Monsters"),

            _controller.AddButton((x) =>
            {
                _controller.CreateVerticalScroll(x, new List<DeveloperControlPanelItem>
                {    _controller.AddButton(
                        () =>
                        {
                           SceneManager.Instance.LoadScene(ScenesNames.Village);
                        },
                        "Force to Village"),

                    _controller.AddButton(
                        () =>
                        {
                            App.Instance.Restart();
                            TimeManager.Instance.SetPause(false);
                        },
                        "Restart"),

                    _controller.AddButton(
                        () =>
                        {
                            _controller.Show(false);
                            ScreenCapture.CaptureScreenshot(Random.Range(0,999).ToString() +".png",1);
                            _controller.Show(true);
                        },
                        "ScreenShot"),
                });
            }, "System")
        });

        _controller.Rebuild();
    }


    private DeveloperControlPanelVerticalScrollItem FillMine(DeveloperControlPanelButtonItem source)
    {
        return _controller.CreateVerticalScroll(source,
            new List<DeveloperControlPanelItem>
            {
                _controller.AddButton(
                    () =>
                    {
                        var mine =
                            App.Instance.Services.RuntimeStorage.Load<BlackTemple.EpicMine.Core.Mine>(RuntimeStorageKeys
                                .SelectedMine);
                        if (mine == null)
                            return;

                        if (mine.IsLast)
                        {
                            var nextTier = App.Instance.Player.Dungeon.Tiers.FirstOrDefault(t => t.IsOpen == false);
                            if (nextTier == null)
                                return;

                           /* var request = new OpenTierDeveloperRequest(nextTier.Number);
                            NetworkManager.Instance.Send<ResponseBase>(request, (res) =>
                            {
                                nextTier.Open(dev: true, onComplete: (a) =>
                                {
                                    if (a)
                                        EventManager.Instance.Publish(new MineSceneEndGameEvent());
                                });
                            }, (a) => { print("error"); });*/

                        }
                        else
                        {
                            if (!mine.IsComplete)
                                mine.Complete();

                            EventManager.Instance.Publish(new MineSceneEndGameEvent());
                        }
                    },
                    "Complete mine"),
                _controller.AddButton(
                    () =>
                    {
                        var wall =
                            FindObjectsOfType<MineSceneWallSection>().First(x=>x.IsReady);

                        if(wall != null)
                            wall.SummonGhost();

                    },
                    "Poltergeist"),

               /* _controller.AddButton(
                    () =>
                    {
                          FieldHelper.LoadFieldData(Application.persistentDataPath, App.Instance.ReferencesTables.FileData.FieldData.text);

                        Debug.LogError("Data Reloaded");
                    },
                    "Reload Field Data"),*/             

                _controller.AddButton(
                    () =>
                    {
                        App.Instance.Player.SetTutorialStep(Enum.GetNames(typeof(TutorialStepIds)).Length);
                        TimeManager.Instance.SetPause(false);
                        App.Instance.Restart();
                    },
                    "Skip tutorial"),
            });
    }

    private DeveloperControlPanelVerticalScrollItem FillPvpMine(DeveloperControlPanelButtonItem source)
    {
        return _controller.CreateVerticalScroll(source,
            new List<DeveloperControlPanelItem>
            {
                _controller.AddButton(
                    () =>
                    {
                        var scenePvpController =
                            FindObjectOfType<PvpArenaNetworkController>();
                        if (scenePvpController != null)
                        {
                          //  scenePvpController.SetWinner(scenePvpController.PlayerData.Name);
                        }
                    },
                    "Win"),

                _controller.AddButton(
                    () =>
                    {
                        var scenePvpController =
                            FindObjectOfType<PvpArenaNetworkController>();
                        if (scenePvpController != null)
                        {
                           // scenePvpController.SetWinner(scenePvpController.OpponentData.Name);
                        }
                    },
                    "Loose"),
                _controller.AddButton(
                    () =>
                    {
                        var scenePvpController =
                            FindObjectOfType<PvpArenaNetworkController>();
                        if (scenePvpController != null)
                        {
                          //  scenePvpController.CheckRoomCount();
                        }
                    },
                    "Room count"),
            });
    }

    public void OnDestroy()
    {
        if (EventManager.Instance == null)
            return;
    }
}



/* Attack point outline
        public void ShowCorners()
        {
            _innerRender.gameObject.SetActive(!_innerRender.gameObject.activeSelf);
            _outerRender.gameObject.SetActive(!_outerRender.gameObject.activeSelf);

            if (!_innerRender.gameObject.activeSelf)
                return;

            _innerRender.positionCount = 5;
            var innerSize = _inner.bounds.extents;

            _innerRender.SetPositions(new[]{ new Vector3(-innerSize.x, -innerSize.y),
                new Vector3(innerSize.x, -innerSize.y), new Vector3(innerSize.x, innerSize.y),
                new Vector3(-innerSize.x, innerSize.y), new Vector3(-innerSize.x, -innerSize.y), });

            _innerRender.transform.SetParent(null);
            _innerRender.transform.localScale = new Vector3(1, 1, 1);
            _innerRender.transform.SetParent(transform);


            _outerRender.positionCount = 5;
            var outerSize = _outer.bounds.extents;

            _outerRender.SetPositions(new[]{ new Vector3(-outerSize.x, -outerSize.y),
                new Vector3(outerSize.x, -outerSize.y), new Vector3(outerSize.x, outerSize.y),
                new Vector3(-outerSize.x, outerSize.y), new Vector3(-outerSize.x, -outerSize.y), });


            _outerRender.transform.SetParent(null);
            _outerRender.transform.localScale = new Vector3(1, 1, 1);
            _outerRender.transform.SetParent(transform);
        }

    */