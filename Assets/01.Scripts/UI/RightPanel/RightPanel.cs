using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RightPanel : MonoBehaviour,IPointerClickHandler
{
    private RectTransform rect;
    private Vector2 minSize;
    private Vector2 maxSize;
    private float maxX = 300;
    private float minX = 100;

    private bool isMax;
    
    [SerializeField] private GameObject button;
    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    void Start()
    {
        maxSize = new Vector2(maxX,rect.sizeDelta.y);
        minSize = new Vector2(minX,rect.sizeDelta.y);
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, minX);
    }

    // Update is called once per frame
    public void OnClick()
    {
        if(!isMax) return;
        rect.DOSizeDelta(minSize, 0.3f);
        isMax = false;
        button.SetActive(false);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        isMax = true;
        button.SetActive(true);
        rect.DOSizeDelta(maxSize, 0.3f);
    }
}
