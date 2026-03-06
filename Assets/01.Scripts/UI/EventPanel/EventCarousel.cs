using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EventCarousel : MonoBehaviour
{
    private int currentIndex = 1;
    private float slidingTime = 5f;
    private float slidingDuring = 2f;
    private int contentCount;
    private float contentSize;
    private float snapDuring = 0.1f;
    private int lastCopyIndex;
    private int lastRealIndex;
    
    private CarouselScrollRect  scrollRect;
    private List<Image> images = new List<Image>();
    
    [SerializeField] private RectTransform content;
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private Transform dotContainer;

    private void Awake()
    {
        scrollRect = GetComponent<CarouselScrollRect>();
    }

    void Start()
    {
        SetCopyImage();

        SetValue();
        
        InitPos();
        SetDot();
        DOTween.Init();
        
        StartCoroutine(AutoSliding());
    }
    private void SetValue()
    {
        contentCount = content.transform.childCount;
        
        contentSize = 1f / (contentCount - 1);
        lastRealIndex = contentCount - 2;
        lastCopyIndex = contentCount - 1;
    }
    private void OnEnable()
    {
        scrollRect.onEndDrag += OnSnap;
        scrollRect.onBeginDrag += OnBeginDrag;
    }
    private void OnDisable()
    {
        scrollRect.onEndDrag -= OnSnap;
        scrollRect.onBeginDrag -= OnBeginDrag;
    }
    private void OnBeginDrag(PointerEventData obj)
    {
        DOTween.Kill(scrollRect);
    }

    private void SetCopyImage()
    {
        GameObject copyImage = Instantiate(content.GetChild(content.childCount-1).gameObject,content.transform);
        Instantiate(content.GetChild(0).gameObject,content.transform); 
        copyImage.transform.SetAsFirstSibling();
    }

    private void InitPos()
    {
        scrollRect.horizontalNormalizedPosition =  contentSize;
    }

    private void SetDot()
    {
        for(int i=0 ;i < lastRealIndex; i++)
        {
            Image dot = Instantiate(dotPrefab, dotContainer.transform).GetComponent<Image>();
            images.Add(dot);
        }
    }

    private void UpdateDot(int index)
    {
        Debug.Log("UpdateDot index: " + index + " images.Count: " + images.Count);
        foreach (Image image in images) 
        {
            image.color = Color.white;
        }
        images[index-1].color = Color.red;
    }
    
    private IEnumerator AutoSliding()
    {
        while (true) 
        {
            yield return new WaitForSeconds(slidingTime);
            
            currentIndex++;
            
            
            float currentPos = contentSize * currentIndex;

            scrollRect.DOHorizontalNormalizedPos(currentPos, slidingDuring).OnComplete(() => {
                if (currentIndex == lastCopyIndex) 
                {
                    currentIndex = 1;
                    scrollRect.horizontalNormalizedPosition =  contentSize;
                }
            });
            if (currentIndex == 0)
                UpdateDot(lastRealIndex);
            else if (currentIndex == lastCopyIndex)
                UpdateDot(1);
            else
                UpdateDot(currentIndex);

            
            Debug.Log("sliding " + currentIndex);
        }
    }
    private void OnSnap(PointerEventData eventData)
    {
        int index = Mathf.RoundToInt(scrollRect.horizontalNormalizedPosition/contentSize);
        scrollRect.DOHorizontalNormalizedPos(contentSize * index, snapDuring).OnComplete(()=>
        {
            if (index == 0) {
                currentIndex = lastRealIndex;
                scrollRect.horizontalNormalizedPosition = contentSize * currentIndex;
            } else if (index == contentCount - 1) {
                currentIndex = 1;
                scrollRect.horizontalNormalizedPosition = contentSize;
            } else {
                currentIndex = index;
            }
        });
        currentIndex = Mathf.Clamp(index, 1, lastRealIndex);
        
        if (index == 0)
            UpdateDot(lastRealIndex);
        else if (index == lastCopyIndex)
            UpdateDot(1);
        else
            UpdateDot(index);
    }
}
