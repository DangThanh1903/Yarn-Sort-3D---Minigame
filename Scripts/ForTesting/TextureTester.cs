using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
public class TextureTester : MonoBehaviour
{
    [SerializeField] private Texture2D texture2D;
    public int gridSize = 15;
    public int threshold = 1;
    public int colorCount = 5;
    public Image imageTestTexture;
    [Button]
    public void test()
    {
        AllLinesHaveFewerUniqueColors(texture2D);
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

                // if (uniqueColors.Count > maxUnique)
                //     return false;
                Debug.Log(uniqueColors.Count);
            }
            
            // foreach (var temp in uniqueColors)
            // {
            //     Debug.Log(temp);
            // }
        }

        return true;
    }
    [Button]
    public void ReduceColors()
    {
        if (texture2D == null)
        {
            Debug.LogError("Texture2D not assigned.");
            return;
        }

        Color32[] pixels = texture2D.GetPixels32();
        List<Color32> distinctColors = GetDistinctColors(pixels, threshold);

        Debug.Log($"Original color count: {pixels.Length}, Distinct color count: {distinctColors.Count}");
        foreach (var color in distinctColors)
        {
            Debug.Log($"Color: {color.r}, {color.g}, {color.b}");
        }
    }

    private List<Color32> GetDistinctColors(Color32[] pixels, int threshold)
    {
        List<Color32> uniqueColors = new List<Color32>();

        foreach (var color in pixels)
        {
            if (!uniqueColors.Any(c => IsColorSimilar(c, color, threshold)))
            {
                uniqueColors.Add(color);
            }
        }

        return uniqueColors;
    }

    private bool IsColorSimilar(Color32 a, Color32 b, int threshold)
    {
        return Mathf.Abs(a.r - b.r) <= threshold &&
               Mathf.Abs(a.g - b.g) <= threshold &&
               Mathf.Abs(a.b - b.b) <= threshold;
    }
    [Button]
    public void ReduceAndRemapColors()
    {
        if (texture2D == null)
        {
            Debug.LogError("Texture2D not assigned.");
            return;
        }

        Color32[] pixels = texture2D.GetPixels32();
        List<Color32> remappedColors = MapToRepresentativeColors(pixels, threshold);
        imageTestTexture.sprite = ConvertToSprite(CreateTextureFromColors(remappedColors, 15, 15));
        Debug.Log($"Remapped color count: {remappedColors.Count}");
        foreach (var color in remappedColors)
        {
            Debug.Log($"Color: {color.r}, {color.g}, {color.b}");
        }
    }

    private List<Color32> MapToRepresentativeColors(Color32[] pixels, int threshold)
    {
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
    public Sprite ConvertToSprite(Texture2D texture)
    {
        return Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f) // pivot (center)
        );
    }
}

#endif