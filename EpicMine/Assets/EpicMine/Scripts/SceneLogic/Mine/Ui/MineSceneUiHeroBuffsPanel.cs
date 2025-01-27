using System.Collections.Generic;
using System.Linq;
using BlackTemple.Common;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class MineSceneUiHeroBuffsPanel : MonoBehaviour
    {
        [SerializeField] private MineSceneHero _hero;

        [SerializeField] private MineSceneUiHeroBuffItem _buffItemPrefab;

        [SerializeField] private Transform _buffsContainer;

        private List<MineSceneUiHeroBuffItem> _buffItems;


        private void Awake()
        {
            _buffItems = new List<MineSceneUiHeroBuffItem>();
            EventManager.Instance.Subscribe<MineSceneHeroBuffsChangeEvent>(OnHeroBuffsChange);
        }

        private void OnDestroy()
        {
            if (EventManager.Instance != null)
                EventManager.Instance.Unsubscribe<MineSceneHeroBuffsChangeEvent>(OnHeroBuffsChange);
        }


        private void OnHeroBuffsChange(MineSceneHeroBuffsChangeEvent data)
        {
            UpdateBuffs();
        }

        private void UpdateBuffs()
        {
            foreach (var buffIcon in _buffItems.ToList())
            {
                if (!_hero.Buffs.Contains(buffIcon.Buff))
                {
                    _buffItems.Remove(buffIcon);
                    Destroy(buffIcon.gameObject);
                }
            }

            foreach (var buff in _hero.Buffs)
            {
                var existBuffIcon = _buffItems.FirstOrDefault(b => b.Buff == buff);
                if (existBuffIcon != null)
                    existBuffIcon.Initialize(buff);
                else
                {
                    var buffItem = Instantiate(_buffItemPrefab, _buffsContainer, false);
                    buffItem.Initialize(buff);
                    _buffItems.Add(buffItem);
                }
            }
        }

        private void ClearBuffIcons()
        {
            _buffsContainer.ClearChildObjects();
            _buffItems.Clear();
        }
    }
}