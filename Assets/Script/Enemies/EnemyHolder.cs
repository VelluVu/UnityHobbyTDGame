using System.Collections.Generic;
using System.Linq;
using TheTD.Spawning;
using UnityEngine;

namespace TheTD.Enemies
{
    public class EnemyHolder : MonoBehaviour
    {
        public static EnemyHolder Instance;

        private void Awake()
        {
            CheckSingleton();
        }

        private void CheckSingleton()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        private void Start()
        {
            SpawnersControl.Instance.OnEnemySpawn += OnEnemySpawned;
        }

        private void OnEnemySpawned(WaveState waveState, Enemy enemy)
        {
            enemy.transform.SetParent(transform);
        }

        public List<Enemy> GetConstructions()
        {
            return transform.GetComponentsInChildren<Enemy>().ToList();
        }
    }
}