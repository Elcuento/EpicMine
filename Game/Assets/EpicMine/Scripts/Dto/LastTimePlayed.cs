using Newtonsoft.Json;

namespace BlackTemple.EpicMine.Dto
{
    public class LastTimePlayed
    {
        [JsonProperty("playerName")]
        public string PlayerName;

        public LastTimePlayed( string playerName)
        {
            PlayerName = playerName;
        }
    }
}