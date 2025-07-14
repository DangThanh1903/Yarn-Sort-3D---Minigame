using UnityEngine;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Linq;


namespace ThanhScrollController.Grid
{
    [CreateAssetMenu(fileName = "ThanhData", menuName = "Data/DataMiniGame")]
    public class LevelData : SerializedScriptableObject
    {
        public List<ThanhData> levels = new List<ThanhData>();

        [Button]
        public void Generate()
        {
#if UNITY_EDITOR
            ClearGeneratedFolder();
#endif
            levels.Clear();
            List<ThanhData> tempList = new List<ThanhData>();
            Sprite[] sprites = Resources.LoadAll<Sprite>("SomeAsset");
            int count = sprites.Length;
            int index = 0;
            for (int i = 0; i < count; i++)
            {
                ThanhData thanhData = new ThanhData();
                if (!AllLinesHaveFewerUniqueColors(sprites[i].texture))
                {
                    List<Color32> temp = MapToRepresentativeColors(sprites[i].texture, 20);
                    if (!AllLinesHaveFewerUniqueColors(temp))
                    {
                        Debug.LogError($"The {i} sprite ({sprites[i].name}) is not available after double check");
                        continue;
                    }
                    else
                    {
                        Texture2D generatedTex = CreateTextureFromColors(temp, 15, 15);
#if UNITY_EDITOR
                        if (generatedTex != null)
                        {
                            string savedPath = SaveTextureAsAsset(generatedTex, sprites[i].name + "_Generated");
                            // Reload saved texture from asset to assign properly
                            generatedTex = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(savedPath);
                        }
#endif
                        thanhData.texture2D = generatedTex;
                    }
                }
                else
                {
                    thanhData.texture2D = sprites[i].texture;
                }
                thanhData.image = sprites[i];
                thanhData.id = index;
                thanhData.name = sprites[i].name;
                thanhData.duration = 120;
                thanhData.uniqueColorCount = CountUniqueColors(thanhData.texture2D);
                tempList.Add(thanhData);
                index++;
            }
            tempList = tempList.OrderBy(data => data.uniqueColorCount).ToList();
            // Reassign index after sorting
            for (int i = 0; i < tempList.Count; i++)
            {
                tempList[i].id = i;
            }

            levels = tempList;
        }
        private int CountUniqueColors(Texture2D texture)
        {
            HashSet<Color32> unique = new HashSet<Color32>(texture.GetPixels32());
            return unique.Count;
        }

        private bool AllLinesHaveFewerUniqueColors(Texture2D texture, int maxUnique = 5)
        {
            int width = texture.width;
            int height = texture.height;
            Color32[] flatColors = texture.GetPixels32();

            for (int y = 0; y < height; y++)
            {
                HashSet<Color32> uniqueColors = new HashSet<Color32>();

                for (int x = 0; x < width; x++)
                {
                    Color32 pixelColor = flatColors[y * width + x];
                    uniqueColors.Add(pixelColor);
                    if (uniqueColors.Count > maxUnique)
                        return false;
                }

            }

            return true;
        }
        private bool AllLinesHaveFewerUniqueColors(List<Color32> flatColors, int maxUnique = 5)
        {
            int width = 15;
            int height = 15;
            
            for (int y = 0; y < height; y++)
            {
                HashSet<Color32> uniqueColors = new HashSet<Color32>();

                for (int x = 0; x < width; x++)
                {
                    Color32 pixelColor = flatColors[y * width + x];
                    uniqueColors.Add(pixelColor);
                    if (uniqueColors.Count > maxUnique)
                        return false;
                }
                
            }

            return true;
        }
        private List<Color32> MapToRepresentativeColors(Texture2D texture, int threshold)
        {
            Color32[] pixels = texture.GetPixels32();
            List<Color32> distinctColors = new List<Color32>();
            List<Color32> remapped = new List<Color32>();

            foreach (var color in pixels)
            {
                Color32 representative = FindSimilar(distinctColors, color, threshold);
                if (representative.a == 0) // Not found
                {
                    distinctColors.Add(color);
                    remapped.Add(color);
                }
                else
                {
                    remapped.Add(representative);
                }
            }

            return remapped;
        }
        private Color32 FindSimilar(List<Color32> list, Color32 target, int threshold)
        {
            foreach (var c in list)
            {
                if (IsColorSimilar(c, target, threshold))
                    return c;
            }
            return new Color32(0, 0, 0, 0); // Representing "not found"
        }
        private bool IsColorSimilar(Color32 a, Color32 b, int threshold)
        {
            return Mathf.Abs(a.r - b.r) <= threshold &&
                Mathf.Abs(a.g - b.g) <= threshold &&
                Mathf.Abs(a.b - b.b) <= threshold;
        }
        public Texture2D CreateTextureFromColors(List<Color32> colors, int width, int height)
        {
            if (colors.Count != width * height)
            {
                Debug.LogError("Color list size does not match width * height.");
                return null;
            }

            Texture2D newTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            newTexture.SetPixels32(colors.ToArray());
            newTexture.Apply();

            return newTexture;
        }
        #if UNITY_EDITOR
        public static string SaveTextureAsAsset(Texture2D texture, string assetName = "GeneratedTexture")
        {
            string folderPath = "Assets/_DevThanh/GeneratedTextures";
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                AssetDatabase.CreateFolder("Assets/_DevThanh", "GeneratedTextures");
            }

            string filePath = $"{folderPath}/{assetName}.png";
            byte[] pngData = texture.EncodeToPNG();
            File.WriteAllBytes(filePath, pngData);

            AssetDatabase.ImportAsset(filePath, ImportAssetOptions.ForceUpdate);

            // Set texture import settings
            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(filePath);
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.maxTextureSize = 32;
            importer.isReadable = true;
            importer.SaveAndReimport();

            Debug.Log($"Texture saved and imported at: {filePath}");

            return filePath;
        }
        #endif
        #if UNITY_EDITOR
        private void ClearGeneratedFolder(string folderPath = "Assets/_DevThanh/GeneratedTextures")
        {
            if (!Directory.Exists(folderPath))
                return;

            string[] files = Directory.GetFiles(folderPath);

            foreach (string file in files)
            {
                if (file.EndsWith(".meta")) continue;

                File.Delete(file);
                File.Delete(file + ".meta"); // Try to delete meta as well (Unity will regenerate)
            }

            AssetDatabase.Refresh();
        }
        #endif
    }
    [Serializable]
    public class ThanhData
    {
        public int id;
        public string name;
        public Sprite image;
        public Texture2D texture2D;
        public int uniqueColorCount;
        public int duration;
    }
}