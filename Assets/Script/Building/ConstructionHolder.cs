using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TheTD.Building
{
    public class ConstructionHolder : MonoBehaviour
    {
        public static ConstructionHolder Instance;

        private void Awake()
        {
            CheckSingleton();
        }

        public void CheckSingleton()
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
            BuildArea.OnBuild += OnBuildingBuild;
        }

        private void OnBuildingBuild(Construction building)
        {
            building.transform.SetParent(transform);
        }

        public List<Construction> GetConstructions()
        {
            return transform.GetComponentsInChildren<Construction>().ToList();
        }
    }
}