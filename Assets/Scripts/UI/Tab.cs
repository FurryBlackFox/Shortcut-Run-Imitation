using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Tab : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] 
    private Canvas tabContent;

    [SerializeField]
    private Sprite activeStateSprite;
    
    private TabGroup tabGroup;

    private Image image;
    
    private Sprite inactiveStateSprite;


    private void Awake()
    {
        image = GetComponent<Image>();
        inactiveStateSprite = image.sprite;
        IsActive(false);
    }

    private void Start()
    {

    }

    public void SubscribeToTabGroup(TabGroup group)
    {
        tabGroup = group;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        tabGroup.OnTabEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tabGroup.OnTabExit(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
       tabGroup.OnTabClick(this);
    }

    public void IsActive(bool value)
    {
        tabContent.enabled = value;
        image.sprite = value ? activeStateSprite : inactiveStateSprite;
    }
}
