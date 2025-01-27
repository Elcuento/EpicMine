using System;
using System.Collections.Generic;
using System.Linq;
using BlackTemple.EpicMine;
using BlackTemple.EpicMine.Dto;
using CommonDLL.Static;
using UnityEngine;
using Random = UnityEngine.Random;


public class EntrySceneDeveloperPanel : MonoBehaviour {

    private DeveloperController _controller;


    public void Initialize(DeveloperController developerController)
    {
        _controller = developerController;
        Fill();
    }


    public void Fill()
    {
        _controller.CreateVerticalScroll(null, new List<DeveloperControlPanelItem>
        {
            _controller.AddButton((x) =>
            {
                App.Instance.Restart();
            },"Restart"),
            _controller.AddToggle((x) =>
            {
                var controller = FindObjectOfType<EntryPointSceneController>();
                PlayerPrefsHelper.Save(PlayerPrefsType.PreloadQuality, x ? 1 : 0);
            },"Load async ", defaultVal : true)

        });

        _controller.Rebuild();
    }


}
