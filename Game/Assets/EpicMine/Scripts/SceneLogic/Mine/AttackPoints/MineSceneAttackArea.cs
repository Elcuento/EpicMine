using System;
using BlackTemple.Common;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BlackTemple.EpicMine
{
    public class MineSceneAttackArea : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        public Action OnHit;

        public MineSceneSection Section { get; protected set; }

        [SerializeField] protected Color _missVignetteColor;

        protected bool _isStarted;

        protected float _animCooldown;

        private bool _isPressed;


        protected virtual void Start()
        {
            Subscribe();
        }

        protected void Subscribe()
        {
          //  EventManager.Instance.Subscribe<MineSceneTorchUseEvent>(OnTorchUse);
        }

        protected void UnSubscribe()
        {
            if (EventManager.Instance == null)
                return;

           // EventManager.Instance.Unsubscribe<MineSceneTorchUseEvent>(OnTorchUse);
        }


        public void OnDestroy()
        {
            if (EventManager.Instance == null)
                return;
        }

        public virtual void FillArea()
        {
        }


        protected virtual  void ResetField()
        {

        }


        public virtual void Clear()
        {
            _isStarted = false;
            _isPressed = false;
            Section = null;
        }

        protected virtual void Update()
        {
            if(_isPressed)
            OnClick();

            if(_animCooldown > 0)
                _animCooldown -= Time.deltaTime;
        }

        public virtual void OnClick(bool force = false)
        {
            if (!force)
            {
                if (!Section.Hero.Pickaxe.IsReadyClick || _animCooldown > 0)
                    return;

                _animCooldown = MineLocalConfigs.PickaxePressedCooldown;
            }
       

            OnHit?.Invoke();
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            _isPressed = true; 
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            _animCooldown = 0;
            _isPressed = false;
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            _animCooldown = 0;
            _isPressed = false;
        }

        protected virtual void OnChangeDirection()
        {
           
        }


        public void EditorReset()
        {
            ResetField();
        }

        public virtual void Initialize(MineSceneSection section, bool isHorizontalAttackLine, float moveTime)
        {
            Section = section;
        }

        public virtual void Initialize(MineSceneSection section)
        {
            Section = section;
        }


    }
}