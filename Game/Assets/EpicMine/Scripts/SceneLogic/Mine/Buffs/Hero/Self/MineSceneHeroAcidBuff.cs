using System.Collections;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using CommonDLL.Static;
using UnityEngine;

public class MineSceneHeroAcidBuff : MineSceneHeroBuff {

    public int TimeLeft { get; private set; }

    public int Stuck { get; private set; }

    public AbilityLevel Acid { get; private set; }

    public void Start()
    {
        EventManager.Instance.Subscribe<MineSceneSectionReadyEvent>(OnSectionReady);
    }

    public void OnSectionReady(MineSceneSectionReadyEvent eventData)
    {
        var isMonsterSection = eventData.Section is MineSceneMonsterSection;

        if (TimeLeft > 0)
        {
            if (isMonsterSection)
            {
                StopAllCoroutines();
                _hero.RemoveBuff(this);
            }
            else
            {
                eventData.Section.AddBuff(AbilityType.Acid);
            }
        }
    }

    public override void Initialize(MineSceneHero hero)
    {
        base.Initialize(hero);

        Acid = App.Instance.Player.Abilities.Acid.StaticLevel;

        if (Acid == null)
            return;

        if (TimeLeft <= 0)
        {
            TimeLeft = Acid.Duration;
            Stuck = 1;
        }
        else
        {
            Stuck++;
        }

        StartCoroutine(Timer());
    }

    public override void Clear()
    {
        base.Clear();
        TimeLeft = 0;
        Stuck = 0;
    }

    public void UpdateTime()
    {
        TimeLeft = Acid.Duration;
        Stuck++;
    }


    private IEnumerator Timer()
    {
        while (TimeLeft > 0)
        {
            TimeLeft--;
            yield return new WaitForSeconds(1);
        }

        _hero.RemoveBuff(this);
    }

    private void OnDestroy()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.Unsubscribe<MineSceneSectionReadyEvent>(OnSectionReady);
    }

}
