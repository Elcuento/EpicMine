using System;
using System.Globalization;
using BlackTemple.Common;
using BlackTemple.EpicMine;
using CommonDLL.Static;
using DragonBones;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Quest = BlackTemple.EpicMine.Core.Quest;
using QuestTaskGoal = BlackTemple.EpicMine.Core.QuestTaskGoal;
using Transform = UnityEngine.Transform;

public class VillageSceneAutoMiner : MonoBehaviour
{
    [SerializeField] private Transform _container;
    [SerializeField] private DragonBoneButton _button;

    [Header("Button")]
    [SerializeField] private CanvasGroup _bottomButtonAlpha;
    [SerializeField] private Button _bottomButton;
    [SerializeField] private GameObject _bottomButtonIcon;

    [Space]
    [SerializeField] private GameObject _fullWarning;

    [Header("Build process")]
    [SerializeField] private GameObject _buildLes;
    [SerializeField] private TextMeshProUGUI _buildTimer;

    private QuestTaskGoal _minerGoal;

    private GameObject _autoMiner;

    private void Start()
    {
        EventManager.Instance.Subscribe<AutoMinerChangeMinerLevelEvent>(OnAutoMinerChangeMinerLevel);
        EventManager.Instance.Subscribe<AutoMinerOpenEvent>(OnAutoMinerOpen);
        EventManager.Instance.Subscribe<AutoMinerFullEvent>(OnAutoMinerFull);
        EventManager.Instance.Subscribe<SecondsTickEvent>(OnTick);

        Initialize();
    }

    private void OnDestroy()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.Unsubscribe<AutoMinerChangeMinerLevelEvent>(OnAutoMinerChangeMinerLevel);
        EventManager.Instance.Unsubscribe<AutoMinerOpenEvent>(OnAutoMinerOpen);
        EventManager.Instance.Unsubscribe<AutoMinerFullEvent>(OnAutoMinerFull);
        EventManager.Instance.Unsubscribe<SecondsTickEvent>(OnTick);
        EventManager.Instance.Unsubscribe<QuestStartEvent>(OnQuestStart);
    }

    private void OnTick(SecondsTickEvent data)
    {
        if (_minerGoal == null || _minerGoal.IsCompleted)
            return;

        var date = new DateTime();
        var endTime = _minerGoal.StartTime + _minerGoal.StaticGoal.Goal.Value - TimeManager.Instance.NowUnixSeconds;
        endTime = endTime < 0 ? 0 : endTime;
        date = date.AddSeconds(endTime);

        _buildTimer.text = date.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
    }

    private void OnAutoMinerFull(AutoMinerFullEvent eventData)
    {
        _fullWarning.SetActive(true);
    }

    private void OnAutoMinerOpen(AutoMinerOpenEvent eventData)
    {
        Initialize();
    }

    private void OnAutoMinerChangeMinerLevel(AutoMinerChangeMinerLevelEvent eventData)
    {
        Initialize();
    }

    private void OnQuestStart(QuestStartEvent data)
    {

       if(data.Quest.Status == QuestStatusType.Started 
          && data.Quest.StaticQuest.RewardFeatures.Contains(FeaturesType.AutoMiner))
       {
           EventManager.Instance.Unsubscribe<QuestStartEvent>(OnQuestStart);
            Initialize();
       }
    }

    private void Initialize()
    {
        if (!App.Instance.Player.AutoMiner.IsOpen)
        {
   

            var quest = App.Instance.Player.Quests.QuestList.Find(x =>
                x.Status == QuestStatusType.Started && x.StaticQuest.RewardFeatures.Contains(FeaturesType.AutoMiner));

            if (quest != null)
            {
                for (var index = 0; index < quest.TaskList.Count; index++)
                {
                    var questTask = quest.TaskList[index];
                    for (var i = 0; i < questTask.GoalsList.Count; i++)
                    {
                        var taskGoal = questTask.GoalsList[i];
                        if (!taskGoal.IsCompleted && taskGoal.StaticGoal.Type == QuestTaskType.TimeLeft)
                        {
                            _minerGoal = taskGoal;
                            break;
                        }
                    }
                }

                if (_minerGoal != null)
                {
                    _buildLes.SetActive(true);

                    EventManager.Instance.Unsubscribe<SecondsTickEvent>(OnTick);
                    EventManager.Instance.Subscribe<SecondsTickEvent>(OnTick);

                    OnTick(new SecondsTickEvent());
                }
            }
            else
            {
                EventManager.Instance.Unsubscribe<QuestStartEvent>(OnQuestStart);
                EventManager.Instance.Subscribe<QuestStartEvent>(OnQuestStart);
            }

            return;
        }

        _buildLes.SetActive(false);

        _container.ClearChildObjects();

        _bottomButton.enabled = true;
        _bottomButtonAlpha.alpha = 1;
        _bottomButtonIcon.SetActive(true);

        _fullWarning.SetActive(App.Instance.Player.AutoMiner.IsFull);

        _autoMiner = AutoMinerHelper.GetModel(_container);

        _button.SetMaterial(_autoMiner.GetComponentInChildren<UnityUGUIDisplay>().material);
    }

}
