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
        public int angleToTurn;
        private int _InitialAmount;

        public TileEntryRuntime(TileDefinition pDef)
        {
            prefab = pDef.tilesToPlace;
            remaining = pDef.amountToPlace;
            angleToTurn = pDef.angle;
            _InitialAmount = pDef.amountToPlace;
        }

        public void UseOne()
        {
            remaining = Mathf.Max(remaining - 1, 0);
        }

        public void ResetAmount()
        {
            remaining = _InitialAmount;
        }
    }
}