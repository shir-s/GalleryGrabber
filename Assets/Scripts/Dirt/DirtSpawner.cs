using System.Collections;
using Managers;
using UnityEngine;
using Utils;

namespace Dirt
{
    public class DirtSpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private float spawnRadius = 0.3f;
        [SerializeField] private float dirtSpawnInterval = 10f;
        [SerializeField] private int divideFactorMaxDirt = 5;

        [SerializeField] private Sprite[] dirtSprites;
        [SerializeField] private LayerMask forbiddenLayer;

        [Header("Room Reference")]
        [SerializeField] internal Collider2D roomCollider;

        private Coroutine _spawnRoutine;
        private int _initialDirtToSpawn;

        private void OnEnable()
        {
            GameEvents.StartLevel += StartSpawning;
        }

        private void OnDisable()
        {
            if (_spawnRoutine != null)
            {
                StopCoroutine(_spawnRoutine);
                _spawnRoutine = null;
            }

            GameEvents.StartLevel -= StartSpawning;
        }

        private void Start()
        {
            _initialDirtToSpawn = GameStates.MaxDirt / divideFactorMaxDirt;
        }

        private void StartSpawning()
        {
            if (_spawnRoutine != null)
            {
                StopCoroutine(_spawnRoutine);
            }

            SpawnInitialDirt();
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

        private void SpawnInitialDirt()
        {
            for (var i = 0; i < _initialDirtToSpawn; i++)
            {
                SpawnSingleDirt();
            }
        }

        private void SpawnSingleDirt()
        {
            var spawnPosition = FindValidPositionInRoom();

            if (spawnPosition == Vector2.zero) return;

            var dirt = DirtPool.Instance.Get();
            if (dirt == null) return;

            var spriteRenderer = dirt.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && dirtSprites.Length > 0)
            {
                var randomIndex = Random.Range(0, dirtSprites.Length);
                spriteRenderer.sprite = dirtSprites[randomIndex];
            }

            dirt.transform.position = spawnPosition;

            GameEvents.OnDirtSpawned?.Invoke();
        }

        private Vector2 FindValidPositionInRoom()
        {
            if (roomCollider == null) return Vector2.zero;
            var bounds = roomCollider.bounds;

            for (var i = 0; i < 10; i++)
            {
                var randomPos = new Vector2(
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
