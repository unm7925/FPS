using DG.Tweening;
using Mirror;
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
                        NetworkManager.singleton.StartHost();
                        NetworkManager.singleton.ServerChangeScene("InGameScene");
                }
        }

        public void OnCloseClick()
        {
                multiPanel.SetActive(false);
        }
}

