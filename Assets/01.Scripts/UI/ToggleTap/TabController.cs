using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TabController : MonoBehaviour
{
    private ToggleGroup toggleGroup;
    [SerializeField] private Toggle[] toggles;
    [SerializeField] private CanvasGroup[] panels;

    private void Awake()
    {
        toggleGroup = gameObject.GetComponent<ToggleGroup>();
    }
    void Start()
    {
        for (int i = 0; i < toggles.Length; i++) 
        {
            int index = i;
            toggles[i].onValueChanged.AddListener((isOn)=>
            {
                if(isOn) 
                {
                    Show(index);
                }
                else 
                {
                    Hide(index);
                }
            });
        }
    }

    public void OnClick()
    {
        toggleGroup.SetAllTogglesOff();
    }
    
    private void Show(int _index)
    {
        panels[_index].blocksRaycasts = true;
        panels[_index].DOFade(1, 0.5f);
    }
    private void Hide(int _index)
    {
        
        panels[_index].DOFade(0, 0.5f);
        panels[_index].blocksRaycasts = false;
    }
}
