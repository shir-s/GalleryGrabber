using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Dirt
{
    public class DirtSpawner : MonoBehaviour
    {
        [SerializeField] private LayerMask forbiddenLayer; 
        [SerializeField] private float spawnRadius = 0.3f;
        [SerializeField] private float dirtSpawnInterval = 10f;
        
        private DirtPool _dirtPool;
        private Collider2D _roomCollider;
        private Coroutine _spawnRoutine;
        private int _initiallDirtToSpawn;

        /*public void SetUp(DirtPool dirtPool, Collider2D roomCollider)
        {
            _dirtPool = dirtPool;
            _roomCollider = roomCollider;
        }*/
        private void OnEnable()
        {
            GameEvents.StartLevel += StartSpawning;
        }

        private void OnDisable()
        {
            if (_spawnRoutine != null)
            {
                StopCoroutine(_spawnRoutine);
            }
            GameEvents.StartLevel -= StartSpawning;
        }

        private void Start()
        {
            _initiallDirtToSpawn = GameManager.Instance.maxDirt /2;
            _roomCollider = GameManager.Instance.roomCollider;
            _dirtPool = GameManager.Instance.DirtPool;
        }
        
        private void StartSpawning()
        {
            _spawnRoutine = StartCoroutine(SpawnDirtOverTime());
        }
        
        private IEnumerator SpawnDirtOverTime()
        {
            // מחכה לפני ספאון ראשון (אפשר לשים 0 אם לא צריך)
            yield return new WaitForSeconds(dirtSpawnInterval);

            while (true)
            {
                SpawnSingleDirt();
                yield return new WaitForSeconds(dirtSpawnInterval);
            }
        }

        public void InitialSpawn()
        {
            //this._roomCollider = GameManager.Instance.roomCollider;

            for (int i = 0; i < _initiallDirtToSpawn; i++)
            {
                SpawnSingleDirt();
            }
        }

        private void SpawnSingleDirt()
        {
            Vector2 spawnPosition = FindValidPositionInRoom(_roomCollider);

            if (spawnPosition != Vector2.zero)
            {
                var dirt = _dirtPool.Get();
                if (dirt != null)
                {
                    dirt.transform.position = spawnPosition;
                }
            }
            else
            {
                Debug.LogWarning("No valid position found for dirt.");
            }
            GameEvents.OnDirtSpawned?.Invoke();
        }

        private Vector2 FindValidPositionInRoom(Collider2D roomCollider)
        {
            if (roomCollider == null)
            {
                Debug.LogWarning("Room has no collider!");
                return Vector2.zero;
            }

            Bounds bounds = roomCollider.bounds;

            for (int i = 0; i < 10; i++)
            {
                Vector2 randomPos = new Vector2(
                    Random.Range(bounds.min.x, bounds.max.x),
                    Random.Range(bounds.min.y, bounds.max.y)
                );

                if (!Physics2D.OverlapCircle(randomPos, spawnRadius, forbiddenLayer))
                {
                    return randomPos;
                }
            }

            return Vector2.zero; 
        }
    }
}