﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map
{
    int YN, XN;
    float[,] grid;

    public Map(int xn, int yn)
    {
        XN = xn;
        YN = yn;

        grid = new float[XN, YN];
    }

    public Vector3 position
    {
        get
        {
            return new Vector3(-XN / 2, -YN / 2, 0);
        }
    }

    public Mesh generateMesh()
    {
        float[] values = { 0, 0.25f, 0.5f, 0.75f };
        var verts = new List<Vector3>();
        List<int>[] tris = {
            new List<int>(),
            new List<int>(),
            new List<int>(),
            new List<int>()
        };
        var uvs = new List<Vector2>();

        int offset = 0;
        for (int y = 0; y < YN; ++y)
        {
            for (int x = 0; x < XN; ++x)
            {
                int typeX = Random.Range(0, 4);
                int section = getSection(x, y);
                //   2 --- 3
                //   |     |
                //   |     |
                //   0 --- 1

                verts.Add(new Vector3(x, y));
                verts.Add(new Vector3(x + 1, y));
                verts.Add(new Vector3(x, y + 1));
                verts.Add(new Vector3(x + 1, y + 1));

                // tri 0
                tris[section].Add(offset + 0);
                tris[section].Add(offset + 2);
                tris[section].Add(offset + 1);

                // tri 1
                tris[section].Add(offset + 1);
                tris[section].Add(offset + 2);
                tris[section].Add(offset + 3);

                uvs.Add(new Vector2(0 + values[typeX], 0));
                uvs.Add(new Vector2(0.25f + values[typeX], 0));
                uvs.Add(new Vector2(0 + values[typeX], 1));
                uvs.Add(new Vector2(0.25f + values[typeX], 1));

                offset += 4;
            }
        }

        Mesh mesh = new Mesh();
        mesh.subMeshCount = 4;
        mesh.vertices = verts.ToArray();
        for (int i = 0; i < 4; ++i)
            mesh.SetTriangles(tris[i], i);
        mesh.uv = uvs.ToArray();
        return mesh;
    }

    public Texture2D generateNoiseMap(int seed)
    {
        var tex = new Texture2D(XN, YN);
        var colors = new Color[XN * YN];

        generateObstacleMap(seed);

        for (int y = 0; y < YN; ++y)
        {
            for (int x = 0; x < XN; ++x)
            {
                colors[y * XN + x] = new Color(grid[x, y], grid[x, y], grid[x, y]);

            }
        }

        tex.filterMode = FilterMode.Point;
        tex.SetPixels(colors);
        tex.Apply();

        return tex;
    }

    public float[,] generateObstacleMap(int seed)
    {
        System.Random prng = new System.Random(seed);
        grid = generateNoise(prng.Next());
        var samples = generateNoise(prng.Next());
        var samples2 = generateNoise(prng.Next());
        for (int y = 0; y < YN; ++y)
        {
            for (int x = 0; x < XN; ++x)
            {
                grid[x, y] = 1 - Mathf.Clamp01(grid[x, y] - samples[x, y] - samples2[x, y]);
            }
        }

        return grid;
    }

    public float[,] generateNoise(int seed = 0, float scale = 50, int octaves = 6, float persistance = 0.6f, float lacunarity = 2)
    {
        var samples = new float[XN, YN];
        scale = 2;
        octaves = 6;
        persistance = 0.6f;
        lacunarity = 3.41f;
        Vector2 offset = new Vector2(0, 0);

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        float maxPossibleHeight = 0;
        float amplitude = 1;
        float frequency = 1;

        Vector2 sampleCentre = new Vector2(0, 0);

        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x + sampleCentre.x;
            float offsetY = prng.Next(-100000, 100000) - offset.y - sampleCentre.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= persistance;
        }

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        float halfWidth = XN / 2f;
        float halfHeight = YN / 2f;

        for (int y = 0; y < YN; ++y)
        {
            for (int x = 0; x < XN; ++x)
            {
                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth + octaveOffsets[i].x) / scale * frequency;
                    float sampleY = (y - halfHeight + octaveOffsets[i].y) / scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxLocalNoiseHeight)
                {
                    maxLocalNoiseHeight = noiseHeight;
                }
                if (noiseHeight < minLocalNoiseHeight)
                {
                    minLocalNoiseHeight = noiseHeight;
                }
                samples[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < YN; y++)
        {
            for (int x = 0; x < XN; x++)
            {
                samples[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, samples[x, y]);
                if (samples[x, y] < 0.5)
                    samples[x, y] = 0;
                else
                    samples[x, y] = 1;
            }
        }

        return samples;
    }

    int getSection(int x, int y)
    {
        int section = 0;

        if (y < YN / 2 && x < XN / 2)
            section = 0;
        else if (y < YN / 2 && x >= XN / 2)
            section = 1;
        else if (y >= YN / 2 && x < XN / 2)
            section = 2;
        else if (y >= YN / 2 && x >= XN / 2)
            section = 3;

        return section;
    }

}
