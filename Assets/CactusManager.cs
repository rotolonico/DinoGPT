using UnityEngine;

public class CactusManager : MonoBehaviour
{
    public GameManager gameManager;

    
    public GameObject[] cactusPrefabs;
    public Transform playerTransform;
    public float spawnInterval = 2f;
    public float spawnOffset = 5f;
    public float randomOffsetRange = 2f;
    public float cactusSpeed = 5f;

    public GameObject cactiContainer;
    private float playerPositionX;

    private Vector3 dinoStartPosition;

    private void Start()
    {
        cactiContainer = new GameObject("CactiContainer");
        InvokeRepeating("SpawnCactus", spawnInterval, spawnInterval);
        gameManager = FindObjectOfType<GameManager>();
        dinoStartPosition = playerTransform.position;
    }

    private void Update()
    {
        MoveCacti();
    }

    private void SpawnCactus()
    {
        GameObject cactusPrefab = cactusPrefabs[Random.Range(0, cactusPrefabs.Length)];

        // Get the height of the cactus prefab
        float cactusHeight = cactusPrefab.GetComponent<SpriteRenderer>().bounds.extents.y;

        Vector3 spawnPosition = new Vector3(dinoStartPosition.x + spawnOffset, dinoStartPosition.y - cactusHeight, dinoStartPosition.z);
        GameObject newCactus = Instantiate(cactusPrefab, spawnPosition, Quaternion.identity);
        newCactus.transform.parent = cactiContainer.transform;
    }
    
    private void MoveCacti()
    {
        foreach (Transform cactus in cactiContainer.transform)
        {
            cactus.Translate(Vector3.left * cactusSpeed * Time.deltaTime);

            if (cactus.position.x < dinoStartPosition.x - spawnOffset - randomOffsetRange)
            {
                Destroy(cactus.gameObject);
            }
        }
    }

    public void ClearCacti()
    {
        foreach (Transform cactus in cactiContainer.transform)
        {
            Destroy(cactus.gameObject);
        }
    }
}