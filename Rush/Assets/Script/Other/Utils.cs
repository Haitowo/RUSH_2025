// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 03/11/2025 - Beginning of the class

namespace Com.IsartDigital.Rush.Utilities
{
    
    public struct Utils
    {

        //SO - FILE NAME
        public const string LIGHTING_PRESET = "Lighting Preset";
        public const string UI_LEVELSELECT_PRESET = "UI LevelSelect Preset";

        //SO - MENU NAME
        public const string LIGHTING_MENU = "Scriptables/Lighting Preset";
        public const string UI_LEVELSELECT_MENU = "Scriptables/UI";

        //INPUTS
        public const string MOUSE_BUTTON_X = "Mouse X";
        public const string MOUSE_BUTTON_Y = "Mouse Y";
        public const string MOUSE_WHEEL = "Mouse ScrollWheel";

        //HEADER
        public const string CAMERA_MANAGEMENT = "Camera Management";
        public const string CUBE_MANAGEMENT = "Cube Management";
        public const string GAME_SPEED = "Game Speed - Ticks";

        //ERROR MESSAGE
        public const string ERR_TICK_FOUND = "No ITickProvider found in project.";
        public const string ERR_CAMERA_MENU = "Next menu not found, switch camera position didn't work.";
        public const string ERR_HUD_SPAWN = "HUDType sélectionné invalide pour la liste de ScriptableObjects !";
    }
}