using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using CommonDLL.Dto;
using CommonDLL.Static;
using Newtonsoft.Json;
using UnityEngine;
using Currency = BlackTemple.EpicMine.Dto.Currency;


public class VillageSceneDeveloperPanel : MonoBehaviour {

    private DeveloperController _controller;


    public void Initialize(DeveloperController developerController)
    {
        _controller = developerController;
        Fill();
    }


    public void Fill()
    {
        var currencyInputField = _controller.AddInputField((a) => { }, "0");
        var currencyToggle = _controller.AddToggle((action) => { }, "Increment/Decrement");

        var chestsField = _controller.AddInputField((a) => { }, "0");

        var tierInputField = _controller.AddInputField((a) => { }, "0");

        var tutorialStepDropDown = _controller.AddDropDown((a) => { },
            "Tutorial Step", Enum.GetNames(typeof(TutorialStepIds)).ToList());

        var pvpInputField = _controller.AddInputField((a) => { }, "0");

        var quests = _controller.AddDropDown((a) => { },
            "Tutorial Step", App.Instance.Player.Quests.QuestList.ConvertAll(x=>x.StaticQuest.Id).ToList());

        _controller.CreateVerticalScroll(null, new List<DeveloperControlPanelItem>
        {
            _controller.AddButton((x) =>
            {
                _controller.CreateVerticalScroll(x, new List<DeveloperControlPanelItem>
                {
                    currencyInputField,
                    currencyToggle,
                    _controller.AddButton((y) =>
                    {
                        var amount = int.Parse(currencyInputField.GetText());

                        if (currencyToggle.Status())
                        {
                            App.Instance.Player.Wallet.Add(new Currency(CurrencyType.Gold, amount), IncomeSourceType.FromBuy);
                        }
                        else
                        {
                            App.Instance.Player.Wallet.Remove(new Currency(CurrencyType.Gold, amount));
                        }
                    }, "Add/Remove Gold"),
                _controller.AddButton(
                        (xx) =>
                        {
                          var dir = currencyToggle.Status() ? 1 : -1;
                          var amount = int.Parse(currencyInputField.GetText());

                          if (dir > 0)
                          {
                              App.Instance.Player.Wallet.Add(new Currency(CurrencyType.Crystals,  amount), IncomeSourceType.FromCheating);
                          }
                          else
                          {
                              App.Instance.Player.Wallet.Remove(new Currency(CurrencyType.Crystals, amount));
                          }
                        },
                        "Add/Remove Crystals"),
                    _controller.AddButton(
                        () =>
                        {
                            var dir = currencyToggle.Status() ? 1 : -1;
                            var amount = int.Parse(currencyInputField.GetText());

                            foreach (var res in App.Instance.StaticData.Resources)
                            {
                                if (dir > 0)
                                {
                                    App.Instance.Player.Inventory.Add(new Item(res.Id, amount), IncomeSourceType.FromPvp);
                                }
                                else
                                {
                                    App.Instance.Player.Inventory.Remove(new Item(res.Id, amount), SpendType.Using);

                                }

                            }
                        },
                        "Add/Remove items"),
                    _controller.AddButton(
                        () =>
                        {
                            var amount = int.Parse(currencyInputField.GetText());

                            App.Instance.Player.Artefacts.Remove(App.Instance.Player.Artefacts.Amount);
                            App.Instance.Player.Artefacts.Add(amount);

                        },
                        "Set Artefacts")
                });
            }, "Currency/Items"),

            _controller.AddButton((x) =>
            {
                _controller.CreateVerticalScroll(x, new List<DeveloperControlPanelItem>
                {
                    _controller.AddButton(
                        () =>
                        {
                            WindowManager.Instance.Show<WindowOpenGift>()
                                .Initialize(App.Instance.Player.Gifts.OpenedCount);
                        },
                        "Get simple gift (if not day limit)"),
                    _controller.AddButton(
                        () =>
                        {
                            WindowManager.Instance.Show<WindowOpenGift>()
                                .Initialize(App.Instance.StaticData.Configs.Gifts.DailyCount - 1);
                        },
                        "Get rare gift (if not day limit)"),

                    chestsField,
                    _controller.AddButton(
                        () => { App.Instance.Player.Burglar.AddChest(ChestType.Simple, int.Parse(chestsField.GetText())); },
                        "Get Simple Chest by level"),
                    _controller.AddButton(
                        () => { App.Instance.Player.Burglar.AddChest(ChestType.Royal, int.Parse(chestsField.GetText())); },
                        "Get Rare Chest, by level")
                });
            }, "Gifts/Chests"),

            _controller.AddButton((x) =>
            {
                _controller.CreateVerticalScroll(x, new List<DeveloperControlPanelItem>
                {
                    tierInputField,
                    _controller.AddButton(
                        () =>
                        {
                            var tierNumber = int.Parse(tierInputField.GetText());
                            tierNumber = tierNumber > App.Instance.StaticData.Tiers.Count-1 ? App.Instance.StaticData.Tiers.Count - 1 : tierNumber;

                            foreach (var tier in App.Instance.Player.Dungeon.Tiers)
                            {
                                if (tier.Number - 1 <= tierNumber)
                                {
                                    if(tier.IsOpen)
                                        continue;

                                    tier.Open(dev:true);
                                }
                            }
                        },
                        "Open Tier"),
                });
            }, "Tiers/Mines"),

            _controller.AddButton((x) =>
            {
                _controller.CreateVerticalScroll(x, new List<DeveloperControlPanelItem>
                {
                    tutorialStepDropDown,
                    _controller.AddButton(
                        () =>
                        {
                            var tutorialStepNumber = tutorialStepDropDown.GetValue();

                            App.Instance.Player.SetTutorialStep(tutorialStepNumber);
                            App.Instance.Restart();
                        },
                        "Set tutorial step"),
                    _controller.AddButton(
                        () =>
                        {
                            App.Instance.Player.SetTutorialStep(tutorialStepDropDown.GetValue());
                            TimeManager.Instance.SetPause(false);
                            App.Instance.Restart();

                        },
                        "Skip tutorial"),
                });
            }, "Tutorial"),

            _controller.AddButton((x) =>
            {
                _controller.CreateVerticalScroll(x, new List<DeveloperControlPanelItem>
                {
                    pvpInputField,
                    _controller.AddButton(
                        () =>
                        {
                            var rating = int.Parse(pvpInputField.GetText());
                            App.Instance.Player.Pvp.SetRating(rating);

                        },
                        "Set pvp rating")
                });
            }, "PvP"),

            _controller.AddButton((x) =>
            {
                _controller.CreateVerticalScroll(x, new List<DeveloperControlPanelItem>
                {
                    _controller.AddButton(
                        () =>
                        {
                            App.Instance.Player.Abilities.Drop();
                            App.Instance.Player.Skills.Drop();
                            App.Instance.Restart();

                        },
                        "Drop Skills/Abilities")
                });
            }, "Skills/Abilities"),

            _controller.AddButton((x) =>
            {
                _controller.CreateVerticalScroll(x, new List<DeveloperControlPanelItem>
                {
                    _controller.AddButton(
                        () =>
                        {
                            var offers = App.Instance.StaticData.ShopPacks.Where(xx=>xx.Type == ShopPackType.SpecialOffer);
                            foreach (var of in offers)
                            {
                                App.Instance.Player.Shop.AddShopOffer(of.Id);
                            }

                        },
                        "Get all offers")
                });
            }, "Shop"),
            _controller.AddButton((x) =>
            {
                _controller.CreateVerticalScroll(x, new List<DeveloperControlPanelItem>
                {
                    _controller.AddButton(
                        () =>
                        {
                            var picks = App.Instance.Player.Blacksmith.Pickaxes;
                            foreach (var pickax in picks)
                            {
                                pickax.Create(force: true);
                            }

                        },
                        "Get all pick"),
                    _controller.AddButton(
                        () =>
                        {
                            var torches = App.Instance.Player.TorchesMerchant.Torches;
                            foreach (var torch in torches)
                            {
                                torch.Create(force: true);
                            }

                        },
                        "Get all torches")
                });
            }, "Items"),

            _controller.AddButton((x) =>
            {
                _controller.CreateVerticalScroll(x, new List<DeveloperControlPanelItem>
                {

                    _controller.AddButton(
                        () =>
                        {
                            App.Instance.Player.Inventory.Add(new Item("chiken_egg",1), IncomeSourceType.FromBuy);

                        },
                        "Get Chicken egg"),

                    _controller.AddButton(
                        () =>
                        {
                           EventManager.Instance.Publish(new GiftOpenEvent());

                        },
                        "Add Gift to Quests"),

                    _controller.AddButton(
                    () =>
                    {
                        EventManager.Instance.Publish(new OpenChestEvent(ChestType.Simple));

                      },
                    "Add Simple Chest to Quests"),

                   _controller.AddButton(
                    () =>
                    {
                        EventManager.Instance.Publish(new OpenChestEvent(ChestType.Royal));

                    },
                    "Add Rare Chest to Quests"),

                   _controller.AddControlItem("Select quest"),

                   quests,

                   _controller.AddControlItem("Commands"),

                   _controller.AddButton(
                       () =>
                       {
                           var quest = App.Instance.Player.Quests.QuestList[quests.GetValue()];
                           quest.SetActivate();

                       },
                       "Activate Quest"),

                   _controller.AddButton(
                       () =>
                       {
                           var quest = App.Instance.Player.Quests.QuestList[quests.GetValue()];
                           quest.SetStart();

                       },
                       "Start quest"),

                   _controller.AddButton(
                       () =>
                       {
                           var quest = App.Instance.Player.Quests.QuestList[quests.GetValue()];
                           quest.SetComplete();

                       },
                       "Complete quest"),
                   _controller.AddButton(
                       () =>
                       {
                           var quest = App.Instance.Player.Quests.QuestList[quests.GetValue()];
                           quest.SetReset();

                       },
                       "Reset(until restart) quest"),

                   _controller.AddControlItem("Common"),

                   _controller.AddButton(
                       () =>
                       {
                           foreach (var quest1 in App.Instance.Player.Quests.QuestList)
                           {
                               quest1.SetActivate();
                           }
                       },
                       "Activate ALL Quests"),

                   _controller.AddButton(
                       () =>
                       {
                           foreach (var quest1 in App.Instance.Player.Quests.QuestList)
                           {
                               quest1.SetComplete();
                           }
                       },
                       "Complete ALL Quests"),

                   _controller.AddButton(
                       () =>
                       {
                           foreach (var quest1 in App.Instance.Player.Quests.QuestList)
                           {
                               quest1.SetStart();
                           }
                       },
                       "Start ALL Quests"),

                   _controller.AddButton(
                       () =>
                       {
                           foreach (var quest1 in App.Instance.Player.Quests.QuestList)
                           {
                               quest1.SetReset();
                           }
                       },
                       "Deactivate ALL Quests"),

                });
            }, "Quests"),

            _controller.AddButton((x) =>
            {
                _controller.CreateVerticalScroll(x, new List<DeveloperControlPanelItem>
                {
                    _controller.AddButton(
                        () =>
                        {
                          App.Instance.Services.AdvertisementService.ShowRewardedVideo(AdSource.UnlockTorch);
                        },
                        "Show rewarded ads"),

                    _controller.AddButton(
                        () =>
                        {
                            App.Instance.Services.AdvertisementService.ShowInterstitialVideo(false);
                        },
                        "Show interstitial ads"),

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
                            PlayerPrefs.DeleteAll();
                            PlayerPrefs.Save();

                            App.Instance.Restart();
                            TimeManager.Instance.SetPause(false);

                        },
                        "Clear Player Prefs"),
                    _controller.AddButton((xx) =>
                    {
                        _controller.CreateVerticalScroll(xx, new List<DeveloperControlPanelItem>
                        {
                            _controller.AddButton(
                                () =>
                                {
                                    App.Instance.Controllers.LocalPushNotificationsController.Clear();
                                    App.Instance.Controllers.LocalPushNotificationsController.SaveResetNotification();
                                },
                                "Clear push"),

                            _controller.AddButton(
                                () =>
                                {
                                    App.Instance.Services.LocalPushNotificationsService.Push("Hello world", "Test Push", DateTime.Now.AddSeconds(5));
                                },
                                "Check Local push")
                        });
                    }, "Loc Push Notifications"),
                    _controller.AddButton((xx) =>
                    {
                        _controller.CreateVerticalScroll(xx, new List<DeveloperControlPanelItem>
                        {
                            _controller.AddButton(
                                () =>
                                {
                                   // InAppPurchaseManager.Instance.CheckSubscription()
                                },
                                "Check all iaps write to console")
                        });
                    }, "IAP"),
                });
            }, "System"),
            _controller.AddButton((xx) =>
            {
            _controller.CreateVerticalScroll(xx, new List<DeveloperControlPanelItem>
            {
                _controller.AddButton(
                    () =>
                    {
                        PlayerPrefs.DeleteAll();
                        PlayerPrefs.Save();
                        App.Instance.Restart();
                    },
                    "Reset all progress")
            });
        }, "Profile")

        });

        _controller.Rebuild();
    }


}
