using System.Collections.Generic;
using System.Linq;
using TheTD.Spawning;
using UnityEngine;

namespace TheTD.Enemies
{
    public class EnemyHolder : MonoBehaviour, IEventListener
    {
        public static EnemyHolder Instance;
        public List<Enemy> Enemies { get => GetEnemies(); }

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
            AddListeners();
        }

        public void AddListeners()
        {
            SpawnersControl.Instance.OnEnemySpawn += OnEnemySpawned;
        }

        public void RemoveListeners()
        {
            SpawnersControl.Instance.OnEnemySpawn -= OnEnemySpawned;
        }

        private void OnEnemySpawned(WaveState waveState, Enemy enemy)
        {
            enemy.transform.SetParent(transform);
        }

        private List<Enemy> GetEnemies()
        {
            return transform.GetComponentsInChildren<Enemy>().ToList();
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }
    }
}