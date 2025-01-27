using System.Collections;
using DragonBones;
using TMPro;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class AgreementsSceneController : MonoBehaviour
    {
        private const string PrivacyPolicyUrl = "https://www.blacktemple.ru/privacy-policy";
        private const string PrefsKey = "agreements_accepted";

        [SerializeField] private UnityArmatureComponent _burglar;
        [SerializeField] private GameObject _window;

        [SerializeField] private TextMeshProUGUI _topInfoTitle;
        [SerializeField] private TextMeshProUGUI _topInfoDescription;
        [SerializeField] private TextMeshProUGUI _centerInfo;
        [SerializeField] private TextMeshProUGUI _bottomInfo;
        [SerializeField] private TextMeshProUGUI _hint;
        [SerializeField] private TextMeshProUGUI _acceptButton;
        [SerializeField] private TextMeshProUGUI _policyButton;


        public void GoToPrivacyPolicy()
        {
            Application.OpenURL(PrivacyPolicyUrl);
        }

        public void AcceptAgreements()
        {
            _window.SetActive(false);
            PlayerPrefs.SetInt(PrefsKey, 1);
            PlayerPrefs.Save();
            UnityEngine.SceneManagement.SceneManager.LoadScene(ScenesNames.EntryPoint);
        }

        private void Awake()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            _burglar.gameObject.SetActive(false);
            _window.SetActive(false);

            SetText();

            var agreementsAccepted = PlayerPrefs.GetInt(PrefsKey, 0) == 1;
            if (agreementsAccepted)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(ScenesNames.EntryPoint);
            }
            else
            {
                _window.SetActive(true);
                StartCoroutine(PlayBurglarAnimation());
            }
        }

        private void SetText()
        {
            _topInfoTitle.text = LocalizationHelper.GetBuildInLocale("agreements_top_info_title");
            _topInfoDescription.text = LocalizationHelper.GetBuildInLocale("agreements_top_info_description");
            _centerInfo.text = LocalizationHelper.GetBuildInLocale("agreements_center_info");
            _bottomInfo.text = LocalizationHelper.GetBuildInLocale("agreements_bottom_info");
            _hint.text = LocalizationHelper.GetBuildInLocale("agreements_hint");
            _acceptButton.text = LocalizationHelper.GetBuildInLocale("agreements_accept_button");
            _policyButton.text = LocalizationHelper.GetBuildInLocale("agreements_policy_button");
        }

        private IEnumerator PlayBurglarAnimation()
        {
            var releaseAnimation = _burglar.animation.animations["Release_up"];
            var waitAnimation = _burglar.animation.animations["Waiting"];
            var tapAnimation = _burglar.animation.animations["Tap"];

            yield return new WaitForSeconds(0.3f);
            _burglar.gameObject.SetActive(true);
            _burglar.animation.Play(releaseAnimation.name, 1);

            yield return new WaitForSeconds(releaseAnimation.duration);
            _burglar.animation.Play(waitAnimation.name);

            while (true)
            {
                yield return new WaitForSeconds(3f);
                _burglar.animation.Play(tapAnimation.name, 1);

                yield return new WaitForSeconds(tapAnimation.duration);
                _burglar.animation.Play(waitAnimation.name);
            }
        }
    }
}