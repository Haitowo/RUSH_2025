using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.Rush.UI
{
    
    public class TileEntryRuntime
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        public GameObject prefab;
        public int remaining;

        public TileEntryRuntime(TileDefinition pDef)
        {
            prefab = pDef.tilesToPlace;
            remaining = pDef.amountToPlace;
        }

        public void UseOne()
        {
            remaining = Mathf.Max(remaining - 1, 0);
        }

        public void ResetOne()
        {
            remaining = Mathf.Max(remaining++, 0);
        }
    }
}