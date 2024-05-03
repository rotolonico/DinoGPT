using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public int generationSize = 100;
    public GameObject dinoPrefab;

    private List<GameObject> dinosaurs = new List<GameObject>();
    private int deadCount = 0;

    private List<DinosaurData> currentGeneration = new List<DinosaurData>();

    private void Start()
    {
        SpawnDinosaurs();
    }

    private void SpawnDinosaurs()
    {
        GetComponent<CactusManager>().ClearCacti();

        // Destroy the existing dinosaurs and clear the list
        foreach (GameObject dino in dinosaurs)
        {
            Destroy(dino);
        }

        dinosaurs.Clear();

        for (int i = 0; i < generationSize; i++)
        {
            GameObject dino = Instantiate(dinoPrefab, transform.position, Quaternion.identity);
            NeuralNetwork neuralNetwork = new NeuralNetwork(new int[] { 3, 1, 10 });
            dino.GetComponent<DinosaurController>().neuralNetwork = neuralNetwork;
            dinosaurs.Add(dino);
        }
    }

    private void CheckAllDead()
    {
        deadCount++;
        if (deadCount == generationSize)
        {
            // Calculate fitness scores for each dinosaur based on their performance.
            foreach (GameObject dino in dinosaurs)
            {
                // Get the DinosaurController component
                DinosaurController dinoController = dino.GetComponent<DinosaurController>();

                // Calculate the fitness score based on the dinosaur's performance
                float fitnessScore = dinoController.fitnessScore;

                // Create a new DinosaurData object with the fitness score and neural network
                currentGeneration.Add(new DinosaurData(fitnessScore, dinoController.neuralNetwork));
            }

            // Sort the current generation by fitness score (in descending order).
            currentGeneration.Sort((a, b) => b.fitnessScore.CompareTo(a.fitnessScore));

            // Perform selection, reproduction, mutation, and speciation to create the next generation.
            List<DinosaurData> nextGeneration = new List<DinosaurData>();

            // Keep the top performers (e.g., top 30%) and pass them directly to the next generation.
            int topPerformersCount = Mathf.RoundToInt(generationSize * 0.01f);
            for (int i = 0; i < topPerformersCount; i++)
            {
                DinosaurData topDino = currentGeneration[i];
                nextGeneration.Add(new DinosaurData(topDino.fitnessScore, topDino.neuralNetwork));
            }

            // Generate offspring through crossover and mutation for the remaining slots in the next generation.
            for (int i = topPerformersCount; i < generationSize; i++)
            {
                NeuralNetwork offspringNetwork = PerformCrossover();
                PerformMutation(offspringNetwork);
                nextGeneration.Add(new DinosaurData(0f, offspringNetwork));
            }

            // Replace the current generation with the next generation.
            currentGeneration = nextGeneration;

            // Once the next generation is ready, reset the dead count and spawn the new dinosaurs.
            deadCount = 0;
            SpawnDinosaurs();
        }
    }

    public void DinoDied()
    {
        CheckAllDead();
    }

    // Method to perform crossover between two parent neural networks
    private NeuralNetwork PerformCrossover()
    {
        // Select two parent networks for crossover
        DinosaurData parent1 = SelectParent();
        DinosaurData parent2 = SelectParent();

        // Get the neural networks from the parent dinosaurs
        NeuralNetwork network1 = parent1.neuralNetwork;
        NeuralNetwork network2 = parent2.neuralNetwork;

        // Create a new child network for crossover (same as the selected parent)
        NeuralNetwork childNetwork = network1.Clone();

        // Perform mutation on the child network
        PerformMutation(childNetwork);

        return childNetwork;
    }

// Method to perform mutation on a neural network
    private void PerformMutation(NeuralNetwork network)
    {
        var mutationRate = 0.05f;
        if (Random.Range(0f, 1f) < mutationRate) network.Mutate(mutationRate);
    }

// Method to select a parent for crossover using a fitness-based selection algorithm (e.g., roulette wheel selection)
    private DinosaurData SelectParent()
    {
        float totalFitness = 0f;

        // Calculate the total fitness of the current generation
        foreach (DinosaurData dinosaur in currentGeneration)
        {
            totalFitness += dinosaur.fitnessScore;
        }

        // Perform roulette wheel selection
        float randomFitness = Random.Range(0f, totalFitness);
        float cumulativeFitness = 0f;

        foreach (DinosaurData dinosaur in currentGeneration)
        {
            cumulativeFitness += dinosaur.fitnessScore;

            if (cumulativeFitness >= randomFitness)
            {
                return dinosaur;
            }
        }

        // Fallback case (shouldn't be reached)
        return currentGeneration[0];
    }
}

public class DinosaurData
{
    public float fitnessScore;
    public NeuralNetwork neuralNetwork;

    public DinosaurData(float fitnessScore, NeuralNetwork neuralNetwork)
    {
        this.fitnessScore = fitnessScore;
        this.neuralNetwork = neuralNetwork;
    }
}