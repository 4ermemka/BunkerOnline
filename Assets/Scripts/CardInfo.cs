using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardInfo : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI categoryDisplay;
    [SerializeField] TextMeshProUGUI attributeNameDisplay;
    [SerializeField] TextMeshProUGUI descriptionDisplay;
    [SerializeField] Image iconDisplay;
    [SerializeField] Image PanelColor;
    [SerializeField] Color iconColor;

    public void SetInfo(string attributeName, string category, string description, Sprite icon, Color color) 
    {
        this.attributeNameDisplay.text = attributeName; 
        this.categoryDisplay.text = category; 
        this.descriptionDisplay.text = description; 
        this.iconDisplay.sprite = icon; 
        this.iconDisplay.color = color;
        color.a = 0.2f;
        this.PanelColor.color = color;
    }

    private void Start() 
    {
        gameObject.GetComponent<CanvasGroup>().alpha = 0;
        LeanTween.alphaCanvas(gameObject.GetComponent<CanvasGroup>(),1, 0.2f);
    } 

    public void UpdatePosition(Vector3 newPos)
    {
        gameObject.transform.position = newPos;
    }

    public void UpdateOffset(Vector3 offset)
    {
        gameObject.transform.localPosition += offset;
    }
}
