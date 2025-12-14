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
        public const string VERTICAL_INPUTS = "Vertical";
        public const string HORIZONTAL_INPUTS = "Horizontal";

        //HEADER
        public const string CAMERA_MANAGEMENT = "Camera Management";
        public const string CUBE_MANAGEMENT = "Cube Management";
        public const string PARTICLES_MANAGEMENT = "Particles Management";
        public const string COLORS_MANAGEMENT = "Colors - Dictionnary";
        public const string PARAMETERS_SPAWNER = "Parameters - Spawner";
        public const string PARAMETERS_LEVEL = "Parameters - Level";
        public const string PARAMETERS_HUD = "Parameters - HUD";
        public const string PARAMETERS_MENU = "Parameters - Menu";
        public const string PARAMETERS_SOUND = "Parameters - Sound";
        public const string PARAMETERS_PREVIEW = "Parameters - Preview Tile";
        public const string GAME_SPEED = "Game Speed - Ticks";
        public const string SOUND = "Sounds";

        //ERROR MESSAGE
        public const string ERR_TICK_FOUND = "No ITickProvider found in project.";
        public const string ERR_CAMERA_MENU = "Next menu not found, switch camera position didn't work.";
        public const string ERR_HUD_SPAWN = "HUDType sélectionné invalide pour la liste de ScriptableObjects !";
        public const string ERR_SNAP_POS = "TilePreviewManager: Impossible de snap la position ! La case est invalide ou hors layer mask.";
        public const string ERR_LEVEL_CONTAINER_PARENT = "Aucun LevelDesignSpawner trouvé dans la scène !";

        //TRIGGER
        public const string TAG_CUBE = "CUBE";

        //SOUND
        public const string MASTER_PARAM = "MasterVolume";
        public const string MUSIC_PARAM = "MusicVolume";
        public const string SOUND_PARAM = "SFXVolume";

        //SCENE NAMES
        public const string SCENE_GAME = "Game";
    }
}