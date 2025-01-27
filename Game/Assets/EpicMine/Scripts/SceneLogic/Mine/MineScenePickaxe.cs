using BlackTemple.Common;

using CodeStage.AntiCheat.ObscuredTypes;
using CommonDLL.Static;
using DG.Tweening;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class MineScenePickaxe : MonoBehaviour
    {
        public ObscuredInt Health { get; private set; }
        public int HitCounter { get; private set; }

        public bool IsReadyClick => HitCounter > 0;

        [SerializeField] private MineSceneHero _hero;

        [SerializeField] private Animator _animator;

        private MineSceneSection _section;
        private MineScenePickaxeCustomize _customize;

        [Header("Torch Use")]
        [SerializeField] private Vector3 _usePosition;
        [SerializeField] private Vector3 _useRotation;

        private Vector3 _startRotation;
        private Vector3 _startPosition;

        private float _hitTimeCounter;
        private float _hitTimeMaxCounter = 1f / MineLocalConfigs.MaxPickaxeHit;


        private PickaxeEffect _hitEffect;


        public void AddHealth(int value)
        {
            Health += value;

            var maxHealth = App.Instance.StaticData.Configs.Dungeon.Mines.MaxHealth;
            if (Health > maxHealth)
                Health = maxHealth;

            OnHealthChange();
        }

        public void RemoveHealth(int value)
        {

            var health = (int)Health;

            health = health - value <= 0 ? 0 : health - value;

            Health = health;

            OnHealthChange();

            if (Health > 0)
                return;

            Health = 0;
            EventManager.Instance.Publish(new MineScenePickaxeDestroyedEvent(_section));
            AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.PickaxeDestroyed);

            
        }

        public void RefillHealth()
        {
            Health = App.Instance.StaticData.Configs.Dungeon.Mines.MaxHealth;
            OnHealthChange();
        }

        public void Dig(bool isMissed, bool takeDamage = true)
        {
            if (HitCounter <= 0)
                return;

            HitCounter--;

            _animator.Play("MinePickaxeDig", -1, 0f);

            if (isMissed)
            {
                var randomMissSoundIndex = Random.Range(0, App.Instance.ReferencesTables.Sounds.PickaxeMisses.Length);
                var randomMissSound = App.Instance.ReferencesTables.Sounds.PickaxeMisses[randomMissSoundIndex];
                AudioManager.Instance.PlaySound(randomMissSound);

                if (takeDamage)
                {
                    Health--;
                    OnHealthChange();

                    if (Health > 0)
                        return;

                    Health = 0;
                    EventManager.Instance.Publish(new MineScenePickaxeDestroyedEvent(_section));
                    AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.PickaxeDestroyed);
                    return;
                }
            }

            var randomHitSoundIndex = Random.Range(0, App.Instance.ReferencesTables.Sounds.PickaxeOuterHits.Length);
            var hitSound = App.Instance.ReferencesTables.Sounds.PickaxeOuterHits[randomHitSoundIndex];
            AudioManager.Instance.PlaySound(hitSound);

        }

        public void Hit(Vector3 position)
        {
            if (_hitEffect != null)
            {
                switch (_hitEffect.Type)
                {
                    case PickaxeEffectType.Freeze:
                        if (Random.Range(0, 100) > 3)
                            return;

                        _customize.BlinkEffect(Color.blue);
                        _hero.ApplyAbility(AbilityType.Freezing);

                        break;
                    case PickaxeEffectType.HealthRecover:
                        if (Random.Range(0, 100) > 3)
                            return;

                        var amount = 1;
                        var mainViewportPosition =
                            Camera.main.WorldToViewportPoint(position);

                        var windowFlyingIcons = WindowManager.Instance.Show<WindowFlyingIcons>();

                        windowFlyingIcons.Create(
                            App.Instance.ReferencesTables.Sprites.HeartIcon,
                            $"+{amount}",
                            mainViewportPosition,
                            Tags.PlayerHealthBar);

                        _customize.BlinkEffect(Color.red);
                        AddHealth(amount);

                        break;
                }
            }
        }

        private void OnTorchUse(MineSceneTorchUseEvent eventData)
        {
            transform.DOKill(true);

            if (eventData.IsStart)
            {
                _animator.enabled = false;
                transform.transform.DOLocalRotate(_useRotation, 0.2f);
                transform.transform.DOLocalMove(_usePosition, 0.2f);
            }
            else
            {
                transform.transform.DOLocalMove(_startPosition, 0.2f);
                transform.transform.DOLocalRotate(_startRotation, 0.2f).OnComplete(() => { _animator.enabled = true; });
            }

        }


        private void Start()
        {
            var path = Paths.ResourcesPrefabsPickaxesPath + App.Instance.Player.Blacksmith.SelectedPickaxe.StaticPickaxe.Id;
            var prefab = Resources.Load<GameObject>(path);

            var pickaxe = Instantiate(prefab, transform, false);
            Health = App.Instance.StaticData.Configs.Dungeon.Mines.MaxHealth;
            _startPosition = transform.localPosition;
            _startRotation = transform.localEulerAngles;


            EventManager.Instance.Subscribe<MineSceneSectionReadyEvent>(OnSectionReady);
            EventManager.Instance.Subscribe<MineSceneTorchUseEvent>(OnTorchUse);

            _hitEffect =
                App.Instance.StaticData.PickaxeEffects.Find(x =>
                    x.Id == App.Instance.Player.Blacksmith.SelectedPickaxe.StaticPickaxe.HitEffect);

            _customize = pickaxe.GetComponent<MineScenePickaxeCustomize>();

        }

        private void Update()
        {
            if (HitCounter <= MineLocalConfigs.MaxPickaxeHit)
            {
                _hitTimeCounter += Time.deltaTime;

                if (_hitTimeCounter < _hitTimeMaxCounter)
                    return;

                _hitTimeCounter = 0;

                HitCounter++;
            }
        }




        private void OnDestroy()
        {
            if (EventManager.Instance == null)
                return;

            EventManager.Instance.Unsubscribe<MineSceneSectionReadyEvent>(OnSectionReady);
            EventManager.Instance.Unsubscribe<MineSceneTorchUseEvent>(OnTorchUse);
        }

        private void OnSectionReady(MineSceneSectionReadyEvent eventData)
        {
            _section = eventData.Section;
        }

        private void OnHealthChange()
        {
            var healthChangeEvent = new MineScenePickaxeHealthChangeEvent(Health);
            EventManager.Instance.Publish(healthChangeEvent);
        }
    }
}