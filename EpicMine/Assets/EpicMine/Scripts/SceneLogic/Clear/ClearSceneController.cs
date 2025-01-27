using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public class ClearSceneController : MonoBehaviour
    {
        private readonly List<string> _whiteList = new List<string>
        {
            "Firebase Services",
            "[DOTween]",
            "UnityFacebookSDKPlugin",
            "DragonBones Object",
            "PhotonMono",
            "UnnyNet"
        };

        private readonly List<Type> _whiteTypeList = new List<Type>
        {
           
        };

        private void Start()
        {
            var gameObjects = FindObjectsOfType(typeof(GameObject));
            foreach (var go in gameObjects)
            {
                if (go != gameObject &&
                    !_whiteList.Contains(go.name))
                {
                    var gameOb = go as GameObject;
                    var inWhiteList = false;

                    if (gameOb != null)
                    {
                        foreach (var type in _whiteTypeList)
                        {
                            var comp = gameOb.GetComponents(type);
                            if (comp.Length > 0)
                            {
                                inWhiteList = true;
                                break;
                            }
                        }
                    }

                    if(!inWhiteList)
                    Destroy(go);
                }
            }

            GC.Collect();
            UnityEngine.SceneManagement.SceneManager.LoadScene(ScenesNames.EntryPoint);
            
        }
    }
}