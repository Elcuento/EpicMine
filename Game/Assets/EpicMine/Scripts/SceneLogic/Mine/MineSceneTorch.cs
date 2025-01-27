using BlackTemple.Common;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class MineSceneTorch : MonoBehaviour
    {
        [SerializeField] private MineSceneHero _hero;

        private bool _isUsingTorch;
        private float _timer;

        private int _usingId;

        private void Start()
        {
            var path = Paths.ResourcesPrefabsTorchesPath + App.Instance.Player.TorchesMerchant.SelectedTorch.StaticTorch.Id;
            var prefab = Resources.Load<GameObject>(path);

            Instantiate(prefab, transform, false);

            EventManager.Instance.Subscribe<MineSceneSectionPassedEvent>(OnPassSection);
        }

        private void OnDestroy()
        {
            if (EventManager.Instance == null)
                return;

            EventManager.Instance.Unsubscribe<MineSceneSectionPassedEvent>(OnPassSection);
        }

        private void Update()
        {
            if (_isUsingTorch)
            {
                _timer += Time.deltaTime;

                if (_timer >= 1)
                {
                    if (_hero.EnergySystem.Value <= 0)
                    {
                        UseTorchEnd();
                        return;
                    }
                    _timer = 0;
                    _hero.EnergySystem.Subtract(MineLocalConfigs.TorchUseSecCoast);
                }
            } 
        }

        private void OnPassSection(MineSceneSectionPassedEvent eventData)
        {
            UseTorchEnd();
        }


        public bool UseTorchContinuous()
        {
            if (_isUsingTorch)
            {
                if (_hero.EnergySystem.Value <= 0)
                {
                    UseTorchEnd();
                    return false;
                }

                return false;
            }
            else
            {
                if (!_hero.EnergySystem.Subtract(MineLocalConfigs.TorchUseMomentCoast))
                {
                    return false;
                }

                if (!_isUsingTorch)
                {
                    AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.DragonLightStart);
                   _usingId = AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.DragonLightProgress, true);
                }

                _isUsingTorch = true;

                EventManager.Instance.Publish(new MineSceneTorchUseEvent(true));
                return true;
            }
        }

        public void UseTorchEnd()
        {
            if (!_isUsingTorch)
                return;

            AudioManager.Instance.StopSound(_usingId);

            _isUsingTorch = false;

            EventManager.Instance.Publish(new MineSceneTorchUseEvent(false));
        }
    }
}