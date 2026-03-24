using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class PlayModeToggle:MonoBehaviour
{
        [SerializeField] private GameObject multiPanel;
        [SerializeField] private TextMeshProUGUI modeTxt;
        [SerializeField] private Slider slider;
        private float modeChangeDuration = 1f;
        private bool isMulti;

        public void OnToggleClick()
        {
                isMulti = !isMulti;
                if (isMulti) 
                {
                        slider.DOValue(1,modeChangeDuration);
                        modeTxt.text = "Multi";
                }
                else 
                {
                        slider.DOValue(0,modeChangeDuration);
                        modeTxt.text = "Single";
                }
        }

        public void OnStartClick()
        {
                if (isMulti) 
                {
                        multiPanel.SetActive(true);  
                }
                else 
                {
                        SceneManager.LoadScene(1);
                }
        }

        public void OnCloseClick()
        {
                multiPanel.SetActive(false);
        }
}

