using Com.IsartDigital.Rush.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.Rush.UI
{
    
    public class UISlotPool : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [SerializeField] private UITileSlot prefab;
        [SerializeField] private Transform container;

        private List<UITileSlot> _availableSlots = new List<UITileSlot>();

        public UITileSlot Get()
        {
            UITileSlot lSlot;
            if (_availableSlots.Count > 0)
            {
                int lLastIndex = _availableSlots.Count - 1;
                lSlot = _availableSlots[lLastIndex];
                _availableSlots.RemoveAt(lLastIndex);
                lSlot.gameObject.SetActive(true);
            }
            else
                lSlot = Instantiate(prefab, container);
            
            return lSlot;
        }

        public void Return(UITileSlot pSlot)
        {
            pSlot.gameObject.SetActive(false);
            _availableSlots.Add(pSlot);
        }

        public void ReturnAll(List<UITileSlot> pSlots)
        {
            foreach (UITileSlot slot in pSlots)
            {
                Return(slot);
            }
            pSlots.Clear();
        }
    }
}