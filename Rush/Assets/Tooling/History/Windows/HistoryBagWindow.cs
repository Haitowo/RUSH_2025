using UnityEditor.ShortcutManagement;
using static Editors.History.Utils;
using Editors.History.Parameters;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Editors.History
{
    public class HistoryBagWindow : BaseDisplayButtonWindow<string>
    {
        public static bool IsGenericMenuOpen { get; private set; } = false;

        private static Data _ClipBoardedData = null;
        private static Object[] _ClipBoardedObject = null;

        private static GUIContent[] _GuiContentArray;
        private static List<Data> _DataList = new List<Data>();
        internal static Data CurrentData;

        public static double clickTime = 0;

        public static HistoryBagWindow instance;
        private static Vector2 _ScrollPos = Vector2.zero;

        internal static HistoryParameter Parameter { get; private set; }

        static HistoryBagWindow()
        {
            System.Type lType = typeof(Editor).Assembly.GetType("UnityEditor.HostView");
            FieldInfo lField = lType.GetField("k_DockedMinSize", BindingFlags.Static | BindingFlags.NonPublic);
            lField!.SetValue(null, new Vector2(100f, ICON_SIZE));
        }

        [MenuItem("Window/History/Window")]
        public static HistoryBagWindow Open()
        {
            HistoryBagWindow lWindow = GetWindow<HistoryBagWindow>();
            lWindow.titleContent = new GUIContent(HISTORY, GetIconContent(CLOCK_ICON));

            return lWindow;
        }

        public void CreateGUI()
        {
            instance = this;
            SetFolder();
            SetDataBag();
        }

        private void OnGUI()
        {
            CheckDraggedObject();
            if (_DataList.Count >= 0 && CurrentData != null)
            {
                CreateTabs();
                ShowHistory();
            }
            else
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(EditorGUIUtility.IconContent(BAG_ICON).image, GUILayoutSize(SIZE))) DisplayBagChoosingMenu();
                GUIStyle lGUIStyle = new GUIStyle(EditorStyles.boldLabel);

                lGUIStyle.fontSize = 25;
                GUILayout.Label("Drop Item To Start", lGUIStyle);
                GUILayout.EndHorizontal();
            }
        }

        private void ShowHistory()
        {
            _ScrollPos = GUILayout.BeginScrollView(_ScrollPos);
            GUILayout.BeginHorizontal();

            int lIteration = CurrentData.GUIDList.Count - 1;
            string lObjectGUID;

            for (int i = lIteration; i >= 0; i--)
            {
                lObjectGUID = CurrentData.GUIDList[i];
                HistoryButton(lObjectGUID);
            }

            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();
        }

        public void HistoryButton(string pGUID)
        {
            string lName = GetChoppedName(GUIDToObjectName(pGUID));
            Object lObject = GUIDToObject(pGUID);

            if (lObject == null)
            {
                CurrentData.TryRemove(pGUID);
                return;
            }

            GUIContent lContent;

            if (CurrentData.showType == ShowType.None) lContent = new GUIContent(CurrentData.showName ? lName : string.Empty);
            else lContent = new GUIContent(CurrentData.showName ? lName : string.Empty, TryGetAssetPreview(lObject, CurrentData.showType));

            Rect lRect = GUILayoutUtility.GetRect(lContent, GUI.skin.button, GUILayout.Width(lName.Length * CHAR_SIZE + ICON_SIZE), GUILayout.Height(SIZE));

            if (m_FolderSelectable.IsSelecting && m_FolderSelectable.SelectList.Contains(pGUID))
                GUI.backgroundColor = GUI.skin.settings.selectionColor;

            GUI.Box(lRect, lContent, GUI.skin.button);

            if (GUI.backgroundColor == GUI.skin.settings.selectionColor)
            {
                Repaint();
                GUI.backgroundColor = Color.white;
            }

            ManageEventOnRect(lObject, lRect);
        }

        protected override void OnControlClick(Object pObject) => m_FolderSelectable.TryAddByControl(ObjectToGUID(pObject));
        protected override void OnShiftClick(Object pObject) => m_FolderSelectable.TryAddByShift(ObjectToGUID(pObject), CurrentData.GUIDList);
        protected override void OnClick(Object pObject)
        {
            DoubleClick();
            AssetDatabase.OpenAsset(pObject);
            m_FolderSelectable.ClearSelectedList();
        }

        protected override void OnLeftClick(Object pObject)
        {
            ButtonGenericMenu(pObject);
            if (!IsGenericMenuOpen) m_FolderSelectable.ClearSelectedList();
        }

        protected override void OnMouseDrag(Object pObject)
        {
            DragAndDrop.PrepareStartDrag();

            if (m_FolderSelectable.IsSelecting)
            {
                DragAndDrop.objectReferences = GUIDSToGameObject(m_FolderSelectable.SelectList).ToArray();
                DragAndDrop.paths = GUIDSToPaths(m_FolderSelectable.SelectList).ToArray();
            }
            else
            {
                DragAndDrop.objectReferences = new Object[] { pObject };
                string lAssetPath = AssetDatabase.GetAssetPath(pObject);
                DragAndDrop.paths = new string[] { lAssetPath };
            }

            DragAndDrop.StartDrag($"Dragging");
        }

        internal void SetDataBag()
        {
            if (Parameter.GetCurrentBagPath() == null)
                Parameter.SetCurrentBagPath(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets(FOLDER_SEARCH, new string[] { DATA_PATH })[0]));

            UpdateData();
            SetTitleContent(Parameter.GetCurrentBagPath());
        }

        internal void SetDataBag(int pIndex) => SetDataBag(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets(FOLDER_SEARCH, new string[] { DATA_PATH })[pIndex]));

        internal void SetDataBag(string pPath)
        {
            Parameter.SetCurrentBagPath(pPath);
            UpdateData();
            Parameter.CurrentTabNumber = 0;
            SetTitleContent(Parameter.GetCurrentBagPath());
        }

        public void UpdateData()
        {
            AssetDatabase.Refresh();

            m_FolderSelectable.ClearSelectedList();
            _DataList.Clear();

            Data lData;
            string[] lPathArray = AssetDatabase.FindAssets(T_SEARCH + nameof(Data), new string[] { Parameter.GetCurrentBagPath() });
            _GuiContentArray = new GUIContent[lPathArray.Length];

            for (int i = 0; i < lPathArray.Length; i++)
            {
                lData = AssetDatabase.LoadAssetAtPath<Data>(AssetDatabase.GUIDToAssetPath(lPathArray[i]));
                _DataList.Add(lData);

                _GuiContentArray[i] = new GUIContent(lData.tabName, lData.logo);
                lData.onValidate += OnGUI;
            }

            if (_DataList.Count <= 0) CurrentData = null;
            else CurrentData = Parameter.CurrentTabNumber > _DataList.Count ? _DataList[0] : _DataList[Parameter.CurrentTabNumber];
        }

        private void DisplayBagChoosingMenu()
        {
            GenericMenu lMenu = new GenericMenu();
            string[] lPathArray = GetFoldersPathsInFolders(Parameter.savePath).ToArray();
            int pCharacterToIgnore = Parameter.savePath.Length;

            string lCurrentTabPath = Parameter.GetCurrentBagPath();

            for (int i = 0; i < lPathArray.Length; i++)
            {
                string lPath = lPathArray[i].Replace('\\', '/');

                if (lCurrentTabPath != lPath)
                {
                    lMenu.AddItem(new GUIContent("Open" + lPath.Substring(pCharacterToIgnore)), false, () => SetDataBag(lPath));
                    lMenu.AddItem(new GUIContent("Delete" + lPath.Substring(pCharacterToIgnore) + "/You sure ?"), false, () => DeleteDirectory(lPath));
                }
            }

            lMenu.AddSeparator(string.Empty);
            lMenu.AddItem(new GUIContent("Create New Bag"), false, () => BagMakerWindow.Open());
            lMenu.AddItem(new GUIContent("Force Update"), false, () => UpdateData());

            lMenu.ShowAsContext();
        }

        private void CreateTabs()
        {
            float lTabWidth = (position.width - SIZE) / _DataList.Count;

            GUILayout.BeginHorizontal();
            Data lData;
            Rect lTabRect;
            Event lEvent = Event.current;

            for (int i = 0; i < _DataList.Count; i++)
            {
                lTabRect = GUILayoutUtility.GetRect(new GUIContent(_GuiContentArray[i]), EditorStyles.toolbarButton, GUILayout.Width(lTabWidth), GUILayout.Height(SIZE));
                lData = _DataList[i];

                if (lEvent.type == EventType.MouseDown && lTabRect.Contains(lEvent.mousePosition))
                {
                    if (lEvent.button == 0)
                    {
                        CurrentData = _DataList[i];
                        Parameter.CurrentTabNumber = i;
                        lEvent.Use();
                    }
                    else if (lEvent.button == 1)
                    {
                        TabGenericMenu(lData);
                        lEvent.Use();
                    }

                    m_FolderSelectable.ClearSelectedList();
                }
                GUI.Toggle(lTabRect, CurrentData == lData, _GuiContentArray[i], EditorStyles.toolbarButton);
            }

            if (GUILayout.Button(EditorGUIUtility.IconContent(BAG_ICON).image, GUILayoutSize(SIZE))) DisplayBagChoosingMenu();

            GUILayout.EndHorizontal();
        }

        private void TabGenericMenu(Data pData)
        {
            GenericMenu lMenu = new GenericMenu();
            IsGenericMenuOpen = true;

            lMenu.AddItem(new GUIContent("Open Data"), false, () => AssetDatabase.OpenAsset(pData));

            lMenu.AddItem(new GUIContent($"Remove/{pData.tabName} From Unity Storage/ You Sure ?"), false, () => { DeleteFile(AssetDatabase.GetAssetPath(pData)); UpdateData(); });
            lMenu.AddItem(new GUIContent($"Remove/All Objects From {pData.tabName} History"), false, () => pData.RemoveAllPath());

            CopyPaste(lMenu, pData);
            lMenu.ShowAsContext();
        }

        protected virtual void ButtonGenericMenu(Object pObject)
        {
            GenericMenu lMenu = new GenericMenu();
            IsGenericMenuOpen = true;

            if (!m_FolderSelectable.IsSelecting)
            {
                lMenu.AddItem(new GUIContent("Copy/Object"), false, () => _ClipBoardedObject = new Object[1] { pObject });
                lMenu.AddItem(new GUIContent("Copy/Path"), false, () => GUIUtility.systemCopyBuffer = AssetDatabase.GetAssetPath(pObject));
                lMenu.AddItem(new GUIContent("Copy/GUID"), false, () => GUIUtility.systemCopyBuffer = ObjectToGUID(pObject)); ;

                lMenu.AddSeparator(string.Empty);
                lMenu.AddItem(new GUIContent("Open At Path"), false, () => EditorGUIUtility.PingObject(pObject));
                lMenu.AddItem(new GUIContent("Show In Explorer"), false, () => ShowInExplorer(pObject));

                lMenu.AddSeparator(string.Empty);
                lMenu.AddItem(new GUIContent("Remove From/Tab"), false, () => CurrentData.TryRemove(pObject));
                lMenu.AddItem(new GUIContent("Remove From/Unity Project/ You Sure ?"), false, () => { CurrentData.TryRemove(pObject); DeleteFile(pObject); });
            }
            else
            {
                lMenu.AddItem(new GUIContent("Copy/Object"), false, () => _ClipBoardedObject = GUIDSToGameObject(m_FolderSelectable.SelectList).ToArray());
                lMenu.AddDisabledItem(new GUIContent("Copy/Path"));
                lMenu.AddDisabledItem(new GUIContent("Copy/GUID"));

                lMenu.AddSeparator(string.Empty);
                lMenu.AddDisabledItem(new GUIContent("Open At Path"));
                lMenu.AddDisabledItem(new GUIContent("Show In Explorer"));

                lMenu.AddSeparator(string.Empty);
                lMenu.AddItem(new GUIContent("Remove From/Tab"), false, () => { CurrentData.TryRemoves(m_FolderSelectable.SelectList); m_FolderSelectable.ClearSelectedList(); });

                lMenu.AddItem(new GUIContent("Remove From/Unity Project/ You Sure ?"), false, () =>
                {
                    foreach (string item in m_FolderSelectable.SelectList)
                    {
                        CurrentData.TryRemove(item);
                        DeleteFile(AssetDatabase.GUIDToAssetPath(item));
                    }
                    AssetDatabase.Refresh();
                });
            }
            lMenu.ShowAsContext();
        }

        protected void DoubleClick()
        {
            if ((EditorApplication.timeSinceStartup - clickTime) < DOUBLE_CLICK_TIME)
            {
                EditorGUIUtility.PingObject(Selection.activeObject);
            }
            clickTime = EditorApplication.timeSinceStartup;
        }

        private void CheckDraggedObject()
        {
            Event lEvent = Event.current;
            Rect lRect = new Rect(0f, 0f, position.width, position.height);
            if (lEvent == null) return;

            switch (lEvent.type)
            {
                case EventType.DragUpdated:
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    lEvent.Use();
                    break;

                case EventType.DragPerform:
                    DragAndDrop.AcceptDrag();
                    ManageDroppedObjects(DragAndDrop.objectReferences);
                    lEvent.Use();
                    break;
            }
        }

        private void ManageDroppedObjects(Object[] pObjectArray)
        {
            System.Type lType = pObjectArray.First().GetType();

            foreach (Object item in pObjectArray)
            {
                if (item.GetType() != lType)
                {
                    Debug.Log(lType + " was different from :" + item.GetType() + " please select multiple object from the same Type");
                    return;
                }
            }

            if (CurrentData != null && lType.Name == CurrentData.TypeName)
            {
                foreach (Object item in pObjectArray)
                {
                    CurrentData.TryAdd(item);
                }
                return;
            }

            DataWindow lWindow = DataWindow.Open(pObjectArray);
        }

        private void CopyPaste(GenericMenu pMenu, Data pData)
        {
            pMenu.AddSeparator(string.Empty);
            pMenu.AddItem(new GUIContent("Copy"), false, () => _ClipBoardedData = pData);

            if (_ClipBoardedData != null)
            {
                pMenu.AddItem(new GUIContent("Paste/Data Values"), false, () =>
                {
                    string lName = pData.name;
                    EditorUtility.CopySerialized(_ClipBoardedData, pData);
                    pData.name = lName;
                });

                pMenu.AddItem(new GUIContent("Paste/As New Tab"), false, () =>
                {
                    Data lData = CreateInstance<Data>();
                    EditorUtility.CopySerialized(_ClipBoardedData, lData);
                    AssetDatabase.CreateAsset(lData, AssetDatabase.GenerateUniqueAssetPath($"{Parameter.GetCurrentBagPath()}/{pData.name}.asset"));
                    UpdateData();
                });
            }
            else
            {
                pMenu.AddDisabledItem(new GUIContent("Paste/Data Values"));
                pMenu.AddDisabledItem(new GUIContent("Paste/As New Tab"));
            }
        }

        private static void SetFolder() //TODO Gerer le ficher par défaut pour le current BagPath/ ouvrir Gitignore handler
        {
            string[] lPathArray = GetSubPaths(DATA_PATH).ToArray();
            int lInt = lPathArray.Length;

            string lParentPath = lPathArray[0];
            string lChildPath;


            for (int i = 1; i < lInt; i++)
            {
                lChildPath = lPathArray[i];
                if (!AssetDatabase.IsValidFolder(lChildPath)) AssetDatabase.CreateFolder(lParentPath, Path.GetFileName(lChildPath));
                lParentPath = lPathArray[i];
            }

            Parameter = GetHistoryParameter();

            if (Parameter == null)
            {
                HistoryParameter lParameter = CreateInstance<HistoryParameter>();
                AssetDatabase.CreateAsset(lParameter, HISTORY_PARAMETER_PATH);
                Parameter = GetHistoryParameter();
                GitignoreHandlerWindow.Open();
            }

            if (Directory.EnumerateDirectories(Parameter.savePath).Count() <= 0)
            {
                AssetDatabase.CreateFolder(Parameter.savePath, DEFAULT_CATEGORY);
                AssetDatabase.CreateFolder(Parameter.savePath + "/" + DEFAULT_CATEGORY, DEFAULT_TAB);
                Parameter.SetCurrentBagPath(Parameter.savePath + "/" + DEFAULT_CATEGORY + "/" + DEFAULT_TAB);
            }


            AssetDatabase.Refresh();
        }

        private void SetTitleContent(string pCurrentTab)
        {
            titleContent = new GUIContent(Path.GetFileName(Directory.GetParent(pCurrentTab).ToString()) + "/" + Path.GetFileName(pCurrentTab), GetIconContent(CLOCK_ICON));
        }

        public static void StaticRepaint() => instance.Repaint();

        #region Shortcut

        [Shortcut("XSearch", KeyCode.X, ShortcutModifiers.Shift)]
        private static void OpenSearchWindow()
        {
            SearchWindow lWindow = SearchWindow.Open(CurrentData, true, false, false);
            lWindow.onObjectSelected += (pObject) => CurrentData.TryAdds(pObject);
        }

        [Shortcut("New Bag", KeyCode.N, ShortcutModifiers.Shift)]
        public static void ShortCutBagMakerWindow() => BagMakerWindow.Open();


        [Shortcut("Test1", KeyCode.Alpha1, ShortcutModifiers.Shift)]
        public static void Test1()
        {
            if (_DataList.Count - 1 >= 0)
            {
                CurrentData = _DataList[0];
                StaticRepaint();
            }
        }

        [Shortcut("Test2", KeyCode.Alpha2, ShortcutModifiers.Shift)]
        public static void Test2()
        {
            if (_DataList.Count - 1 >= 1)
            {
                CurrentData = _DataList[1];
                StaticRepaint();
            }
        }

        [Shortcut("Test3", KeyCode.Alpha3, ShortcutModifiers.Shift)]
        public static void Test3()
        {
            if (_DataList.Count - 1 >= 2)
            {
                CurrentData = _DataList[2];
                StaticRepaint();
            }
        }

        [Shortcut("Test4", KeyCode.Alpha4, ShortcutModifiers.Shift)]
        public static void Test4()
        {
            if (_DataList.Count - 1 >= 3)
            {
                CurrentData = _DataList[3];
                StaticRepaint();
            }
        }

        [Shortcut("Test5", KeyCode.Alpha5, ShortcutModifiers.Shift)]
        public static void Test5()
        {
            if (_DataList.Count - 1 >= 4)
            {
                CurrentData = _DataList[4];
                StaticRepaint();
            }
        }

        [Shortcut("Test6", KeyCode.Alpha6, ShortcutModifiers.Shift)]
        public static void Test6()
        {
            if (_DataList.Count - 1 >= 5)
            {
                CurrentData = _DataList[5];
                StaticRepaint();
            }
        }

        [Shortcut("Test7", KeyCode.Alpha7, ShortcutModifiers.Shift)]
        public static void Test7()
        {
            if (_DataList.Count - 1 >= 6)
            {
                CurrentData = _DataList[6];
                StaticRepaint();
            }
        }

        [Shortcut("Test8", KeyCode.Alpha8, ShortcutModifiers.Shift)]
        public static void Test8()
        {
            if (_DataList.Count - 1 >= 7)
            {
                CurrentData = _DataList[7];
                StaticRepaint();
            }
        }

        [Shortcut("Test9", KeyCode.Alpha9, ShortcutModifiers.Shift)]
        public static void Test9()
        {
            if (_DataList.Count - 1 >= 8)
            {
                CurrentData = _DataList[8];
                StaticRepaint();
            }
        }

        [Shortcut("Test10", KeyCode.Alpha0, ShortcutModifiers.Shift)]
        public static void Test10()
        {
            if (_DataList.Count - 1 >= 9)
            {
                CurrentData = _DataList[9];
                StaticRepaint();
            }
        }

        [Shortcut("Go Righttt", KeyCode.RightArrow, ShortcutModifiers.Control | ShortcutModifiers.Shift)]
        public static void GoRighttt()
        {
            int lIndex = _DataList.IndexOf(CurrentData);
            CurrentData = _DataList[lIndex < _DataList.Count - 1 ? lIndex + 1 : 0];
            StaticRepaint();
        }

        [Shortcut("Go Leftttt", KeyCode.LeftArrow, ShortcutModifiers.Control | ShortcutModifiers.Shift)]
        public static void GoLefttt()
        {
            int lIndex = _DataList.IndexOf(CurrentData);
            CurrentData = _DataList[lIndex > 0 ? lIndex - 1 : _DataList.Count - 1];
            StaticRepaint();
        }

        #endregion
    }
}