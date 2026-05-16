using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skyjoust
{
    /// Spawns waves of enemy drones from a set of spawn points and reports back
    /// to the GameManager when a wave is fully cleared.
    public class WaveSpawner : MonoBehaviour
    {
        [Header("Setup")]
        [SerializeField] EnemyDrone dronePrefab;
        [SerializeField] Transform[] spawnPoints;

        [Header("Tuning")]
        [SerializeField] int baseDrones = 3;
        [SerializeField] int maxDrones = 8;
        [SerializeField] float spawnInterval = 0.6f;

        readonly List<EnemyDrone> active = new List<EnemyDrone>();
        int aliveCount;

        public int AliveCount => aliveCount;

        public void ResetSpawner()
        {
            StopAllCoroutines();
            foreach (EnemyDrone d in active)
                if (d != null) Destroy(d.gameObject);
            active.Clear();
            aliveCount = 0;
        }

        public void SpawnWave(int wave)
        {
            int count = Mathf.Min(baseDrones + wave - 1, maxDrones);
            aliveCount = count;
            StartCoroutine(SpawnRoutine(count, wave));
        }

        IEnumerator SpawnRoutine(int count, int wave)
        {
            for (int i = 0; i < count; i++)
            {
                Transform sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
                int tier = wave > 6 ? 2 : wave > 3 ? 1 : 0;

                EnemyDrone drone = Instantiate(dronePrefab, sp.position, Quaternion.identity);
                drone.Initialize(tier, this);
                active.Add(drone);

                yield return new WaitForSeconds(spawnInterval);
            }
        }

        public void NotifyDroneDestroyed(EnemyDrone drone)
        {
            active.Remove(drone);
            aliveCount--;
            if (aliveCount <= 0)
                GameManager.Instance.NotifyWaveCleared();
        }
    }
}
