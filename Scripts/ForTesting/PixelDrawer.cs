using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PixelPainter : MonoBehaviour
{
    [SerializeField] RawImage canvasImage;
    [SerializeField] Color currentColor = Color.black;
    [SerializeField] List<Button> changeColorButton;
    Texture2D texture;
    int width = 15;
    int height = 15;

    void Start()
    {
        texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point; // Keeps pixels sharp
        ClearCanvas();
        canvasImage.texture = texture;
        SetButtonColor(0, Color.red);
        SetButtonColor(1, Color.blue);
        SetButtonColor(2, Color.cyan);
        SetButtonColor(3, Color.white);
    }

    void SetButtonColor(int index, Color color)
    {
        changeColorButton[index].onClick.AddListener(() =>
        {
            SetColor(color);
        });
        changeColorButton[index].GetComponent<Image>().color = color;
    }

    void Update()
    {
        if (Input.GetMouseButton(0)) // Left mouse or touch
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasImage.rectTransform,
                Input.mousePosition,
                null,
                out localPoint);

            Rect rect = canvasImage.rectTransform.rect;
            float px = (localPoint.x - rect.x) / rect.width;
            float py = (localPoint.y - rect.y) / rect.height;

            int x = Mathf.FloorToInt(px * width);
            int y = Mathf.FloorToInt(py * height);

            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                texture.SetPixel(x, y, currentColor);
                texture.Apply();
            }
        }
    }

    public void ClearCanvas()
    {
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                texture.SetPixel(x, y, Color.white);
        texture.Apply();
    }

    public void SetColor(Color newColor)
    {
        currentColor = newColor;
    }
}
