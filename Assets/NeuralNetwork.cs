using System;

public class NeuralNetwork
{
    private int[] layers;
    private float[][] neurons;
    private float[][] biases;
    private float[][][] weights;

    public NeuralNetwork(int[] layers)
    {
        this.layers = new int[layers.Length];
        for (int i = 0; i < layers.Length; i++)
        {
            this.layers[i] = layers[i];
        }

        InitNeurons();
        InitBiases();
        InitWeights();
    }

    private void InitNeurons()
    {
        // Create neuron array
        neurons = new float[layers.Length][];

        // Fill neuron array with default values of 0
        for (int i = 0; i < layers.Length; i++)
        {
            neurons[i] = new float[layers[i]];
        }
    }

    private void InitBiases()
    {
        // Create bias array
        biases = new float[layers.Length][];

        // Fill bias array with random values between -1 and 1
        for (int i = 0; i < layers.Length; i++)
        {
            biases[i] = new float[layers[i]];
            for (int j = 0; j < layers[i]; j++)
            {
                biases[i][j] = UnityEngine.Random.Range(-1f, 1f);
            }
        }
    }

    private void InitWeights()
    {
        // Create weights array
        weights = new float[layers.Length - 1][][];

        // Fill weights array with random values between -1 and 1
        for (int i = 0; i < layers.Length - 1; i++)
        {
            weights[i] = new float[layers[i + 1]][];
            for (int j = 0; j < layers[i + 1]; j++)
            {
                weights[i][j] = new float[layers[i]];
                for (int k = 0; k < layers[i]; k++)
                {
                    weights[i][j][k] = UnityEngine.Random.Range(-1f, 1f);
                }
            }
        }
    }

    public float[] FeedForward(float[] inputs)
    {
        // Assign inputs to the first layer of neurons
        for (int i = 0; i < inputs.Length; i++)
        {
            neurons[0][i] = inputs[i];
        }

        // Perform feedforward propagation
        for (int i = 1; i < layers.Length; i++)
        {
            for (int j = 0; j < layers[i]; j++)
            {
                float value = 0f;
                for (int k = 0; k < layers[i - 1]; k++)
                {
                    value += weights[i - 1][j][k] * neurons[i - 1][k];
                }
                neurons[i][j] = ActivationFunction(value + biases[i][j]);
            }
        }

        // Return the output layer
        return neurons[layers.Length - 1];
    }

    private float ActivationFunction(float value)
    {
        // Use a sigmoid activation function
        return (float)(1f / (1f + Math.Exp(-value)));
    }

    public void Mutate(float mutationRate)
    {
        // Mutate the weights and biases of the neural network
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    if (UnityEngine.Random.value < mutationRate)
                    {
                        weights[i][j][k] += UnityEngine.Random.Range(-0.5f, 0.5f);
                    }
                }
            }
        }

        for (int i = 1; i < biases.Length; i++)
        {
            for (int j = 0; j < biases[i].Length; j++)
            {
                if (UnityEngine.Random.value < mutationRate)
                {
                    biases[i][j] += UnityEngine.Random.Range(-0.5f, 0.5f);
                }
            }
        }
    }

    public NeuralNetwork Clone()
    {
        // Create a deep copy of the neural network
        NeuralNetwork clone = new NeuralNetwork(layers);

        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                Array.Copy(weights[i][j], clone.weights[i][j], weights[i][j].Length);
            }
        }

        for (int i = 1; i < biases.Length; i++)
        {
            Array.Copy(biases[i], clone.biases[i], biases[i].Length);
        }

        return clone;
    }
}
