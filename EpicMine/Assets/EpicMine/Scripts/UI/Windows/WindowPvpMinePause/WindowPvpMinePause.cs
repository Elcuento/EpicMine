using BlackTemple.Common;
using BlackTemple.EpicMine.Core;
using CommonDLL.Static;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class WindowPvpMinePause : WindowBase
    {
        [SerializeField] private Checkbox _music;
        [SerializeField] private Checkbox _sound;
        [SerializeField] private Checkbox _lowQuality;

        [Space]
        [SerializeField] private Image _language;

        public void OnClickSwitchLanguage()
        {
            WindowManager.Instance.Show<WindowSwitchLanguage>()
                .Initialize(OnCloseLanguageWindow);
        }

        private void OnCloseLanguageWindow()
        {
            _language.sprite = SpriteHelper.GetFlagLongCode(LocalizationHelper.GetCurrentLanguage().ToString());
        }


        public void OnClickLeavePvpArena()
        {
            PvpArenaNetworkController.I.Leave(() =>
            {
                SceneManager.Instance.LoadScene(ScenesNames.PvpArena);
                Close();
            });

        }

        public void RateUs()
        {
            WindowManager.Instance.Show<WindowRateUs>();
        }


        public override void OnShow(bool withPause = false, bool withCurrencies = false, bool withRating = false)
        {
            base.OnShow(withPause, withCurrencies);

            _language.sprite = SpriteHelper.GetFlagLongCode(LocalizationHelper.GetCurrentLanguage().ToString());
            _lowQuality.SetOn(PlayerPrefsHelper.LoadDefault(PlayerPrefsType.WallCracks, false));

            _music.SetOn(!AudioManager.Instance.IsMusicMuted, immediately: true);
            _sound.SetOn(!AudioManager.Instance.IsSoundsMuted, immediately: true);
            _music.OnChange += OnMusicChange;
            _sound.OnChange += OnSoundChange;
            _lowQuality.OnChange += OnQualityChange;
        }

        private void OnQualityChange(bool isOn)
        {
            PlayerPrefsHelper.Save(PlayerPrefsType.WallCracks, isOn);
            EventManager.Instance.Publish(new ChangeSettingsQualityEvent(isOn));
        }

        public override void OnClose()
        {
            base.OnClose();
            _music.OnChange -= OnMusicChange;
            _sound.OnChange -= OnSoundChange;
            _lowQuality.OnChange -= OnQualityChange;
        }


        private void OnSoundChange(bool isOn)
        {
            AudioManager.Instance.SetSoundsMuted(!isOn);
        }

        private void OnMusicChange(bool isOn)
        {
            AudioManager.Instance.SetMusicMuted(!isOn);
        }
    }
}