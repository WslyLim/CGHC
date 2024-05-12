using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class SubtitleBehaviour : PlayableBehaviour
{
    public string subtitleText;
    public float lettersPerSecond; // Rate for revealing text

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        TextMeshProUGUI textMesh = playerData as TextMeshProUGUI;
        if (textMesh == null) return;

        // Get the current time
        double currentTime = playable.GetTime();

        // Calculate the number of characters to reveal based on the current time and letters per second
        int totalCharacters = subtitleText.Length;
        int charactersToReveal = Mathf.FloorToInt((float)currentTime * lettersPerSecond);

        // Ensure the number of characters to reveal does not exceed the total characters
        charactersToReveal = Mathf.Min(charactersToReveal, totalCharacters);

        // Update the displayed text with the appropriate substring
        textMesh.text = subtitleText.Substring(0, charactersToReveal);
        textMesh.color = Color.white; // Ensure text is visible
    }
}
