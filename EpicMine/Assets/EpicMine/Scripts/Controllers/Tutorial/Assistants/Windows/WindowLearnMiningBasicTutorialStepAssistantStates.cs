using System.Linq;
using BlackTemple.Common;
using DG.Tweening;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public partial class WindowLearnMiningBasicTutorialStepAssistant
    {
        private abstract class State
        {
            public bool IsComplete { get; protected set; }

            protected readonly WindowLearnMiningBasicTutorialStepAssistant _window;
            protected State _nextState;

            protected State(WindowLearnMiningBasicTutorialStepAssistant window)
            {
                _window = window;
            }

            public virtual void OnEnter() { }
            public virtual void OnUpdate() { }
            public virtual void GoToNextState() { }
            public virtual void OnExit() { }

            public void SetNextState(State state)
            {
                _nextState = state;
            }
        }


        private class AttackPointsPopupState : State
        {
            private float _delay = 1f;
            private GameObject _attackPoint;
            private GameObject[] _attackPoints;
            private int _attackPointPreviousLayer;
            private Vector2 _attackPointAnchoredPosition;


            public AttackPointsPopupState(WindowLearnMiningBasicTutorialStepAssistant window) : base(window) { }


            public override void OnEnter()
            {
                base.OnEnter();
                _window._background.raycastTarget = true;
            }

            public override void OnUpdate()
            {
                base.OnUpdate();

                if (IsComplete)
                    return;

                if (_delay > 0)
                {
                    _delay -= Time.deltaTime;
                    return;
                }

                /*  if (_attackPoint == null)
                  {
                      _attackPoints = GameObject.FindGameObjectsWithTag(Tags.AttackPoint);
                      if (_attackPoints.Length > 0)
                      {
                          GameObject point = null;
                          var minX = float.MaxValue;

                          foreach (var p in _attackPoints)
                          {
                              if (p.transform.position.x < minX)
                              {
                                  point = p;
                                  minX = p.transform.position.x;
                              }
                          }

                          if (point != null)
                              _attackPoint = point;
                      }
                      return;
                  }

                  _attackPointPreviousLayer = _attackPoint.layer;
                  
                foreach (var p in _attackPoints)
                      p.transform.ChangeLayerRecursive(Layers.TutorialHighlightLayer);
                      */
                 
                  _window._background.DOFade(0.6f, 0f).SetUpdate(true);
                  TimeManager.Instance.SetPause(true);

                 /* var viewportPosition = Camera.main
                      .WorldToViewportPoint(_attackPoint.transform.position);

                  var anchoredPosition = new Vector2(
                      viewportPosition.x * _window._rootRectTransform.sizeDelta.x,
                      viewportPosition.y * _window._rootRectTransform.sizeDelta.y);

                  _attackPointAnchoredPosition = anchoredPosition;
                  anchoredPosition.x -= 75f;*/

                _window._popups[0].localPosition = Vector3.zero;
                _window._popups[0].gameObject.SetActive(true);

                IsComplete = true;
            }

            public override void GoToNextState()
            {
                base.GoToNextState();

                /* var nextState = new AttackLinePopupState(_attackPoint, _attackPointAnchoredPosition, _window);
                 _window.SetState(nextState);*/

                var nextState = new MiningResourcesPopupState(_window);
                _window.SetState(nextState);
            }

            public override void OnExit()
            {
                base.OnExit();

                _window._background.raycastTarget = false;

                _window._background.DOFade(0f, 0f).SetUpdate(true);
                _window._popups[0].gameObject.SetActive(false);

               /* foreach (var p in _attackPoints)
                    p.transform.ChangeLayerRecursive(_attackPointPreviousLayer);*/

                TimeManager.Instance.SetPause(false);
            }
        }

        private class MiningResourcesPopupState : State
        {
            private float _delay = 0.5f;
            private GameObject _resourceIcon;

            public MiningResourcesPopupState(WindowLearnMiningBasicTutorialStepAssistant window) : base(window) { }

            public override void OnEnter()
            {
                base.OnEnter();

            //    _window._background.raycastTarget = true;
            }

            public override void OnUpdate()
            {
                base.OnUpdate();

                if (IsComplete)
                    return;

                if (_resourceIcon == null)
                {
                    _resourceIcon = GameObject.FindWithTag(Tags.WindowFlyingIconsItem);
                    return;
                }

                if (_delay > 0)
                {
                    _delay -= Time.deltaTime;
                    return;
                }


                _window._background.raycastTarget = true;
                _window._background.DOFade(0.6f, 0f).SetUpdate(true);
                TimeManager.Instance.SetPause(true);

                Camera worldCamera = null;

                var uiCameraGo = GameObject.FindWithTag(Tags.UICamera);
                if (uiCameraGo != null)
                    worldCamera = uiCameraGo.GetComponent<Camera>();

                if (worldCamera == null)
                    worldCamera = Camera.main;

                var viewportPosition = worldCamera.WorldToViewportPoint(_resourceIcon.transform.position);

                var anchoredPosition = new Vector2(
                    viewportPosition.x * _window._rootRectTransform.sizeDelta.x,
                    viewportPosition.y * _window._rootRectTransform.sizeDelta.y);

                anchoredPosition.x -= 75f;

                _window._popups[1].anchoredPosition = anchoredPosition;
                _window._popups[1].gameObject.SetActive(true);

                IsComplete = true;
            }

            public override void GoToNextState()
            {
                base.GoToNextState();
                _window.SetState(_nextState);
            }

            public override void OnExit()
            {
                base.OnExit();

                _window._background.raycastTarget = false;
                _window._background.DOFade(0f, 0f).SetUpdate(true);
                _window._popups[1].gameObject.SetActive(false);
                TimeManager.Instance.SetPause(false);
            }
        }
        /*
        private class AttackLinePopupState : State
        {
            private readonly Vector2 _attackPointAnchoredPosition;
            private readonly GameObject _attackPoint;
            private MineSceneAttackLine _attackLine;
            private int _attackPointPreviousLayer;
            private int _attackLinePreviousLayer;
            private bool _isAttackLineReachedPoint;


            public AttackLinePopupState(GameObject attackPoint, Vector2 attackPointAnchoredPosition, WindowLearnMiningBasicTutorialStepAssistant window) : base(window)
            {
                _attackPoint = attackPoint;
                _attackPointAnchoredPosition = attackPointAnchoredPosition;
            }

            public override void OnEnter()
            {
                base.OnEnter();
                _window._background.raycastTarget = true;

            }

            public override void OnUpdate()
            {
                base.OnUpdate();


                if (IsComplete)
                    return;


                if (_attackLine == null)
                {
                    var attackLines = FindObjectsOfType<MineSceneAttackLine>();
                    _attackLine = attackLines.First(x => x.gameObject.activeSelf);
                    return;
                }

                if (!_isAttackLineReachedPoint)
                {
                    var viewportPosition = Camera.main.WorldToViewportPoint(_attackLine.transform.position);
                    var anchoredPosition = new Vector2(viewportPosition.x * _window._rootRectTransform.sizeDelta.x, viewportPosition.y * _window._rootRectTransform.sizeDelta.y);

                    if(_attackLine.IsHorizontal) _isAttackLineReachedPoint = Mathf.Abs(anchoredPosition.x - _attackPointAnchoredPosition.x) < 20f;
                        else _isAttackLineReachedPoint = Mathf.Abs(anchoredPosition.y - _attackPointAnchoredPosition.y) < 20f;
                    return;
                }

                if (_attackLine.IsHorizontal)
                    _attackLine.transform.position = new Vector3(_attackPoint.transform.position.x, _attackLine.transform.position.y, _attackLine.transform.position.z);
                else _attackLine.transform.position = new Vector3(_attackLine.transform.position.x, _attackPoint.transform.position.y, _attackLine.transform.position.z);

                _attackLinePreviousLayer = _attackLine.gameObject.layer;
                _attackPointPreviousLayer = _attackPoint.layer;

                _attackLine.transform.ChangeLayerRecursive(Layers.TutorialHighlightLayer);
                _attackPoint.transform.ChangeLayerRecursive(Layers.TutorialHighlightLayer);

                _window._background.DOFade(0.6f, 0f).SetUpdate(true);
                _window._popups[1].anchoredPosition = _window._popups[0].anchoredPosition;

                TimeManager.Instance.SetPause(true);
                _window._popups[1].gameObject.SetActive(true);

                IsComplete = true;
            }

            public override void GoToNextState()
            {
                base.GoToNextState();

                var wallSections = FindObjectsOfType<MineSceneAttackSection>();
                wallSections?.FirstOrDefault(s => s.IsReady)?.AttackArea.OnClick(true);

                var nextState = new MiningResourcesPopupState(_window);
                _window.SetState(nextState);
            }

            public override void OnExit()
            {
                base.OnExit();
                _window._background.raycastTarget = false;

                _window._background.DOFade(0f, 0f).SetUpdate(true);
                _window._popups[1].gameObject.SetActive(false);

                _attackLine.transform.ChangeLayerRecursive(_attackLinePreviousLayer);
                _attackPoint.transform.ChangeLayerRecursive(_attackPointPreviousLayer);

                TimeManager.Instance.SetPause(false);
            }
        }


   

        /*  private class WallHealthBarPopupState : State
          {
              private float _delay = 1f;
              private float _showingTime = 3f;

              public WallHealthBarPopupState(WindowLearnMiningBasicTutorialStepAssistant window) : base(window) { }

              public override void OnUpdate()
              {
                  base.OnUpdate();

                  if (Input.GetMouseButtonDown(0))
                  {
                      GoToNextState();
                      return;
                  }

                  if (_delay > 0)
                  {
                      _delay -= Time.deltaTime;
                      return;
                  }

                  if (IsComplete)
                  {
                      if (_showingTime > 0)
                      {
                          _showingTime -= Time.deltaTime;
                          return;
                      }

                      GoToNextState();
                      return;
                  }

                  _window._popups[1].gameObject.SetActive(true);
                  IsComplete = true;
              }

              public override void GoToNextState()
              {
                  base.GoToNextState();
                  _window.SetState(null);
              }

              public override void OnExit()
              {
                  base.OnExit();
                  _window._popups[1].gameObject.SetActive(false);
              }
          }

        private class PickaxeHealthBarPopupState : State
          {
              public PickaxeHealthBarPopupState(WindowLearnMiningBasicTutorialStepAssistant window) : base(window) { }

              public override void OnEnter()
              {
                  base.OnEnter();

                  _window._background.raycastTarget = true;
                  _window._background.DOFade(0.6f, 0f).SetUpdate(true);
                  _window._popups[3].gameObject.SetActive(true);
                  TimeManager.Instance.SetPause(true);
              }

              public override void OnUpdate()
              {
                  base.OnUpdate();

                  if (IsComplete)
                      return;

                  IsComplete = true;
              }

              public override void GoToNextState()
              {
                  base.GoToNextState();
                  if (_window._currentState == this)
                      _window.SetState(null);
              }

              public override void OnExit()
              {
                  base.OnExit();

                  _window._background.raycastTarget = false;
                  _window._background.DOFade(0f, 0f).SetUpdate(true);
                  _window._popups[3].gameObject.SetActive(false);
                  TimeManager.Instance.SetPause(false);
              }
          }


      */
    }
}