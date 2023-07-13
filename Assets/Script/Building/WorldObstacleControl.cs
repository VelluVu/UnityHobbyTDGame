using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TheTD.Building
{
    /// <summary>
    /// Controls the obstacles of the world, differs from the buildings with nav mesh obstacles, which are created by player.
    /// </summary>
    public class WorldObstacleControl : MonoBehaviour
    {

        private const string PATH_TO_OBSTACLES_FOLDER = "Prefabs/Obstacles/";
        public static WorldObstacleControl Instance;
        public List<Obstacle> obstacles = new List<Obstacle>();
        public Transform obstacleHolder;

        public delegate void ObstacleDelegate(Obstacle obstacle);
        public static event ObstacleDelegate OnObstacleAdd;
        public static event ObstacleDelegate OnObstacleRemove;

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

        public void Start()
        {
            FindAllObstacles();
            AddListeners();
            InformObstaclePositions();
        }

        public void AddListeners()
        {

        }

        private void RemoveObstacleRuntime(Obstacle obstacle)
        {
            OnObstacleRemove.Invoke(obstacle);
        }

        private void AddObstacleRuntime(Vector3 position, Quaternion orientation, string obstacleName = "CubeObstacle")
        {
            //Create new world obstacle
            var obstaclePrefab = Resources.Load<GameObject>(PATH_TO_OBSTACLES_FOLDER + obstacleName);
            Obstacle obstacle = Instantiate(obstaclePrefab, position, orientation, obstacleHolder).GetComponent<Obstacle>();
            OnObstacleAdd?.Invoke(obstacle);
        }

        public void FindAllObstacles()
        {
            obstacles.Clear();
            obstacles = FindObjectsOfType<Obstacle>().ToList();
        }

        public void InformObstaclePositions()
        {
            obstacles.ForEach(o => o.InformPosition());
        }
    }
}