using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PlayPanel : MonoBehaviour
{
    private ToggleGroup toggleGroup;
    [SerializeField]private Toggle[]  toggles;
    [SerializeField]private CanvasGroup[]  panels;
    [SerializeField] private float fadeSpeed = 0.2f;

    private void Awake()
    {
        toggleGroup = GetComponent<ToggleGroup>();
    }
    void Start()
    {
        for (int i = 0; i < toggles.Length; i++) 
        {
            int index = i;
            toggles[i].onValueChanged.AddListener((isOn) => {
                if (isOn) 
                {
                    ReSetSlot(index);
                };
            });
        }
        
        toggles[0].isOn = true;
    }
    private void ReSetSlot(int index)
    {
        
        Sequence s = DOTween.Sequence();
        foreach (CanvasGroup t in panels) 
        {
            s.Join(t.DOFade(0,fadeSpeed));
        }
        
        s.onComplete = () =>
        {
            foreach (CanvasGroup t in panels) 
            {
                t.gameObject.SetActive(false);
            }
            CreateSlot(index);
        };
    }

    void CreateSlot(int index)
    {
        for(int i =0; i<= index; i++) 
        {
            panels[i].gameObject.SetActive(true);
            panels[i].DOFade(1, fadeSpeed);
        }
        
    }
}
