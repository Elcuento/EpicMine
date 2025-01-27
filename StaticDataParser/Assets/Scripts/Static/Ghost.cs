using System;
using System.Collections.Generic;

namespace BlackTemple.EpicMine.Static
{
    public class Ghost
    {
        public CharacterType Id { get; }

        public int Tier { get; }

        public string Dialogues { get; }

        public List<int> Actions{ get; }

        public Ghost(CharacterType id, int tier, string actions, string dialogues)
        {
            //dialogues.Split('#') : new string[]
            Id = id;
            Tier = tier;
            Dialogues = dialogues;
            Actions = !string.IsNullOrEmpty(actions) ? new List<int>(Array.ConvertAll(actions.Split('#'), int.Parse)) : new List<int>();
        }
    }
}