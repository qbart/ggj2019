using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map
{
    int YN, XN;

    public struct Data
    {
        public float[,] grid;
        public int XN, YN;
    }

    [System.Serializable]
    public struct Range
    {
        public float a;
        public float b;
        public float value;

        public Range(float _a, float _b, float _value)
        {
            a = _a;
            b = _b;
            value = _value;
        }

        public bool isIn(float x)
        {
            return a <= x && x < b;
        }
    }

    public Map(int xn, int yn)
    {
        XN = xn;
        YN = yn;
    }

    public Vector3 position
    {
        get
        {
            return new Vector3(0, 0, 0);
        }
    }

    public Mesh generateMesh(int seed)
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

        var toShuffle = new List<int>() { 0, 1, 2, 3 };
        var shuffled = new List<int>() { };

        while (toShuffle.Count > 0)
        {
            int index = Random.Range(0, toShuffle.Count);
            shuffled.Add(toShuffle[index]);
            toShuffle.RemoveAt(index);
        }
        int[] shuffledType = shuffled.ToArray();

        var samples = generateTerrainMap(seed, new Range[] {
            new Range(0, 0.3f, shuffledType[0]),
            new Range(0.3f, 0.5f, shuffledType[1]),
            new Range(0.5f, 0.7f, shuffledType[2]),
            new Range(0.7f, 1.1f, shuffledType[3])
        }, 20, 3, 0, 5.6f);

        int offset = 0;
        for (int y = 0; y < YN; ++y)
        {
            for (int x = 0; x < XN; ++x)
            {
                int typeX = Random.Range(0, 4);
                int section = (int)samples.grid[x, y];
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

    public Data smoothMap(Range[] ranges, Data data)
    {
        for (int y = 0; y < data.YN; ++y)
        {
            for (int x = 0; x < data.XN; ++x)
            {
                foreach (var r in ranges)
                {
                    if (r.isIn(data.grid[x, y]))
                    {
                        data.grid[x, y] = r.value;
                        break;
                    }
                }
            }
        }

        return data;
    }

    public Texture2D generateNoiseMap(int seed, Range[] ranges, float scale, int octaves, float persistance, float lacunarity)
    {
        var tex = new Texture2D(XN, YN);
        var colors = new Color[XN * YN];

        var data = generateNoise(seed, ranges, scale, octaves, persistance, lacunarity);
        // data = smoothMap(ranges, data);

        for (int y = 0; y < YN; ++y)
        {
            for (int x = 0; x < XN; ++x)
            {
                colors[y * XN + x] = new Color(1 - data.grid[x, y], 1 - data.grid[x, y], 1 - data.grid[x, y]);
            }
        }

        tex.filterMode = FilterMode.Point;
        tex.SetPixels(colors);
        tex.Apply();

        return tex;
    }

    public Data generateTerrainMap(int seed, Range[] ranges, float scale, int octaves, float persistance, float lacunarity)
    {
        var samples0 = generateNoise(seed, ranges, scale, octaves, persistance, lacunarity);
        samples0 = smoothMap(ranges, samples0);
        return samples0;
    }

    public Data generateMap(int seed, Range[] ranges, float scale, int octaves, float persistance, float lacunarity)
    {
        System.Random prng = new System.Random(seed);
        var samples0 = generateNoise(prng.Next(), ranges, scale, octaves, persistance, lacunarity);
        var samples1 = generateNoise(prng.Next(), ranges, scale, octaves, persistance, lacunarity);
        var samples2 = generateNoise(prng.Next(), ranges, scale, octaves, persistance, lacunarity);

        Data data;
        data.grid = new float[XN, YN];
        data.YN = YN;
        data.XN = XN;

        for (int y = 0; y < YN; ++y)
        {
            for (int x = 0; x < XN; ++x)
            {
                data.grid[x, y] = Mathf.Clamp01(
                    hardValue(samples0.grid[x, y], 0.5f)
                     - hardValue(samples1.grid[x, y], 0.5f)
                     - hardValue(samples2.grid[x, y], 0.5f)
                     );
            }
        }

        return data;
    }

    public Data generateNoise(int seed, Range[] ranges, float scale, int octaves, float persistance, float lacunarity)
    {
        Data samples;
        samples.grid = new float[XN, YN];
        samples.YN = YN;
        samples.XN = XN;
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
                samples.grid[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < YN; y++)
        {
            for (int x = 0; x < XN; x++)
            {
                samples.grid[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, samples.grid[x, y]);
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

    float hardValue(float value, float mid)
    {
        if (value < mid)
            return 0;
        else
            return 1;
    }

}
