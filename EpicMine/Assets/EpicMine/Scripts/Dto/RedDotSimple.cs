public class RedDotSimple
{
    public string ValueName;
    public bool Viewed;

    public RedDotSimple(string valName, bool view)
    {
        ValueName = valName;
        Viewed = view;
    }

    public RedDotSimple()
    {
        ValueName = "";
    }

    public void SetView(bool view)
    {
        Viewed = view;
    }

}
