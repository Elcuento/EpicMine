using System;
using CommonDLL.Static;

namespace CommonDLL.Dto
{
  public class PvpArenaUserInfo
  {
      public string Id;
      public string Name;
      public string Pickaxe;
      public string Torch;
      public string Localate;
      public int Rating;
      public bool IsBot;
      public int DonateRang;
      public int Damage;
      public int Walls;
      public bool Ready;
      public int Chest;
      public bool Leaved;

      public PvpArenaUserInfo()
      {

      }

      public PvpArenaUserInfo(Player data)
      {
          Id = data.Id;
          Name = data.Nickname;
          Pickaxe = data.Blacksmith.SelectedPickaxe;
          Torch = data.TorchesMerchant.SelectedTorch;
          Rating = data.Pvp.Rating;
      }

      public void SetProperty(PvpArenaUserPropertyType propType, object prop)
        {
            switch (propType)
            {
                case PvpArenaUserPropertyType.Name:
                    Name = (string)prop;
                    break;
                case PvpArenaUserPropertyType.Id:
                    Id = (string)prop;
                    break;
                case PvpArenaUserPropertyType.Pickaxe:
                    Pickaxe = (string)prop;
                    break;
                case PvpArenaUserPropertyType.Torch:
                    Torch = (string)prop;
                    break;
                case PvpArenaUserPropertyType.Localate:
                    Localate = (string)prop;
                    break;
                case PvpArenaUserPropertyType.Rating:
                    Rating = (int)prop;
                    break;
                case PvpArenaUserPropertyType.TorchDonateRang:
                    DonateRang = (int)prop;
                    break;
                case PvpArenaUserPropertyType.Damage:
                    Damage = (int)prop;
                    break;
                case PvpArenaUserPropertyType.Wall:
                    Walls = (int)prop;
                    break;
                case PvpArenaUserPropertyType.Ready:
                    Ready = (bool)prop;
                    break;
                case PvpArenaUserPropertyType.Chest:
                    Chest = (int)prop;
                    break;
                case PvpArenaUserPropertyType.IsBot:
                    IsBot = (bool)prop;
                    break;
            }
        }
        public object GetProperty<T>(PvpArenaUserPropertyType propType)
        {
            switch (propType)
            {
                case PvpArenaUserPropertyType.Name:
                    return Name;
                case PvpArenaUserPropertyType.Id:
                    return Id;
                case PvpArenaUserPropertyType.Pickaxe:
                    return Pickaxe;
                case PvpArenaUserPropertyType.League:
                    return Pickaxe;
                case PvpArenaUserPropertyType.Localate:
                    return Localate;
                case PvpArenaUserPropertyType.Torch:
                    return Torch;
                case PvpArenaUserPropertyType.Rating:
                    return Rating;
                case PvpArenaUserPropertyType.TorchDonateRang:
                    return DonateRang;
                case PvpArenaUserPropertyType.Damage:
                    return Damage;
                case PvpArenaUserPropertyType.Wall:
                    return Walls;
                case PvpArenaUserPropertyType.Ready:
                    return Ready;
                case PvpArenaUserPropertyType.Chest:
                    return Chest;
                case PvpArenaUserPropertyType.IsBot:
                    return IsBot;
                default:
                    Console.WriteLine("Pvp arena prop not exist " + propType);
                    return null;
            }
        }
    }
}
