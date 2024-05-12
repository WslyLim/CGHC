using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapManagement : MonoBehaviour
{
    [SerializeField] Maps maps;
    [Header("Map UI Management")]
    public Transform player; // Reference to the player's Transform
    public RectTransform mapImage; // Reference to the map UI RectTransform
    public RectTransform playerIcon; // Reference to the player icon on the UI map
    public GameObject map; // Map Background
    public int currentMapIndex; // Index to track which map is currently displayed


    [Header("World Map Settings")]
    public Vector2 worldMapSize; // World map size (X, Y)

    private Vector2 mapImageSize; // The size of the map UI image

    [Header("Player Icon")]
    public Slider playerIconSlider; // Slider to adjust the player icon size
    public TextMeshProUGUI playerIconValue; // Text displaying the current size of the player icon

    public static MapManagement Instance { get; private set; }
    public event Action OpenMap;


    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        mapImageSize = mapImage.rect.size;
        UpdateCurrentMap(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) 
        {
            if (map.activeSelf)
            {
                OpenMap.Invoke();
                map.SetActive(false);
            }
            else if (!map.activeSelf)
            {
                OpenMap.Invoke();
                map.SetActive(true);
            }
        }

        // Normalize the player's position to a 0-1 range
        float normalizedX = player.position.x / worldMapSize.x;
        float normalizedY = player.position.y / worldMapSize.y;

        // Scale normalized coordinates to map image size
        float mapPosX = normalizedX * mapImageSize.x;
        float mapPosY = normalizedY * mapImageSize.y;

        // Update player icon position within the map image
        playerIcon.anchoredPosition = new Vector2(mapPosX, mapPosY);

        playerIcon.sizeDelta = new Vector2(playerIconSlider.value * 15, playerIconSlider.value * 22);
        playerIconValue.text = playerIcon.sizeDelta.ToString();
    }

    public void OnOpenMap()
    {
        if (map.activeSelf)
        {
            OpenMap.Invoke();
            map.SetActive(false);
        }
        else if (!map.activeSelf)
        {
            OpenMap.Invoke();
            map.SetActive(true);
        }
    }

    public void UpdateCurrentMap(int index)
    {
        // Set the new map image
        mapImage.GetComponent<Image>().sprite = maps.mapImages[index];

        // Update the world map size
        worldMapSize = maps.mapSizes[index];

        // Update the map image size
        mapImageSize = mapImage.rect.size;
    }
}

[System.Serializable]
public class Maps
{
    public List<Sprite> mapImages;
    public List<Vector2> mapSizes;
}
