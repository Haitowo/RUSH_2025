using Editors.History.Parameters;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Editors.History
{
    public static class Utils
    {
        public const double DOUBLE_CLICK_TIME = .3f;
        public const int NAME_MAX_CHARACTER = 12;
        public const float SIZE = 23f, CHAR_SIZE = 7f, ICON_SIZE = 29f;

        public const string FOLDER_SEARCH = T_SEARCH + "DefaultAsset";

        public const string DOT_META = ".meta";
        public const string ASSETS = "Assets/";
        public const string ASSETS_STREAMING = "Assets/StreamingAssets/";

        public const string BAG_ICON = "Asset Store@2x";
        public const string HISTORY = "History";
        public const string ADD_ICON = "CreateAddNew@2x";
        public const string PLUS_WHITE_ICON = "d_Toolbar Plus";

        public const string ASSET_EXTENSION = ".asset", FOLDER_ADDED_ICON = "Collab.FolderAdded";

        public const string CLOCK_ICON = "d_UnityEditor.AnimationWindow",
            FOLDER_ICON = "d_FolderOpened Icon", CROSS_ICON = "CrossIcon", PLUS_ICON = "Toolbar Plus@2x"
            , LISTENING_ICON = "d_ViewToolOrbit On@2x";

        public const string SEARCH_ICON = "d_Search Icon";
        public const string ROTATE_ICON = "d_RotateTool On@2x";

        public const string DATA_PATH = "Assets/Tooling/Editor/History/Saves";
        public const string HISTORY_PARAMETER_PATH = "Assets/Tooling/Editor/History/Saves/HistoryParameter.asset";
        public const string GIT_PATH = "ToSet";

        public const string T_SEARCH = "t:";
        public const float DEFAULT = 23f;

        public const string DEFAULT_CATEGORY = "Default Category";
        public const string DEFAULT_TAB = "Default Tab Holder";

        public static void DeleteDirectory(string pPath)
        {
            File.Delete(pPath + ".meta");
            Directory.Delete(pPath, true);
            AssetDatabase.Refresh();
        }

        public static void DeleteFile(Object pObject) => DeleteFile(AssetDatabase.GetAssetPath(pObject));

        public static void DeleteFile(string pPath)
        {
            File.Delete(pPath + ".meta");
            File.Delete(pPath);
            AssetDatabase.Refresh();
        }

        public static GUILayoutOption[] GUILayoutSize(float pSize = DEFAULT) => new GUILayoutOption[] { GUILayout.Width(pSize), GUILayout.Height(pSize) };
        public static GUILayoutOption[] GUILayoutSize(float pHeigth, float pWidth) => new GUILayoutOption[] { GUILayout.Width(pHeigth), GUILayout.Height(pWidth) };

        public static Texture2D GetIconContent(string pPath) => (Texture2D)EditorGUIUtility.IconContent(pPath).image;

        public static Texture2D GetSelectionTexture() => (Texture2D)AssetDatabase.GetCachedIcon(AssetDatabase.GetAssetPath(Selection.activeObject));
        public static Texture2D TextureFromGUID(string pGUID) => (Texture2D)AssetDatabase.GetCachedIcon(AssetDatabase.GUIDToAssetPath(pGUID));
        public static Texture2D TryGetAssetPreview(UnityEngine.Object pObject)
        {
            Texture2D lTexture = AssetPreview.GetAssetPreview(pObject);
            return lTexture == null ? AssetPreview.GetMiniThumbnail(pObject) : lTexture;
        }
        public static Texture2D TryGetAssetPreview(UnityEngine.Object pObject, ShowType pType)
        {
            if (pType == ShowType.Preview)
            {
                Texture2D lTexture = AssetPreview.GetAssetPreview(pObject);
                return lTexture == null ? AssetPreview.GetMiniThumbnail(pObject) : lTexture;
            }
            else return AssetPreview.GetMiniThumbnail(pObject);
        }

        public static T GUIDToObject<T>(string pGUID) where T : UnityEngine.Object => AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(pGUID));
        public static UnityEngine.Object GUIDToObject(string pGUID) => AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(AssetDatabase.GUIDToAssetPath(pGUID));
        public static string GUIDToObjectName(string pGUID) => Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(pGUID));
        public static string ObjectToGUID(UnityEngine.Object pObject) => AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(pObject)).ToString();

        public static string GetChoppedName(string pName, int pMaxCharacter = NAME_MAX_CHARACTER)
            => pName.Length >= NAME_MAX_CHARACTER ? pName.Substring(0, pMaxCharacter) + "..." : pName;

        public static IEnumerable<string> GetSubPaths(string pPath)
        {
            string[] lSubPathArray = pPath.Split(new[] { '/', '\\' }, System.StringSplitOptions.RemoveEmptyEntries);

            string lSubPath = string.Empty;
            string lCurrent = string.Empty;

            int lInt = lSubPathArray.Length;

            for (int i = 0; i < lInt; i++)
            {
                lSubPath = lSubPathArray[i];
                lCurrent = string.IsNullOrEmpty(lCurrent) ? lSubPath : Path.Combine(lCurrent, lSubPath);
                yield return lCurrent.Replace('/', '\\');
            }
        }

        public static Texture2D CreateTexture(int pWidth, int pHeight, Color pColor)
        {
            System.Span<Color> lPixelSpan = stackalloc Color[pWidth * pHeight];
            for (int i = 0; i < lPixelSpan.Length; i++) lPixelSpan[i] = pColor;

            Texture2D lTexture = new Texture2D(pWidth, pHeight);
            lTexture.SetPixels(lPixelSpan.ToArray());
            lTexture.Apply();
            return lTexture;
        }

        public static IEnumerable<Object> GUIDSToGameObject(IEnumerable<string> pEnumerable)
        {
            foreach (string item in pEnumerable)
            {
                yield return GUIDToObject(item);
            }
        }

        public static IEnumerable<string> GUIDSToPaths(IEnumerable<string> pEnumerable)
        {
            foreach (string item in pEnumerable)
            {
                yield return AssetDatabase.GUIDToAssetPath(item);
            }
        }

        public static HistoryParameter GetHistoryParameter() => AssetDatabase.LoadAssetAtPath<HistoryParameter>(HISTORY_PARAMETER_PATH);

        public static void ShowInExplorer(Object pObject)
        {
            if (pObject != null)
            {
                ShowInExplorer(AssetDatabase.GetAssetPath(pObject));
            }
        }

        public static void ShowInExplorer(string pPath)
        {
            if (File.Exists(pPath))
            {
                Process.Start("explorer.exe", "/select," + GetAssetObjectFullPath(pPath));
            }
        }


        public static IEnumerable<string> GetFoldersPathsInFolders(string pParentFolder)
        {
            string[] pDirectories = Directory.GetDirectories(pParentFolder);

            for (int i = 0; i < pDirectories.Length; i++)
            {
                foreach (string item in Directory.GetDirectories(pDirectories[i]))
                {
                    yield return item;
                }
            }
        }

        public static string GetAssetObjectFullPath(string pPath) => Path.Combine(Application.dataPath, pPath.Substring(ASSETS.Length)).Replace("/", "\\");
        public static string GetAssetObjectFullPath(Object pObject) => GetAssetObjectFullPath(AssetDatabase.GetAssetPath(pObject));
    }
}
