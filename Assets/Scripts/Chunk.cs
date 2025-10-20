using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using NoiseDotNet;
using System;
using Unity.Jobs;
using Unity.Collections;

public class Chunk : IDisposable
{
    int2 position;

    public ChunkState state;

    NativeArray<Voxel> voxelData;

    float[] xCoords;

    float[] yCoords;

    float[] output;

    float GetValueAt(int x, int y)
    {
        return output[y * ChunkDefs.ChunkSizeXZ + x];
    }

    JobHandle noiseJobHandle;

    public Chunk(bool geenrate)
    {
        xCoords = new float[ChunkDefs.ChunkArea];
        yCoords = new float[ChunkDefs.ChunkArea];
        output = new float[ChunkDefs.ChunkArea];

        int index = 0;
        for (int y = 0; y < ChunkDefs.ChunkSizeXZ; y++)
            for (int x = 0; x < ChunkDefs.ChunkSizeXZ; x++)
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

    // Will be deleted after implemetation in another script
    /*void Update()
    {
        if (noiseJobHandle.IsCompleted && state == ChunkState.Generating)
        {
            noiseJobHandle.Complete();
            state = ChunkState.ReadyForMesh;

            for (int i = 0; i < ChunkDefs.ChunkArea; i++)
            {
                GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                float height = Mathf.CeilToInt(output[i] * 10f);
                obj.transform.position = new Vector3(xCoords[i], height, yCoords[i]);

                var col = output[i];

                obj.GetComponent<MeshRenderer>().material.color = new Color(col, col, col);
            }
            noiseJobHandle = CreateGradient2DNoiseJob(xCoords, yCoords, output, 0.1f, 0.1f, 1f, 100).Schedule();
        }
    }*/

    public void Dispose()
    {
        
    }
}
