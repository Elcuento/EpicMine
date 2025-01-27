using BlackTemple.Common;
using BlackTemple.EpicMine;
using CommonDLL.Static;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class VillageSceneMine : MonoBehaviour
{
    [SerializeField] private GameObject _header;
    [SerializeField] private GameObject _questArrow;

    private void Start()
    {
        if (!App.Instance.Player.AutoMiner.IsOpen)
        {
            _header.SetActive(true);
            EventManager.Instance.Subscribe<AutoMinerOpenEvent>(OnAutoMinerOpen);
        }
        else _header.SetActive(false);

        if (App.Instance.Controllers.TutorialController.IsStepComplete(TutorialStepIds.ShowDailyTasks))
        {
            CheckQuests();
            EventManager.Instance.Subscribe<QuestUpdateEvent>(OnQuestUpdate);
        }
    }

    private void OnQuestUpdate(QuestUpdateEvent eventData)
    {
        CheckQuests();
    }

    private void CheckQuests()
    {
        var isActive = QuestHelper.GetAnyOneMineQuest();

        if (isActive)
        {
            _questArrow.transform.DOLocalMoveY(_questArrow.transform.localPosition.y + 25f, 2)
                .SetLoops(-1, LoopType.Yoyo);
        }

        _questArrow.SetActive(_questArrow);
    }

    private void OnDestroy()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.Unsubscribe<AutoMinerOpenEvent>(OnAutoMinerOpen);
        EventManager.Instance.Unsubscribe<QuestUpdateEvent>(OnQuestUpdate);
    }
   /* protected override void Subscribe()
    {
        base.Subscribe();

        if (!App.Instance.Player.AutoMiner.IsOpen)
        {
            _header.SetActive(true);
            EventManager.Instance.Subscribe<AutoMinerOpenEvent>(OnAutoMinerOpen);
        }else _header.SetActive(false);
    }

    protected override void UnSubscribe()
    {
        base.UnSubscribe();

        EventManager.Instance.Unsubscribe<AutoMinerOpenEvent>(OnAutoMinerOpen);
    }*/

    private void OnAutoMinerOpen(AutoMinerOpenEvent data)
    {
        _header.SetActive(false);
    }
}
