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
        [SerializeField] private Sprite [] dirtSprites;
        //private DirtPool _dirtPool;
        [SerializeField] internal Collider2D roomCollider;
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
            _initiallDirtToSpawn = GameManager.MaxDirt /5;
            //_dirtPool = GameManager.Instance.DirtPool;
            //GameManager.Instance.SetDirtSpawner(this);
        }
        
        private void StartSpawning()
        {
            if (_spawnRoutine != null)
            {
                StopCoroutine(_spawnRoutine);
            }
            InitialSpawn();
            _spawnRoutine = StartCoroutine(SpawnDirtOverTime());
        }
        
        private IEnumerator SpawnDirtOverTime()
        {
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
            Vector2 spawnPosition = FindValidPositionInRoom(roomCollider);

            if (spawnPosition != Vector2.zero)
            {
                //var dirt = _dirtPool.Get();
                var dirt = DirtPool.Instance.Get();
                //change dirt sprite
                int randomIndex = Random.Range(0, dirtSprites.Length);
                dirt.GetComponent<SpriteRenderer>().sprite = dirtSprites[randomIndex];
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