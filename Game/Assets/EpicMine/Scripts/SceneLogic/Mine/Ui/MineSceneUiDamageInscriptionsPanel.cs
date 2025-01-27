using System.Collections.Generic;
using BlackTemple.Common;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class MineSceneUiDamageInscriptionsPanel : MonoBehaviour
    {
        [SerializeField] private Camera _camera;

        [SerializeField] private RectTransform _rootRectTransform;

        [SerializeField] private MineSceneUiDamageInscriptionItem _damageInscriptionPrefab;

        private bool _isCenterDamageAppear;

        private List<MineSceneUiDamageInscriptionItem> _inscriptionPool = new List<MineSceneUiDamageInscriptionItem>();

        private void Awake()
        {
            EventManager.Instance.Subscribe<MineSceneWallSectionDamageEvent>(OnWallDamage);
        }

        private void OnDestroy()
        {
            if (EventManager.Instance != null)
            {
                EventManager.Instance.Unsubscribe<MineSceneWallSectionDamageEvent>(OnWallDamage);
            }
        }

        private void OnInscriptionEnd(MineSceneUiDamageInscriptionItem item)
        {
            if(!_inscriptionPool.Contains(item))
            _inscriptionPool.Add(item);
        }

        private MineSceneUiDamageInscriptionItem GetItem()
        {
            if (_inscriptionPool.Count > 0)
            {
                var ins = _inscriptionPool[0];
                _inscriptionPool.Remove(ins);
                return ins;
            }

            return null;
        }

        private void OnWallDamage(MineSceneWallSectionDamageEvent eventData)
        {
            var inscription = GetItem() ?? Instantiate(_damageInscriptionPrefab, transform, false);


            if (eventData.AttackPoint != null)
            {
                var  targetPosition = eventData.AttackPoint.gameObject.transform.position;
                var position = _camera.WorldToViewportPoint(targetPosition);

                position.x *= _rootRectTransform.sizeDelta.x;
                position.y *= _rootRectTransform.sizeDelta.y;
                position.z = 0f;

                inscription.Initialize(eventData, position, OnInscriptionEnd);
            }
            else
            {
                var  targetPosition = _camera.transform.position + Vector3.forward;
                var position = _camera.WorldToViewportPoint(targetPosition);

                position.x *= _rootRectTransform.sizeDelta.x ;
                position.y *= _rootRectTransform.sizeDelta.y - (_isCenterDamageAppear ? 200 : 0);
                position.z = 0f;

                _isCenterDamageAppear = true;
                inscription.Initialize(eventData, position, OnInscriptionEnd);
                Invoke("CentreDamageDisappear", 1);
            }         
        }

        private void CentreDamageDisappear()
        {
            _isCenterDamageAppear = false;
        }

    }
}