using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private TabGroup tabGroup;

    [SerializeField] private Image background;

    public UnityEvent OnTabSelected;
    public UnityEvent OnTabDeselected;

    public void SetBackground(Sprite sprite) 
    {
        background.sprite = sprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        tabGroup.OnTabSelected(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tabGroup.OnTabEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tabGroup.OnTabExit(this);
    }

    private void Start() 
    {
        background = GetComponent<Image>();
        tabGroup.Subscribe(this);
    }

    public void Select() 
    {
        OnTabSelected?.Invoke();
    }

    public void Deselect() 
    {
        OnTabDeselected?.Invoke();
    }
}
