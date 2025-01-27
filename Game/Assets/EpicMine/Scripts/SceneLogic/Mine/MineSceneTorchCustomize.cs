using System.Collections.Generic;
using BlackTemple.Common;
using CommonDLL.Static;
using DG.Tweening;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class MineSceneTorchCustomize : MonoBehaviour
    {
        [SerializeField] public TorchHandingType _torchType;
        [SerializeField] private ParticleSystem[] _movingParticles;
        [SerializeField] private AudioClip _audio;
        [SerializeField] private AudioClip _additionalAudio;

        [Header("Use torch")]
        [SerializeField] private Transform _root;
        [SerializeField] private Vector3 _usePosition;
        [SerializeField] private Vector3 _useRotation;

        [Space]
        [SerializeField] private ParticleSystem _useInstant;
        [SerializeField] private ParticleSystem _useContinue;

        [SerializeField] private float _useParticleMultiplier = -7.47f;

        private Vector3 _startRotation;
        private Vector3 _startPosition;

        private float _moveLimit;
        private float _stayLimit;

        private float _moveInheritVelocity;
        private float _moveLimitVelocityOverLifeTime;

        private float _stayInheritVelocity;
        private float _stayLimitVelocityOverLifeTime;

        private bool _isInitialized;

        private void OnDestroy()
        {
            if (AudioManager.Instance != null)
            {
                if (_audio != null)
                    AudioManager.Instance.StopSound(_audio);

                if (_additionalAudio != null)
                    AudioManager.Instance.StopSound(_additionalAudio);
            }

            if (EventManager.Instance != null)
            {
                EventManager.Instance.Unsubscribe<MineSceneHeroMoveEvent>(OnHeroChangeMoveState);
                EventManager.Instance.Unsubscribe<PvpArenaStartGameEvent>(OnGameStarted);
                EventManager.Instance.Unsubscribe<MineSceneTorchUseEvent>(OnTorchUse);
            }
        }


        private void Start()
        {
            EventManager.Instance.Subscribe<MineSceneHeroMoveEvent>(OnHeroChangeMoveState);
            EventManager.Instance.Subscribe<MineSceneTorchUseEvent>(OnTorchUse);

            if (SceneManager.Instance.CurrentScene == ScenesNames.Mine)
            {
                Initialize();
            }
            else
            {
                EventManager.Instance.Subscribe<PvpArenaStartGameEvent>(OnGameStarted);
            }

            _startPosition = _root.transform.localPosition;
            _startRotation = _root.transform.localEulerAngles;

            //   _usePosition = new Vector3(0.27f,-0.01f,-1.66f);
            //   _useRotation = new Vector3(-7.91f, -5.78f, -12.77f);

           // _moveLimit = 0.6f;
          //  _stayLimit = 1;

            //   _moveInheritVelocity = 0.9f;
           // _moveLimitVelocityOverLifeTime = 6;
           // _stayInheritVelocity = 1;
           // _stayLimitVelocityOverLifeTime = 1;
        }

        private void OnTorchUse(MineSceneTorchUseEvent eventData)
        {
            if (_root != null)
            {
                _root.DOKill(true);

                if (eventData.IsStart)
                {
                    foreach (var movingParticle in _movingParticles)
                    {
                        var z = movingParticle.velocityOverLifetime;
                        z.zMultiplier = _useParticleMultiplier;

                        var orbZ = movingParticle.velocityOverLifetime;
                        orbZ.orbitalZ = 0.22f;
                    }

                    _useInstant.Play();
                    _useContinue.Play();
                    _useContinue.loop = true; 

                    _root.transform.DOLocalRotate(_useRotation,0.2f);
                    _root.transform.DOLocalMove(_usePosition, 0.2f).OnComplete(() =>
                    {
                      
                    });
                }
                else
                {
                    _useContinue.loop = false;

                    foreach (var movingParticle in _movingParticles)
                    {
                        var z = movingParticle.velocityOverLifetime;
                        z.zMultiplier = 0;

                        var orbZ = movingParticle.velocityOverLifetime;
                        orbZ.orbitalZ = 0;
                    }
                    _root.transform.DOLocalMove(_startPosition, 0.2f);
                    _root.transform.DOLocalRotate(_startRotation, 0.2f).OnComplete(() =>
                    {

                    });
                }
                
            }
        }

        public void OnHeroChangeMoveState(MineSceneHeroMoveEvent eventData)
        {
            foreach (var movingParticle in _movingParticles)
            {
              //  var part = movingParticle.main;
              //  part.startLifetimeMultiplier = eventData.IsMoving ? _moveLimit : _stayLimit;

               // var inheritV = movingParticle.inheritVelocity;
              //  inheritV.curveMultiplier = eventData.IsMoving ? _moveInheritVelocity : _stayInheritVelocity;

              //  var lvelOtime = movingParticle.limitVelocityOverLifetime;
              //  lvelOtime.limitMultiplier = eventData.IsMoving ? _moveLimitVelocityOverLifeTime : _stayLimitVelocityOverLifeTime;

                //  var place = movingParticle.velocityOverLifetime;
               //       place.orbitalZ = eventData.IsMoving
               //      ? 1.5f
                //     : 0;

                //  movingParticle.simulationSpace = !eventData.IsMoving ? 
                //       ParticleSystemSimulationSpace.World : ParticleSystemSimulationSpace.Local;
            }

         
        }

        private void OnGameStarted(PvpArenaStartGameEvent eventData)
        {
            Initialize();
        }

        private void Initialize()
        {
            if (_isInitialized)
                return;

            _isInitialized = true;

            if (_audio != null)
                AudioManager.Instance.PlaySound(_audio, autoPausable: true, loop: true, fadeTime: 0.5f);

            if (_additionalAudio != null)
                AudioManager.Instance.PlaySound(_additionalAudio, autoPausable: true, loop: true, fadeTime: 0.5f);
            switch (_torchType)
            {
                case TorchHandingType.Stand:
                    break;
                case TorchHandingType.Flying:
                    var path = Paths.ResourcesPrefabsTorchesPath + "hand";
                    var prefab = Resources.Load<GameObject>(path);

                    Instantiate(prefab, transform.parent, false);
                    break;
            }

        }


        public void UseTorch()
        {
            OnTorchUse(new MineSceneTorchUseEvent(true));
        }

        public void EndUseTorch()
        {
            OnTorchUse(new MineSceneTorchUseEvent(false));
        }

        public void StartMove()
        {
            OnHeroChangeMoveState(new MineSceneHeroMoveEvent(true));
        }

        public void EndMove()
        {
            OnHeroChangeMoveState(new MineSceneHeroMoveEvent(false));
        }

    }
}