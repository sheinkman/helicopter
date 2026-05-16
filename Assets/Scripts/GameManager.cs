using System;
using UnityEngine;

namespace Skyjoust
{
    public enum GameState { Menu, Playing, Paused, GameOver }

    /// Central authority for run state: score, lives, waves and the player's lifecycle.
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("References")]
        [SerializeField] AircraftController playerPrefab;
        [SerializeField] Transform playerSpawn;
        [SerializeField] WaveSpawner waveSpawner;

        [Header("Rules")]
        [SerializeField] int startingLives = 3;
        [SerializeField] float respawnInvulnerability = 2.5f;

        public GameState State { get; private set; } = GameState.Menu;
        public int Score { get; private set; }
        public int Lives { get; private set; }
        public int Wave { get; private set; }
        public AircraftController Player { get; private set; }

        public event Action OnStatsChanged;
        public event Action<GameState> OnStateChanged;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        void Start()
        {
            SetState(GameState.Menu);
        }

        void Update()
        {
            if ((State == GameState.Playing || State == GameState.Paused) &&
                Input.GetKeyDown(KeyCode.P))
            {
                TogglePause();
            }
        }

        public void StartGame()
        {
            Score = 0;
            Lives = startingLives;
            Wave = 0;

            if (Player == null)
                Player = Instantiate(playerPrefab, playerSpawn.position, Quaternion.identity);

            Time.timeScale = 1f;
            SetState(GameState.Playing);
            Player.Respawn(playerSpawn.position, respawnInvulnerability);

            waveSpawner.ResetSpawner();
            StartNextWave();
            OnStatsChanged?.Invoke();
        }

        public void StartNextWave()
        {
            Wave++;
            waveSpawner.SpawnWave(Wave);
            OnStatsChanged?.Invoke();
        }

        /// Called by WaveSpawner once every drone in the wave is gone.
        public void NotifyWaveCleared()
        {
            if (State == GameState.Playing) StartNextWave();
        }

        public void AddScore(int amount)
        {
            Score += amount;
            OnStatsChanged?.Invoke();
        }

        public void AddLife()
        {
            Lives++;
            OnStatsChanged?.Invoke();
        }

        /// The player craft was destroyed (joust loss or lava).
        public void PlayerKilled()
        {
            Lives--;
            OnStatsChanged?.Invoke();

            if (Lives <= 0)
            {
                Player.gameObject.SetActive(false);
                SetState(GameState.GameOver);
            }
            else
            {
                Player.Respawn(playerSpawn.position, respawnInvulnerability);
            }
        }

        public void TogglePause()
        {
            if (State == GameState.Playing)
            {
                SetState(GameState.Paused);
                Time.timeScale = 0f;
            }
            else if (State == GameState.Paused)
            {
                Time.timeScale = 1f;
                SetState(GameState.Playing);
            }
        }

        void SetState(GameState next)
        {
            State = next;
            OnStateChanged?.Invoke(next);
        }
    }
}
