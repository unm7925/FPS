using System;
using TMPro;
using UnityEngine;
public class CountdownUI:MonoBehaviour
{
        [SerializeField] RoundManager roundManager;
        [SerializeField] TextMeshProUGUI countdownText;

        private void OnEnable()
        {
                roundManager.OnCountdown += HandleCountdown;
                roundManager.OnRoundReady += HandleRoundReady;
        }

        private void OnDisable()
        {
                roundManager.OnCountdown -= HandleCountdown;
                roundManager.OnRoundReady -= HandleRoundReady;
        }
        private void HandleRoundReady()
        {
                countdownText.gameObject.SetActive(false);
        }
        private void HandleCountdown(int countdown)
        {
                countdownText.gameObject.SetActive(true);
                countdownText.text = countdown.ToString();
        }
}

