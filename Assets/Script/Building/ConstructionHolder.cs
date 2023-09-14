using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TheTD.Building
{
    public class ConstructionHolder : MonoBehaviour, IEventListener
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

        public List<Construction> GetConstructions()
        {
            return transform.GetComponentsInChildren<Construction>().ToList();
        }

        public void AddListeners()
        {
            BuildArea.OnBuild += OnBuildingBuild;
        }

        public void RemoveListeners()
        {
            BuildArea.OnBuild -= OnBuildingBuild;
        }

        private void Start()
        {
            AddListeners();    
        }

        private void OnBuildingBuild(Construction building)
        {
            building.transform.SetParent(transform);
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }
    }
}