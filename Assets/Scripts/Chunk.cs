using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    static int3 chunkSize = new int3(16, 16, 384);

    int2 position = new int2(0, 0);

    List<Voxel> voxelData = new List<Voxel>();

    float[] xCoords = new float[chunkSize.x * chunkSize.y];

    float[] yCoords = new float[chunkSize.x * chunkSize.y];

    float[] output = new float[chunkSize.x * chunkSize.y];

    float GetValueAt(int x, int y)
    {
        return output[y * chunkSize.y + x];
    }

    void Start()
    {
        int index = 0;
        for (int y = 0; y < chunkSize.y; y++)
            for (int x = 0; x < chunkSize.x; x++)
            {
                xCoords[index] = x;
                yCoords[index] = y;
                index++;
            }

        NoiseDotNet.Noise.GradientNoise2D(
            xCoords: xCoords,
            yCoords: yCoords,
            output: output,
            xFreq: 0.1f, // x-coordinates are multiplied by this value before being used
            yFreq: 0.1f, // y-coordinates are multiplied by this value before being used
            amplitude: 1f, // the result of the noise function is multiplied by this value
            seed: 100);

        for (int i = 0; i < chunkSize.x * chunkSize.y; i++)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            float height = Mathf.CeilToInt(output[i] * 10f);
            obj.transform.position = new Vector3(xCoords[i], height, yCoords[i]);

            var col = output[i];

            obj.GetComponent<MeshRenderer>().material.color = new Color(col, col, col);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
