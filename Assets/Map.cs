using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map
{
    int YN, XN;

    public Map(int xn, int yn)
    {
        XN = xn;
        YN = yn;
    }

    public Vector3 position {
        get {
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

                int typeY = 0;
                if (y < YN / 2 && x < XN / 2)
                    typeY = 0;
                else if (y < YN / 2 && x >= XN / 2)
                    typeY = 1;
                else if (y >= YN / 2 && x < XN / 2)
                    typeY = 2;
                else if (y >= YN / 2 && x >= XN / 2)
                    typeY = 3;

                //   2 --- 3
                //   |     |
                //   |     |
                //   0 --- 1

                verts.Add(new Vector3(x, y));
                verts.Add(new Vector3(x + 1, y));
                verts.Add(new Vector3(x, y + 1));
                verts.Add(new Vector3(x + 1, y + 1));

                // tri 0
                tris[typeY].Add(offset + 0);
                tris[typeY].Add(offset + 2);
                tris[typeY].Add(offset + 1);

                // tri 1
                tris[typeY].Add(offset + 1);
                tris[typeY].Add(offset + 2);
                tris[typeY].Add(offset + 3);

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
}
