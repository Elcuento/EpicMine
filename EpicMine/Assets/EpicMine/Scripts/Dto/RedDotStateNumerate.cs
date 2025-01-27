public class RedDotStateNumerate : RedDotState
{
    public int Number;

    public RedDotStateNumerate(string valName, bool state, bool view, int number) : base(valName,state,view)
    {
        ValueName = valName;
        State = state;
        Viewed = view;
        Number = number;
    }

    public RedDotStateNumerate()
    {
        ValueName = "";
    }

    public void SetNumber(int number)
    {
        Number = number;
    }
}
