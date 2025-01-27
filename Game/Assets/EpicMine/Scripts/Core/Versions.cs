
namespace BlackTemple.EpicMine.Core
{
    public class Versions
    {
        public int FixVersion { get; private set; }

        public Versions(CommonDLL.Dto.Versions data)
        {
            FixVersion = data.FixVersion;
        }

        public void SetFix(int version)
        {
            FixVersion = version;
        }
    }
}