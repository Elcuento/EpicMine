using BlackTemple.Common;
using BlackTemple.EpicMine.Core;
using CommonDLL.Static;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTemple.EpicMine
{
    public class WindowSettings : WindowBase
    {
        [SerializeField] private Checkbox _music;
        [SerializeField] private Checkbox _sound;
        [SerializeField] private Checkbox _lowQuality;
        [SerializeField] private TextMeshProUGUI _version;

        [Space]
        [SerializeField] private Image _language;

        public override void OnShow(bool withPause = false, bool withCurrencies = false, bool withRating = false)
        {
            base.OnShow(withPause, withCurrencies);

            _lowQuality.SetOn(PlayerPrefsHelper.LoadDefault(PlayerPrefsType.WallCracks, false));

            _language.sprite = SpriteHelper.GetFlagLongCode(LocalizationHelper.GetCurrentLanguage().ToString());
            _music.SetOn(!AudioManager.Instance.IsMusicMuted, immediately: true);
            _sound.SetOn(!AudioManager.Instance.IsSoundsMuted, immediately: true);

            _music.OnChange += OnMusicChange;
            _sound.OnChange += OnSoundChange;
            _lowQuality.OnChange += OnQualityChange;

            _version.text = Application.version;
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


        public void ShowId()
        {
            var text = $"{App.Instance.Player.Id}/{Application.version}/{App.Instance.StaticData.Version}";
            WindowManager
                .Instance
                .Show<WindowInformation>()
                .Initialize(
                    "window_settings_id_button", 
                    text, 
                    "window_settings_id_copy_button", 
                    isNeedLocalizeDescription: false,
                    onClose: () => { GUIUtility.systemCopyBuffer = text; });
        }

        public void OnClickSwitchLanguage()
        {
            WindowManager.Instance.Show<WindowSwitchLanguage>()
                .Initialize(OnCloseLanguageWindow);
        }

        private void OnCloseLanguageWindow()
        {
            _language.sprite = SpriteHelper.GetFlagLongCode(LocalizationHelper.GetCurrentLanguage().ToString());
        }

        public void RateUs()
        {
            WindowManager.Instance.Show<WindowRateUs>();
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