using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorReferencesTable")]
public class ColorReferencesTable : ScriptableObject {


    [Header("Pvp")]
    public Color[] LeagueHeaderColors;
    public Color[] LeagueTextColors;
    public Color[] LeagueBackGroundColors;

    [Header("Walls")]
    public Color[] LeagueWallNumberColors;


    [Header("Damage Source")]
    public Color FireTextColor;
    public Color AcidTextColor;
    public Color FrostTextColor;
    public Color DefaultTextColor;


}
