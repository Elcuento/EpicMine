using System.Collections;
using System.Collections.Generic;
using System.IO;
using BlackTemple.Common;
using CommonDLL.Dto;
using UnityEngine;

public class LocalizationCheckerController : MonoBehaviour
{
    public TextAsset EnglishLoc;
    public TextAsset RussianLoc;

    public void Start()
    {
        var russianDic = RussianLoc.text.FromJson<LocalizationData>();
        var englishDic = EnglishLoc.text.FromJson<LocalizationData>();
        var nonExist = "";

        Debug.Log("Russian size " + russianDic.Values.Count);
        Debug.Log("English size " + englishDic.Values.Count);

        var notExistList = new List<LocalizationString>();

        foreach (var localizationString in russianDic.Values)
        {
            if (englishDic.Values.Find(x => x.Key == localizationString.Key) == null)
            {
                notExistList.Add(localizationString);
               // nonExist +=  $"\n{localizationString.Key}";
            }
        }
        

        foreach (var localizationString in notExistList)
        {
            nonExist += $"\n{localizationString.Key}={localizationString.Text}";
        }

        File.WriteAllText(Application.persistentDataPath +"LocalizationDifference.txt", nonExist);
        Debug.Log("done");
    }

    public void Check()
    {

    }

}
