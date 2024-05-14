using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI countText;
    RectTransform rectTransform;
    public TextMeshProUGUI NameText => nameText;
    public TextMeshProUGUI CountText => countText;
    public float Height => rectTransform.rect.height;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    public void SetData(ItemSlot itemSlot)
    {
        nameText.text = itemSlot.Item.ItemName;
        countText.text = $"X {itemSlot.Count}";
    }
}
