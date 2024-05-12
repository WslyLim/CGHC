using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutlineUI : MonoBehaviour
{
    public Image originalImage; // The original UI Image
    public Color outlineColor = Color.black; // Outline color
    public float outlineThickness = 0.1f; // Outline thickness as a scale factor

    void Start()
    {
        // Create a new GameObject for the outline
        var outlineObject = new GameObject("Outline");
        outlineObject.transform.SetParent(originalImage.transform.parent);

        // Add an Image component and set the sprite to the same as the original
        var outlineImage = outlineObject.AddComponent<Image>();
        outlineImage.sprite = originalImage.sprite;
        outlineImage.color = outlineColor;

        // Adjust the scale to create the outline effect
        outlineObject.transform.localScale = originalImage.transform.localScale + new Vector3(outlineThickness, outlineThickness, 0);

        // Position the outline behind the original image
        outlineObject.transform.SetSiblingIndex(originalImage.transform.GetSiblingIndex());
    }
}
