using System;
using System.Collections;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using CommonDLL.Static;
using UnityEngine;
using UnityEngine.UI;
using Buff = BlackTemple.EpicMine.Core.Buff;

public class VillageSceneTopButtonsController : MonoBehaviour
{
    [SerializeField] private GameObject _prestigeButtonContainer;
    [SerializeField] private GameObject _meltingButtonContainer;
    [SerializeField] private GameObject _resourceButtonContainer;

    [SerializeField] private Button _prestigeButton;
    [SerializeField] private Button _meltingButton;
    [SerializeField] private Button _resourceButton;


    private Buff _meltingBuff;
    private Buff _maxMeltingBuff;

    private Buff _resourceBuff;
    private Buff _maxResourceBuff;

    public void Awake()
    {
        Initialize();

        EventManager.Instance.Subscribe<EffectAddBuffEvent>(OnAddEffect);
    }

    public void OnAddEffect(EffectAddBuffEvent eventData)
    {
        Initialize();
    }


    public void OnDestroy()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.Unsubscribe<EffectAddBuffEvent>(OnAddEffect);
        EventManager.Instance.Unsubscribe<SecondsTickEvent>(OnTickResource);
    }

    public IEnumerator BuffTimer(Buff buff, WindowInformation window)
    {
        while (true)
        {
            if (buff.IsActive)
            {
                var date = new DateTime();
                date = date.AddSeconds(buff.TimeLeft);
                var time = TimeHelper.Format(date, true);

                window.ChangeText($" {LocalizationHelper.GetLocale("window_shop_time_left")}\n{time}");
            }
            else
            {
                OnTickResource(new SecondsTickEvent());
                TimerOff();
                window.Close();
                break;
            }

            yield return new WaitForSeconds(1);
        }

    }

    public void OnTickResource(SecondsTickEvent eventData)
    {
        if (_resourceButtonContainer.activeSelf && (_resourceBuff == null || !_resourceBuff.IsActive))
        {
            _resourceButtonContainer.SetActive(false);
        }

        if (_meltingButtonContainer.activeSelf && (_meltingBuff == null || !_meltingBuff.IsActive))
        {
            _meltingButtonContainer.SetActive(false);
        }
    }

    public void TimerOff()
    {
        if (!_resourceBuff.IsActive && !_meltingBuff.IsActive)
        {
            EventManager.Instance.Unsubscribe<SecondsTickEvent>(OnTickResource);
        }
    }

    public void TimerStart()
    {
        EventManager.Instance.Unsubscribe<SecondsTickEvent>(OnTickResource);
        EventManager.Instance.Subscribe<SecondsTickEvent>(OnTickResource);
    }


    public void Initialize()
    {
        if (App.Instance.Player.Prestige > 0)
        {
            _prestigeButtonContainer.SetActive(true);
            _prestigeButton.image.sprite = SpriteHelper.GetPrestigeIcon(App.Instance.Player.Prestige);
        }
        else
        {
            _prestigeButtonContainer.SetActive(false);
        }

        _resourceBuff = App.Instance.Player.Effect.GetBuff(BuffType.Boost, BuffValueType.Resource);
        _meltingBuff = App.Instance.Player.Effect.GetBuff(BuffType.Boost, BuffValueType.Melting);


        if (_resourceBuff != null && _resourceBuff.IsActive)
        {
            _resourceButtonContainer.SetActive(true);
            _resourceButton.image.sprite = SpriteHelper.GetEffectTimerIcon(_resourceBuff.Id);
            _maxResourceBuff = App.Instance.Player.Effect.GetMaxTimeBuff(_resourceBuff);
        }
        else _resourceButtonContainer.SetActive(false);

        if (_meltingBuff != null && _meltingBuff.IsActive)
        {
            _meltingButtonContainer.SetActive(true);
            _meltingButton.image.sprite = SpriteHelper.GetEffectTimerIcon(_meltingBuff.Id);
            _maxMeltingBuff = App.Instance.Player.Effect.GetMaxTimeBuff(_meltingBuff);
        }
        else _meltingButtonContainer.SetActive(false);



        if(_meltingButtonContainer.activeSelf || _resourceButtonContainer.activeSelf)
            TimerStart();

    }

    public void OnClickResourceBuff()
    {

        var windowInfo = WindowManager.Instance.Show<WindowInformation>();
        windowInfo.Initialize(
            _resourceBuff.Id,
            "",
            "OK",
            isNeedLocalizeHeader: true,
            isNeedLocalizeDescription: false,
            isNeedLocalizeButton: false, onClose: OnClose);

        StartCoroutine(BuffTimer(_maxResourceBuff, windowInfo));
    }

    public void OnClickPrestige()
    {
        if (App.Instance.Player.Prestige <= 0)
            return;

        var descriptionLocale = LocalizationHelper.GetLocale("window_prestige_info_description");
        var currentBuff = StaticHelper.GetCurrentPrestigeBuff();
        var description = string.Format(
            descriptionLocale,
            currentBuff.FortunePercent,
            currentBuff.CriticalPercent,
            currentBuff.GoldPercent);

        var windowInfo = WindowManager.Instance.Show<WindowInformation>();
        windowInfo.Initialize(
            "window_prestige_info_header",
            description,
            "OK",
            isNeedLocalizeDescription: false,
            isNeedLocalizeButton: false);
    }


    public void OnClickMeltingBuff()
    {
        var windowInfo = WindowManager.Instance.Show<WindowInformation>();
        windowInfo.Initialize(
            _meltingBuff.Id,
            "",
            "OK",
            isNeedLocalizeHeader: true,
            isNeedLocalizeDescription: false,
            isNeedLocalizeButton: false, onClose: OnClose);

        StartCoroutine(BuffTimer(_maxMeltingBuff, windowInfo));
    }




    public void OnClose()
    {
        StopAllCoroutines();
    }

}
