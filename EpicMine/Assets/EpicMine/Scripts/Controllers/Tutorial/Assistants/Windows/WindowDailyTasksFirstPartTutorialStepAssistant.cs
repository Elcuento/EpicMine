using BlackTemple.EpicMine;

public class WindowDailyTasksFirstPartTutorialStepAssistant: WindowBase
{
    public void OnClickOpenTasks()
    {
        OpenDailyTasks();
    }

    private void OpenDailyTasks()
    {
        WindowManager.Instance.Show<WindowDailyTasksQuest>()
            .OpenDaily();

        WindowManager.Instance.Show<WindowDailyTasksSecondPartTutorialStepAssistant>();

        Close();
    }

}
