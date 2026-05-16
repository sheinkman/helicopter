using UnityEngine;
using UnityEngine.UI;

namespace Skyjoust
{
    /// Drives the on-screen HUD and the menu / pause / game-over panels.
    /// Wire the button OnClick events to the public On*Button methods.
    public class HUDController : MonoBehaviour
    {
        [Header("In-game HUD")]
        [SerializeField] Text scoreText;
        [SerializeField] Text waveText;
        [SerializeField] Text livesText;

        [Header("Panels")]
        [SerializeField] GameObject menuPanel;
        [SerializeField] GameObject pausePanel;
        [SerializeField] GameObject gameOverPanel;
        [SerializeField] Text finalScoreText;

        void Start()
        {
            GameManager gm = GameManager.Instance;
            gm.OnStatsChanged += RefreshStats;
            gm.OnStateChanged += HandleState;
            HandleState(gm.State);
            RefreshStats();
        }

        void OnDestroy()
        {
            if (GameManager.Instance == null) return;
            GameManager.Instance.OnStatsChanged -= RefreshStats;
            GameManager.Instance.OnStateChanged -= HandleState;
        }

        void RefreshStats()
        {
            GameManager gm = GameManager.Instance;
            if (scoreText != null) scoreText.text = "SCORE  " + gm.Score;
            if (waveText != null) waveText.text = "WAVE  " + gm.Wave;
            if (livesText != null) livesText.text = "CRAFT  " + gm.Lives;
        }

        void HandleState(GameState state)
        {
            if (menuPanel != null) menuPanel.SetActive(state == GameState.Menu);
            if (pausePanel != null) pausePanel.SetActive(state == GameState.Paused);
            if (gameOverPanel != null) gameOverPanel.SetActive(state == GameState.GameOver);

            if (state == GameState.GameOver && finalScoreText != null)
            {
                finalScoreText.text = "SCORE  " + GameManager.Instance.Score +
                                      "\nWAVE  " + GameManager.Instance.Wave;
            }
        }

        // --- hook these to UI Button OnClick events ---
        public void OnStartButton() => GameManager.Instance.StartGame();
        public void OnRestartButton() => GameManager.Instance.StartGame();
        public void OnResumeButton() => GameManager.Instance.TogglePause();
    }
}
