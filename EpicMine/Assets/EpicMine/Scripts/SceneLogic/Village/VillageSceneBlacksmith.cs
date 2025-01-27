using System.Collections;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using UnityEngine;

public class VillageSceneBlacksmith : VillageSceneCharacter
{
    protected override void Start()
    {
        StartCoroutine(AnimationCycle());

        _controller.RegisterVillageQuestCharacter(this, _questArrow);
        CheckQuest();
    }

    protected override IEnumerator AnimationCycle()
    {
        while (true)
        {
            _armature.animation.FadeIn(_idleAnimationName, 0.1f);

            var randomTime = Random.Range(5f, 10f);
            yield return new WaitForSecondsRealtime(randomTime);

            var randomAnimationIndex = Random.Range(0, _randomAnimationNames.Count);
            var randomAnimationName = _randomAnimationNames[randomAnimationIndex];

            if (randomAnimationName == "Kovka")
                Invoke("PlayHammer",2.068f);
            

            _armature.animation.FadeIn(randomAnimationName, 0.1f, 1);
            yield return new WaitForSecondsRealtime(_armature.animation.animations[randomAnimationName].duration);
        }
    }

    private void PlayHammer()
    {
        AudioManager.Instance.PlaySound(App.Instance.ReferencesTables.Sounds.BlacksmithCharacterHammer);
    }
}
