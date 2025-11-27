using Com.IsartDigital.Rush.Manager;
using UnityEngine;
using UnityEngine.UI;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.Rush.UI
{
    
    public class HUDResetButton : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [SerializeField] private Button _CurrentResetButton;
        [SerializeField] private HUDTileToPlace _HUDTileToPlace;

        private TileSelectionManager _TileSelectionManager => TileSelectionManager.Instance;
        private TilePreviewManager _TilePreviewManager => TilePreviewManager.Instance;

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        private void Start() => _CurrentResetButton.onClick.AddListener(ResetTiles);
        
        private void ResetTiles()
        {
            _HUDTileToPlace.ResetSlot();
            _TileSelectionManager.ResetAll();
            _TilePreviewManager.ResetPreview();
        }
    }
}