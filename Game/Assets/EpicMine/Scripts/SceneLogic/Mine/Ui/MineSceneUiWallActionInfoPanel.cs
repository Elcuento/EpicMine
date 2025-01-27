using System;
using System.Collections;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using CommonDLL.Static;
using UnityEngine;
using UnityEngine.UI;

public class MineSceneUiWallActionInfoPanel : MonoBehaviour {

    [SerializeField] private Slider _wallSpecialActionBar;
    [SerializeField] private Image  _wallSpecialActionBarIcon;

    [Space]
    [SerializeField] private Slider _wallDefaultActionBar;

    private void Awake () {
        EventManager.Instance.Subscribe<MineSceneSectionReadyEvent>(OnSectionReady);
        EventManager.Instance.Subscribe<MineSceneSectionPassedEvent>(OnSectionPassed);
        EventManager.Instance.Subscribe<MineSceneSectionStartActionEvent>(OnSectionStartAction);
        EventManager.Instance.Subscribe<MineSceneSectionEndActionEvent>(OnSectionEndAction);
    }

    private void OnDestroy()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.Unsubscribe<MineSceneSectionReadyEvent>(OnSectionReady);
        EventManager.Instance.Unsubscribe<MineSceneSectionPassedEvent>(OnSectionPassed);
        EventManager.Instance.Unsubscribe<MineSceneSectionStartActionEvent>(OnSectionStartAction);
        EventManager.Instance.Unsubscribe<MineSceneSectionEndActionEvent>(OnSectionEndAction);
    }

    public void Clear()
    {
        StopAllCoroutines();
        _wallDefaultActionBar.value = 0;
        _wallSpecialActionBar.value = 0;
    }

    private void OnSectionReady(MineSceneSectionReadyEvent eventData)
    {
        var monsterSection = eventData.Section as MineSceneMonsterSection;

        if (monsterSection != null)
        {
            Clear();
        }
    }

    private void OnSectionPassed(MineSceneSectionPassedEvent eventData)
    {
        _wallSpecialActionBar.gameObject.SetActive(false);
        _wallDefaultActionBar.gameObject.SetActive(false);
    }

    private void OnSectionStartAction(MineSceneSectionStartActionEvent eventData)
    {
       Clear();

       _wallSpecialActionBar.gameObject.SetActive(false);
       _wallDefaultActionBar.gameObject.SetActive(false);

        switch (eventData.MonsterAction)
       {        
           case MonsterActionType.AbilityDamage:
               _wallSpecialActionBar.gameObject.SetActive(true);
                _wallSpecialActionBarIcon.sprite = App.Instance.ReferencesTables.Sprites.AbilityDamageIcon;
               StartCoroutine(FillActionBar(eventData.Time, _wallSpecialActionBar));
                break;
           case MonsterActionType.AbilityHeal:
               _wallSpecialActionBar.gameObject.SetActive(true);
                _wallSpecialActionBarIcon.sprite = App.Instance.ReferencesTables.Sprites.HealingBuffIcon;
               StartCoroutine(FillActionBar(eventData.Time, _wallSpecialActionBar));
                break;
           case MonsterActionType.Damage:
               _wallDefaultActionBar.gameObject.SetActive(true);
               StartCoroutine(FillActionBar(eventData.Time, _wallDefaultActionBar));
                break;
           default:
               throw new ArgumentOutOfRangeException();
       }
      
    }

    private void OnSectionEndAction(MineSceneSectionEndActionEvent eventData)
    {
       Clear();
    }

    public IEnumerator FillActionBar(float timeMax, Slider slider)
    {
        var time = 0f;

        while (time < timeMax)
        {
            time += Time.deltaTime;
            slider.value = time / timeMax;
            yield return null;
        }

        slider.gameObject.SetActive(false);
        slider.value = 0;
    }

}
