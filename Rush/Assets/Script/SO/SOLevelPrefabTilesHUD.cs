using Com.IsartDigital.Rush.Utilities;
using System.Collections.Generic;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.Rush.UI
{
    [CreateAssetMenu(fileName = Utils.UI_LEVELSELECT_PRESET, menuName = Utils.UI_LEVELSELECT_MENU)]
    public class SOLevelPrefabTilesHUD : ScriptableObject
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [SerializeField] public List<TileDefinition> tilesToPlaceForLevel = new List<TileDefinition>();
    }
}