using System.Collections.Generic;
using Utils;
using UnityEngine;

namespace Dirt
{
    public class DirtPool : MonoBehaviour
    {
        private string dirtPrefabName = "Dirt";
        [SerializeField] private int poolSize = 25;
        
        private GameObject dirtPrefab;
        private List<GameObject> availableDirt = new List<GameObject>();

        private void Awake()
        {
            dirtPrefab = Resources.Load<GameObject>(dirtPrefabName);
            if (dirtPrefab == null)
            {
                Debug.LogError("Could not load dirt prefab from Resources! Make sure the prefab is in a Resources folder and the name matches.");
                return;
            }
            CreatePool();
        }
        
        private void OnEnable()
        {
            GameEvents.RestartLevel += ResetPool;
        }
        private void OnDisable()
        {
            GameEvents.RestartLevel -= ResetPool;
        }

        private void CreatePool()
        {
            for (int i = 0; i < poolSize; i++)
            {
                GameObject dirt = Instantiate(dirtPrefab, transform);
                dirt.SetActive(false);
                availableDirt.Add(dirt);
            }
        }

        public GameObject GetDirt()
        {
            if (availableDirt.Count > 0)
            {
                GameObject dirt = availableDirt[0];
                availableDirt.RemoveAt(0);
                dirt.SetActive(true);
                return dirt;
            }
            else
            {
                Debug.LogWarning("No dirt available in the pool!");
                return null;
            }
        }

        public void ReturnDirt(GameObject dirt)
        {
            dirt.SetActive(false);
            availableDirt.Add(dirt);
        }

        public void ResetPool()
        {
            foreach (Transform dirt in transform)
            {
                dirt.gameObject.SetActive(false);
                if (!availableDirt.Contains(dirt.gameObject))
                    availableDirt.Add(dirt.gameObject);
            }
        }
    }
}