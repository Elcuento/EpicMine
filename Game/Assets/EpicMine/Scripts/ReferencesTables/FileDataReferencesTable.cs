using UnityEngine;

[CreateAssetMenu(fileName = "FileDataReferencesTable")]
public class FileDataReferencesTable : ScriptableObject
{
    public TextAsset FieldData;
    public TextAsset BlockedNames;

    public TextAsset RussianInBuildLocalization;
    public TextAsset EnglishInBuildLocalization;

    public TextAsset FirebaseAndroidLive;
    public TextAsset FirebaseAndroidDev;
}
