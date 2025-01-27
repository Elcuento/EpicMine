using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.Callbacks;
using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;

public class PostBuildProcess : IPostprocessBuildWithReport
{
    [PostProcessBuild]
    public static void ChangeXCodePList(BuildTarget buildTarget, string pathToBuiltProject)
    {
        if (buildTarget == BuildTarget.iOS)
        {
        const string adMobId = "ca-app-pub-6304136201707366~3001070925";
        const string bluetoothInfo = "Allow Epic Mine send bluetooth messages";
        const string trackingDesc = "Your data will only be used to deliver personalized ads to you.";

            var pListPath = pathToBuiltProject + "/Info.plist";
            var pLsit = new PlistDocument();
            pLsit.ReadFromFile(pListPath);
            var root = pLsit.root;

            root.SetString("GADApplicationIdentifier", adMobId);
            root.SetString("NSBluetoothAlwaysUsageDescription", bluetoothInfo);
            root.SetString("NSUserTrackingUsageDescription", trackingDesc);

            AddUtlTypes(root);

            File.WriteAllText(pListPath, pLsit.WriteToString());   

            Debug.Log("PosBuildProcess added admob and bluetooth disc info to pList");
        }
    }

    private static void AddUtlTypes(PlistElementDict dic)
    {
        PlistElementDict rootDict = dic;
        PlistElementArray urlTypesArray = null;
        PlistElementDict urlTypesItems = null;
        PlistElementArray urlSchemesArray = null;

        if (!dic.values.ContainsKey("CFBundleURLTypes"))
        {
            urlTypesArray = rootDict.CreateArray("CFBundleURLTypes");
        }
        else
        {
            urlTypesArray = rootDict.values["CFBundleURLTypes"].AsArray();

            if (urlTypesArray == null)
            {
                urlTypesArray = rootDict.CreateArray("CFBundleURLTypes");
            }
        }

        if (urlTypesArray.values.Count == 0)
        {
            urlTypesItems = urlTypesArray.AddDict();
        }
        else
        {
            urlTypesItems = urlTypesArray.values[0].AsDict();

            if (urlTypesItems == null)
            {
                urlTypesItems = urlTypesArray.AddDict();
            }
        }

        if (!urlTypesItems.values.ContainsKey("CFBundleURLSchemes"))
        {
            urlSchemesArray = urlTypesItems.CreateArray("CFBundleURLSchemes");
        }
        else
        {
            urlSchemesArray = urlTypesItems.values["CFBundleURLSchemes"].AsArray();

            if (urlSchemesArray == null)
            {
                urlSchemesArray = urlTypesItems.CreateArray("CFBundleURLSchemes");
            }
        }

        var url = "com.googleusercontent.apps.312687649229-75tusrn25l5ics0gtrl9671os1udqvde";

        foreach (PlistElement elem in urlSchemesArray.values)
        {
            if (elem.AsString() != null && elem.AsString().Equals(url))
            {
                urlSchemesArray.values.Remove(elem);
                break;
            }
        }

        foreach (PlistElement elem in urlSchemesArray.values)
        {
            if (elem.AsString() != null && elem.AsString().Equals(url))
            {
                urlSchemesArray.values.Remove(elem);
                break;
            }
        }

        urlSchemesArray.AddString(url);
    }


    public int callbackOrder { get; }
    public void OnPostprocessBuild(BuildReport report)
    {
    }
}
#endif