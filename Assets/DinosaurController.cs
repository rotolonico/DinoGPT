using UnityEngine;

public class DinosaurController : MonoBehaviour
{
    public float jumpForce = 5f;
    private bool isJumping = false;
    private Rigidbody2D rb;
    
    public NeuralNetwork neuralNetwork;
    private CactusManager cactusManager;
    
    public float distanceTraveled = 0f;
    public bool isDead = false;
    public float fitnessScore = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cactusManager = FindObjectOfType<CactusManager>();
    }

    private void Update()
    {
        float[] inputs = GetNeuralNetworkInputs();
        float output = neuralNetwork.FeedForward(inputs)[0];
        ProcessOutput(output);
        
        
        // Update the distance traveled
        distanceTraveled += Time.deltaTime;
    }
    
    private float[] GetNeuralNetworkInputs()
    {
        float closestDistance = float.MaxValue;
        float nearestDistance = float.MaxValue;
        GameObject closestCactus = null;

        // Find the closest cactus
        foreach (Transform cactus in cactusManager.cactiContainer.transform)
        {
            float distance = Vector3.Distance(transform.position, cactus.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestCactus = cactus.gameObject;
            }

            if (distance < nearestDistance && cactus != closestCactus)
            {
                nearestDistance = distance;
            }
        }

        // Calculate the inputs for the neural network
        var distanceToClosestCactus = closestDistance;
        var closestCactusType = closestCactus != null ? closestCactus.GetComponent<Cactus>().cactusType : 0;
        var distanceToNearestCactus = nearestDistance;

        // Return the inputs as an array
        return new float[] { distanceToClosestCactus, closestCactusType, distanceToNearestCactus };
    }

    private void ProcessOutput(float output)
    {
        // Process the output value from the neural network
        // and control the behavior of the dinosaur accordingly
        if (output > 0.5f && !isJumping)
        {
            Jump();
        }
    }

    private void Jump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        isJumping = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
        }
    }
    
    public void Die()
    {
        isDead = true;
        fitnessScore = distanceTraveled * distanceTraveled;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;
        
        if (other.CompareTag("Cactus"))
        {
            // Handle collision with cactus
            Die();
            cactusManager.gameManager.DinoDied();
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
        }
    }

}