public class RedDotState : RedDotSimple
{
    public bool State;

    public RedDotState(string valName, bool state, bool view) : base (valName, view)
    {
        ValueName = valName;
        State = state;
        Viewed = view;
    }

    public RedDotState()
    {
        ValueName = "";
    }

    public void SetState(bool state)
    {
        State = state;
    }
}
