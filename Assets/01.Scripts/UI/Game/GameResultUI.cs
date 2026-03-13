
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameResultUI:MonoBehaviour
{
        [SerializeField] private GameObject winPanel;
        [SerializeField] private GameObject losePanel;

        private float timer = 3f;

        private void Awake()
        {
                winPanel.SetActive(false);
                losePanel.SetActive(false);
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
                if (team == GameManager.Team.TeamA) 
                {
                        winPanel.SetActive(true);          
                }
                else 
                {
                        losePanel.SetActive(true);
                }
                StartCoroutine(GameEndAnimaion());
        }
        private IEnumerator GameEndAnimaion()
        {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                yield return new WaitForSeconds(timer);
                SceneManager.LoadScene(0);
        }

}