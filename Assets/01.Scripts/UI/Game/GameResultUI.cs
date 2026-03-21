
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameResultUI:MonoBehaviour
{
        [SerializeField] private GameObject resultPanel;
         private CanvasGroup canvasGroup;
        [SerializeField] private GameObject winText;
        [SerializeField] private GameObject loseText;

        private float fadeTiemr = 6f;

        private float timer = 3f;

        private void Awake()
        {
                canvasGroup = GetComponent<CanvasGroup>();
                winText.SetActive(false);
                loseText.SetActive(false);
                resultPanel.SetActive(false);
        }
        private void OnEnable()
        {
                GameManager.Instance.OnGameEnd += GameResultPanel;
        }
        private void OnDisable()
        {
                GameManager.Instance.OnGameEnd -= GameResultPanel;
        }
        private void GameResultPanel(GameManager.Team team)
        {
                Time.timeScale = 0.3f;
                resultPanel.SetActive(true);
                if (team == GameManager.Team.TeamA) 
                {
                        winText.SetActive(true);          
                }
                else 
                {
                        loseText.SetActive(true);
                }
                StartCoroutine(GameEndAnimaion());
        }
        private IEnumerator GameEndAnimaion()
        {
                yield return null;
                
                canvasGroup.DOFade(0.2f, fadeTiemr);
                yield return new WaitForSeconds(fadeTiemr);
                canvasGroup.DOFade(1, timer);
                
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                yield return new WaitForSeconds(timer);
                SceneManager.LoadScene(0);
        }

}