using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelEditorWindow : EditorWindow
{
    private LevelData currentLevel;
    private bool isDirty;
    private List<LevelData> levels = new();
    private int selectedIndex = -1;

    [MenuItem("Tools/Match3/Level Editor")]
    public static void Open()
    {
        var window = GetWindow<LevelEditorWindow>();
        window.UpdateTitle();
    }

    private void OnGUI()
    {
        DrawLevelList();

        GUILayout.Space(10);

        DrawBottomButtons();

        GUILayout.Space(10);

        if (currentLevel == null)
            return;

        DrawLevelFields();
    }
    private void OnEnable()
    {
        LoadLevels();
    }

    private void DrawLevelList()
    {
        EditorGUILayout.LabelField(
            "Levels",
            EditorStyles.boldLabel);

        for (int i = 0; i < levels.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            GUI.backgroundColor =
                selectedIndex == i
                ? Color.gray
                : Color.white;

            if (GUILayout.Button(levels[i].name))
            {
                selectedIndex = i;
                currentLevel = levels[i];

                isDirty = false;
                UpdateTitle();
            }

            GUI.backgroundColor = Color.white;

            if (GUILayout.Button("Select", GUILayout.Width(60)))
            {
                Selection.activeObject = levels[i];
            }

            EditorGUILayout.EndHorizontal();
        }
    }
    private void DrawBottomButtons()
    {
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("+", GUILayout.Height(30)))
        {
            CreateNewLevel();
            LoadLevels();
        }

        GUI.enabled = currentLevel != null;

        if (GUILayout.Button("-", GUILayout.Height(30)))
        {
            RemoveCurrentLevel();
        }

        if (GUILayout.Button("Save", GUILayout.Height(30)))
        {
            Save();
        }

        GUI.enabled = true;

        EditorGUILayout.EndHorizontal();
    }
    private void RemoveCurrentLevel()
    {
        if (currentLevel == null)
            return;

        string path =
            AssetDatabase.GetAssetPath(currentLevel);

        RemoveLevelFromBootstrap(currentLevel);

        levels.Remove(currentLevel);

        AssetDatabase.DeleteAsset(path);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        currentLevel = null;
        selectedIndex = -1;

        LoadLevels();
    }
    private void RemoveLevelFromBootstrap(LevelData level)
    {
        GameBootstrap bootstrap =
            FindFirstObjectByType<GameBootstrap>();

        if (bootstrap == null)
            return;

        SerializedObject serializedObject =
            new SerializedObject(bootstrap);

        SerializedProperty levelsProperty =
            serializedObject.FindProperty("levels");

        for (int i = levelsProperty.arraySize - 1; i >= 0; i--)
        {
            SerializedProperty element =
                levelsProperty.GetArrayElementAtIndex(i);

            if (element.objectReferenceValue == level)
            {
                levelsProperty.DeleteArrayElementAtIndex(i);
            }
        }

        serializedObject.ApplyModifiedProperties();

        EditorUtility.SetDirty(bootstrap);
    }
    private void DrawLevelFields()
    {
        EditorGUI.BeginChangeCheck();

        currentLevel.width = Mathf.Clamp(
            EditorGUILayout.IntField("Width", currentLevel.width), 1, 8);

        currentLevel.height = Mathf.Clamp(
            EditorGUILayout.IntField("Height", currentLevel.height), 1, 8);

        currentLevel.movesLimit = Mathf.Clamp(
            EditorGUILayout.IntField("Moves Limit", currentLevel.movesLimit), 1, 50);
        currentLevel.targetScore = Mathf.Clamp(
            EditorGUILayout.IntField("Target Score", currentLevel.targetScore), 1, 9999);


        SerializedObject serializedObject = new SerializedObject(currentLevel);
        SerializedProperty tilesProperty = serializedObject.FindProperty("availableTiles");
        EditorGUILayout.PropertyField(tilesProperty, true);

        serializedObject.ApplyModifiedProperties();

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(currentLevel);
        }
    }

    private void CreateNewLevel()
    {
        currentLevel = CreateInstance<LevelData>();

        currentLevel.width = 8;
        currentLevel.height = 8;
        currentLevel.movesLimit = 25;
        currentLevel.targetScore = 1500;

        currentLevel.availableTiles = new List<TileData>();

        string[] guids =
            AssetDatabase.FindAssets("t:TileData");

        for (int i = 0; i < Mathf.Min(3, guids.Length); i++)
        {
            string path =
                AssetDatabase.GUIDToAssetPath(guids[i]);

            TileData tile =
                AssetDatabase.LoadAssetAtPath<TileData>(path);

            if (tile != null)
            {
                currentLevel.availableTiles.Add(tile);
            }
        }

        selectedIndex = -1;

        isDirty = true;
        UpdateTitle();
    }
    private void Save()
    {
        if (!ValidateLevel())
            return;
        if (AssetDatabase.Contains(currentLevel))
        {
            EditorUtility.SetDirty(currentLevel);
            AssetDatabase.SaveAssets();
            return;
        }

        const string folderPath = "Assets/SO";

        int index = 1;

        string path;

        do
        {
            path = $"{folderPath}/Level_{index}.asset";
            index++;
        }
        while (AssetDatabase.LoadAssetAtPath<LevelData>(path) != null);

        AssetDatabase.CreateAsset(currentLevel, path);
        AddLevelToBootstrap(currentLevel);
        EditorUtility.SetDirty(currentLevel);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = currentLevel;

        LoadLevels();
        Debug.Log($"Saved Level: {path}");
    }
    private void LoadLevels()
    {
        levels.Clear();

        string[] guids =
            AssetDatabase.FindAssets("t:LevelData", new[] { "Assets/SO" });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            LevelData level =
                AssetDatabase.LoadAssetAtPath<LevelData>(path);

            levels.Add(level);
        }
    }
    private void UpdateTitle()
    {
        string title = isDirty
            ? "Level Editor *"
            : "Level Editor";

        titleContent = new GUIContent(title);
    }
    private void AddLevelToBootstrap(LevelData level)
    {
        GameBootstrap bootstrap = FindFirstObjectByType<GameBootstrap>();

        if (bootstrap == null)
        {
            Debug.LogWarning("GameBootstrap not found in scene.");
            return;
        }

        SerializedObject serializedObject = new SerializedObject(bootstrap);
        SerializedProperty levelsProperty = serializedObject.FindProperty("levels");

        int index = levelsProperty.arraySize;

        levelsProperty.InsertArrayElementAtIndex(index);

        SerializedProperty element = levelsProperty.GetArrayElementAtIndex(index);

        element.objectReferenceValue = level;

        serializedObject.ApplyModifiedProperties();

        EditorUtility.SetDirty(bootstrap);

        Debug.Log($"Added {level.name} to GameBootstrap.");
    }
    private bool ValidateLevel()
    {
        if (currentLevel.width < 3 || currentLevel.width > 8)
        {
            ShowError("Width must be between 3 and 8.");
            return false;
        }

        if (currentLevel.height < 3 || currentLevel.height > 8)
        {
            ShowError("Height must be between 3 and 8.");
            return false;
        }

        if (currentLevel.movesLimit < 1 || currentLevel.movesLimit > 50)
        {
            ShowError("Moves Limit must be between 1 and 50.");
            return false;
        }

        if (currentLevel.targetScore < 1)
        {
            ShowError("Target Score must be greater than 0.");
            return false;
        }

        if (currentLevel.availableTiles == null)
        {
            ShowError("Available Tiles list is null.");
            return false;
        }

        HashSet<TileData> uniqueTiles = new();

        foreach (TileData tile in currentLevel.availableTiles)
        {
            if (tile == null)
            {
                ShowError("Available Tiles contains NULL tile.");
                return false;
            }

            uniqueTiles.Add(tile);
        }

        if (uniqueTiles.Count < 3)
        {
            ShowError("Need at least 3 UNIQUE tiles.");
            return false;
        }

        return true;
    }
    private void ShowError(string message)
    {
        EditorUtility.DisplayDialog(
            "Invalid Level",
            message,
            "OK");
    }
}