using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using NoiseDotNet;
using System;
using Unity.Jobs;

public class Chunk : MonoBehaviour
{
    static int3 chunkSize = new int3(16, 384, 16);

    int2 position = new int2(0, 0);

    List<Voxel> voxelData = new List<Voxel>();

    float[] xCoords = new float[chunkSize.x * chunkSize.z];

    float[] yCoords = new float[chunkSize.x * chunkSize.z];

    float[] output = new float[chunkSize.x * chunkSize.z];

    float GetValueAt(int x, int y)
    {
        return output[y * chunkSize.z + x];
    }

    JobHandle noiseJobHandle;
    bool isGenerated;

    void Start()
    {
        int index = 0;
        for (int y = 0; y < chunkSize.z; y++)
            for (int x = 0; x < chunkSize.x; x++)
            {
                xCoords[index] = x;
                yCoords[index] = y;
                index++;
            }

        var noiseJob = CreateGradient2DNoiseJob(xCoords, yCoords, output, 0.1f, 0.1f, 1f, 100);
        noiseJobHandle = noiseJob.Schedule();
    }

    unsafe BurstNoiseJob CreateGradient2DNoiseJob(ReadOnlySpan<float> x, ReadOnlySpan<float> y, Span<float> output, float xFreq, float yFreq, float amplitude, int seed)
    {
        if (output.Length == 0)
            throw new ArgumentException($"Output buffer length was 0. Expected > 0.");
        if (output.Length != x.Length)
            throw new ArgumentException($"Expected x buffer length {x.Length} to equal output buffer length {output.Length}");
        if (output.Length != y.Length)
            throw new ArgumentException($"Expected y buffer length {y.Length} to equal output buffer length {output.Length}");

        fixed (float* xPtr = x)
        {
            fixed (float* yPtr = y)
            {
                fixed (float* outPtr = output)
                {
                    BurstNoiseJob job = new();
                    job.noiseType = NoiseType.GradientNoise2D;
                    job.xBuffer = xPtr;
                    job.yBuffer = yPtr;
                    job.output1Buffer = outPtr;
                    job.length = x.Length;
                    job.amplitude1 = amplitude;
                    job.xFrequency = xFreq;
                    job.yFrequency = yFreq;
                    job.seed = seed;
                    return job;
                }
            }
        }
    }

    void Update()
    {
        if (noiseJobHandle.IsCompleted && !isGenerated)
        {
            noiseJobHandle.Complete();
            isGenerated = true;

            for (int i = 0; i < chunkSize.x * chunkSize.z; i++)
            {
                GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                float height = Mathf.CeilToInt(output[i] * 10f);
                obj.transform.position = new Vector3(xCoords[i], height, yCoords[i]);

                var col = output[i];

                obj.GetComponent<MeshRenderer>().material.color = new Color(col, col, col);
            }
            noiseJobHandle = CreateGradient2DNoiseJob(xCoords, yCoords, output, 0.1f, 0.1f, 1f, 100).Schedule();
        }
    }
}
