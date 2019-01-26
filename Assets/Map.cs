using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map
{
    int YN, XN;

    public class Level
    {
        public Mesh mesh;
        public Data data;
    }

    public class Matrix
    {
        public int[] v;

        public Matrix(
            int v1 = 0, int v2 = 0, int v3 = 0,
            int v4 = 0, int v5 = 0, int v6 = 0,
            int v7 = 0, int v8 = 0, int v9 = 0
            )
        {
            v = new int[9] {
            0, 0, 0,
            0, 0, 0,
            0, 0, 0
            };

            v[0] = v1;
            v[1] = v2;
            v[2] = v3;
            v[3] = v4;
            v[4] = v5;
            v[5] = v6;
            v[6] = v7;
            v[7] = v8;
            v[8] = v9;
        }


        public bool matches(
            int v1 = 0, int v2 = 0, int v3 = 0,
            int v4 = 0, int v5 = 0, int v6 = 0,
            int v7 = 0, int v8 = 0, int v9 = 0)
        {
            var tmp = new Matrix();
            tmp.v[0] = v1;
            tmp.v[1] = v2;
            tmp.v[2] = v3;
            tmp.v[3] = v4;
            tmp.v[4] = v5;
            tmp.v[5] = v6;
            tmp.v[6] = v7;
            tmp.v[7] = v8;
            tmp.v[8] = v9;



            for (int i = 0; i < v.Length; ++i)
                if (tmp.v[i] == 1 && tmp.v[i] != v[i])
                    return false;

            return true;
        }
    }

    public struct Data
    {
        public float[,] grid;
        public int XN, YN;

        public float this[int x, int y]
        {
            get
            {
                if (x < 0 || x >= XN || y < 0 || y >= YN)
                    return -1;

                return grid[x, y];
            }
        }

        public Matrix around(int x, int y, int s)
        {
            var mat = new Matrix();

            mat.v[0] = this[x - 1, y + 1] == s ? 1 : 0;
            mat.v[1] = this[x, y + 1] == s ? 1 : 0;
            mat.v[2] = this[x + 1, y + 1] == s ? 1 : 0;

            mat.v[3] = this[x - 1, y] == s ? 1 : 0;
            mat.v[4] = this[x, y] == s ? 1 : 0;
            mat.v[5] = this[x + 1, y] == s ? 1 : 0;

            mat.v[6] = this[x - 1, y - 1] == s ? 1 : 0;
            mat.v[7] = this[x, y - 1] == s ? 1 : 0;
            mat.v[8] = this[x + 1, y - 1] == s ? 1 : 0;

            return mat;
        }
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

    public Level generateLevel(int seed)
    {
        Level level = new Level();

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

                uvs.Add(new Vector2(0 + values[typeX], 0.75f));
                uvs.Add(new Vector2(0.25f + values[typeX], 0.75f));
                uvs.Add(new Vector2(0 + values[typeX], 1));
                uvs.Add(new Vector2(0.25f + values[typeX], 1));

                offset += 4;
            }
        }

        for (int i = 0; i < shuffledType.Length; ++i)
        {
            int s = shuffledType[i];

            for (int y = 0; y < YN; ++y)
                for (int x = 0; x < XN; ++x)
                {
                    if (samples[x, y] == s)
                        continue;

                    float frame = -1;
                    Matrix mat = samples.around(x, y, s);

                    if (mat.matches(
                        0, 0, 0,
                        0, 0, 1,
                        0, 1, 1
                    )) frame = 13;
                    else if (mat.matches(
                        0, 0, 0,
                        1, 0, 0,
                        1, 1, 0
                    )) frame = 12;
                    else if (mat.matches(
                        1, 1, 0,
                        1, 0, 0,
                        0, 0, 0
                    )) frame = 15;
                    else if (mat.matches(
                        0, 1, 1,
                        0, 0, 1,
                        0, 0, 0
                    )) frame = 14;

                    else if (mat.matches(
                        0, 0, 0,
                        1, 0, 0,
                        0, 0, 0
                    )) frame = 8;
                    else if (mat.matches(
                        0, 1, 0,
                        0, 0, 0,
                        0, 0, 0
                    )) frame = 11;
                    else if (mat.matches(
                        0, 0, 0,
                        0, 0, 1,
                        0, 0, 0
                    )) frame = 10;
                    else if (mat.matches(
                        0, 0, 0,
                        0, 0, 0,
                        0, 1, 0
                    )) frame = 9;

                    else if (mat.matches(
                        1, 0, 0,
                        0, 0, 0,
                        0, 0, 0
                    )) frame = 7;
                    else if (mat.matches(
                        0, 0, 1,
                        0, 0, 0,
                        0, 0, 0
                    )) frame = 6;
                    else if (mat.matches(
                        0, 0, 0,
                        0, 0, 0,
                        0, 0, 1
                    )) frame = 5;
                    else if (mat.matches(
                        0, 0, 0,
                        0, 0, 0,
                        1, 0, 0
                    )) frame = 4;


                    if (frame != -1)
                    {
                        verts.Add(new Vector3(x, y, -5));
                        verts.Add(new Vector3(x + 1, y, -5));
                        verts.Add(new Vector3(x, y + 1, -5));
                        verts.Add(new Vector3(x + 1, y + 1, -5));

                        // tri 0
                        tris[s].Add(offset + 0);
                        tris[s].Add(offset + 2);
                        tris[s].Add(offset + 1);

                        // tri 1
                        tris[s].Add(offset + 1);
                        tris[s].Add(offset + 2);
                        tris[s].Add(offset + 3);

                        int frameX = (int)frame % 4;
                        int frameY = (int)frame / 4;

                        uvs.Add(new Vector2(0 + values[frameX], 0 + values[3 - frameY]));
                        uvs.Add(new Vector2(0.25f + values[frameX], 0 + values[3 - frameY]));
                        uvs.Add(new Vector2(0 + values[frameX], 0.25f + values[3 - frameY]));
                        uvs.Add(new Vector2(0.25f + values[frameX], 0.25f + values[3 - frameY]));

                        offset += 4;

                    }
                }

        }

        Mesh mesh = new Mesh();
        mesh.subMeshCount = 4;
        mesh.vertices = verts.ToArray();
        for (int i = 0; i < 4; ++i)
            mesh.SetTriangles(tris[i], i);
        mesh.uv = uvs.ToArray();

        level.mesh = mesh;
        level.data = samples;

        return level;
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

    public Data generateObjects(int seed, Range[] ranges, float scale, int octaves, float persistance, float lacunarity)
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
