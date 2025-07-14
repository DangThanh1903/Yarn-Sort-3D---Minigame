using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
namespace MiniGameThanh
{
#if UNITY_EDITOR
public class CustomEditorWindow : EditorWindow
{
    private GameObject prefab;
    private Texture2D texture;

    [MenuItem("Tools/Level generator")]
    public static void ShowWindow()
    {
        CustomEditorWindow window = GetWindow<CustomEditorWindow>("Prefab and Texture Editor");
        window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Prefab Field", EditorStyles.boldLabel);
        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);

        EditorGUILayout.LabelField("Texture Field", EditorStyles.boldLabel);
        texture = (Texture2D)EditorGUILayout.ObjectField("Texture", texture, typeof(Texture2D), false);

        if (prefab != null && texture != null && GUILayout.Button("Create Modified Prefab"))
        {
            var (colorList, isOk) = LoadPngToColorArray(texture);
            if (!isOk)
            {
                Debug.LogError("The texture have too many color (bigger than 5)!");
                return;
            }
            CreateModifiedPrefab(prefab, colorList);
        }
    }

    private void CreateModifiedPrefab(GameObject basePrefab, Color[,] value)
    {
        // Create an instance of the prefab
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(basePrefab);
        if (instance == null)
        {
            Debug.LogError("Failed to instantiate prefab.");
            return;
        }

        // Modify the component
        GameplayController comp = instance.GetComponent<GameplayController>();
        if (comp != null)
        {
            comp.SetColorMap(value); // Use the setter method you added
            EditorUtility.SetDirty(comp);
        }
        else
        {
            Debug.LogWarning("GameplayController not found on prefab.");
        }

        // Ensure save folder exists
        string folderPath = "Assets/_DevThanh/ModifiedPrefabs";
        if (!AssetDatabase.IsValidFolder(folderPath))
            AssetDatabase.CreateFolder("Assets/_DevThanh", "ModifiedPrefabs");

        // Generate unique prefab name and path
        string newPath = $"{folderPath}/{basePrefab.name}_Modified.prefab";
        newPath = AssetDatabase.GenerateUniqueAssetPath(newPath);

        // Save the new prefab
        PrefabUtility.SaveAsPrefabAsset(instance, newPath);
        DestroyImmediate(instance);

        Debug.Log("New modified prefab saved to: " + newPath);
    }

    private static (Color[,], bool) LoadPngToColorArray(Texture2D texture)
    {
        int width = texture.width;
        int height = texture.height;
        
        Color[] flatColors = texture.GetPixels();
        Color[,] colorGrid = new Color[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colorGrid[x, y] = flatColors[y * width + x];
            }
        }

        return (colorGrid, AllLinesHaveFewerUniqueColors(colorGrid, 5));
    }

    public static bool AllLinesHaveFewerUniqueColors(Color[,] colorGrid, int maxUnique)
    {
        int width = colorGrid.GetLength(0);
        int height = colorGrid.GetLength(1);

        for (int y = 0; y < height; y++)
        {
            HashSet<Color> uniqueColors = new HashSet<Color>();

            for (int x = 0; x < width; x++)
            {
                uniqueColors.Add(colorGrid[x, y]);
                if (uniqueColors.Count > maxUnique)
                    return false; // Early exit if one row exceeds the limit
            }
        }

        return true;
    }
}
#endif
}
