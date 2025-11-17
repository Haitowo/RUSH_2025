using static Editors.History.Utils;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace Editors.History.Parameters
{
    public class GitignoreHandlerWindow : EditorWindow
    {
        private const string SUMMARY = "#To ignore the tabs within the project\n";

        private static bool _IsSaveFileIgnored = false;
        private static bool _IsGitignoreFound = false;

        private static string gitignorePath;
        private static string gitignoreText;

        private static HistoryParameter _Parameter;

        private static GUIStyle _GUIStyle;

        [MenuItem("Window/History/Gitignore Parameter")]
        public static GitignoreHandlerWindow Open()
        {
            GitignoreHandlerWindow lWindow = GetWindow<GitignoreHandlerWindow>();
            lWindow.titleContent = new GUIContent("Gitignore Handler", (Texture2D)EditorGUIUtility.IconContent("FolderOpened On Icon").image);

            return lWindow;
        }

        private void CreateGUI()
        {
            _GUIStyle = new GUIStyle(EditorStyles.boldLabel);

            _Parameter = GetHistoryParameter();
            GetText();
        }

        private void OnGUI()
        {
            if (_IsGitignoreFound) ShowPathUpdatingButton();
            else ShowError();
        }

        private void GetText()
        {
            gitignorePath = Application.dataPath.Substring(0, Application.dataPath.Length - ASSETS.Length) + "/" + ".gitignore";
            try
            {
                gitignoreText = File.ReadAllText(gitignorePath);
                _IsSaveFileIgnored = gitignoreText.Contains(GetFullGitMessage(), System.StringComparison.OrdinalIgnoreCase);
                _IsGitignoreFound = true;
            }
            catch { }
        }

        private void ShowPathUpdatingButton()
        {
            GUILayout.Label($"Updates your .gitignore by adding");
            GUILayout.Label($"{GetFullGitMessageNoLine()}\n", _GUIStyle);
            GUILayout.Label($"Please Commit your Gitignore before using the extension");

            if (_IsSaveFileIgnored)
            {
                GUI.backgroundColor = Color.green;
                if (GUILayout.Button("Already Updated")) { }
            }
            else
            {
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("Need Update")) SetPathInGitignore();
            }

            GUI.backgroundColor = Color.white;
        }

        private void ShowError()
        {
            GUI.backgroundColor = Color.red;
            GUILayout.Label($"You're .gitignore was not found, please click to copy then paste in into your gitignore");
            if (GUILayout.Button("Copy")) GUIUtility.systemCopyBuffer = GetFullGitMessageNoLine();
            GUI.backgroundColor = Color.white;
        }

        public void SetPathInGitignore() //TODO REMOVE PRECEDENT
        {
            string lFullMessage = GetFullGitMessage();

            if (gitignoreText.Contains(lFullMessage))
            {
                gitignoreText += "\n" + lFullMessage;
                File.WriteAllText(gitignorePath, gitignoreText);
            }

            _IsSaveFileIgnored = true;
        }

        private string GetFullGitMessageNoLine() => SUMMARY + "/" + _Parameter.savePath;
        private string GetFullGitMessage() => "\n" + SUMMARY + "/" + _Parameter.savePath;
    }
}