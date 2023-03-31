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
        public List<Obstacle> obstacles = new List<Obstacle>();

        public delegate void ObstacleDelegate();
        public static event ObstacleDelegate OnObstacleAdd;
        public static event ObstacleDelegate OnObstacleRemove;

        public void Start()
        {
            FindAllObstacles();
            AddListeners();
            InformObstaclePositions();
        }

        public void AddListeners()
        {

        }

        private void AddObstacleRuntime()
        {
            //Create new world obstacle
            OnObstacleAdd?.Invoke();
        }

        private void OnPathBlocked(Vector3 position)
        {
            DestroyClosestObstacle(position);
        }

        private void DestroyClosestObstacle(Vector3 position)
        {
            var obstacle = FindClosestObstacle(position);
            obstacles.Remove(obstacle);
            OnObstacleRemove?.Invoke();
            Destroy(obstacle.gameObject);
            obstacles.TrimExcess();
        }

        public Obstacle FindClosestObstacle(Vector3 position)
        {
            var closest = obstacles[0];

            for (int i = 0; i < obstacles.Count; i++)
            {
                var shortestDistance = Vector3.Distance(position, closest.transform.position);
                var distanceToCurrentObstacle = Vector3.Distance(position, obstacles[i].transform.position);

                if (distanceToCurrentObstacle < shortestDistance)
                {
                    closest = obstacles[i];
                }
            }

            return closest;
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