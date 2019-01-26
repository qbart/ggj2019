using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class MapEntity : MonoBehaviour
{
    [System.Serializable]
    public struct Section
    {
        public GameObject[] obstacles;
    }

    public Section[] sections;

    Map map;

    void Start()
    {
        MeshRenderer mesh_Renderer = GetComponent<MeshRenderer>();
        MeshFilter mesh_Filter = GetComponent<MeshFilter>();
        map = new Map(52, 52);
        mesh_Filter.mesh = map.generateMesh();
        transform.position = map.position;

        float scale = 2;
        int octaves = 6;
        float persistance = 0.6f;
        float lacunarity = 3.41f;

        var mapData = map.generateMap(38714623, scale, octaves, persistance, lacunarity);

        var tree = sections[0].obstacles[0];

        int zindex = 0;
        for (int y = 0; y < mapData.YN; ++y)
            for (int x = 0; x < mapData.XN; ++x)
            {
                if (mapData.grid[x, y] == 1)
                {
                    var collider = tree.GetComponent<BoxCollider2D>();
                    var offset = new Vector3(collider.offset.x, collider.offset.y + collider.size.y / 2, 0);
                    var pos = new Vector3(x + 0.5f, y + 0.5f, 0) + map.position - offset;
                    var obj = Instantiate(tree, pos, Quaternion.identity);
                    var renderer = obj.GetComponentInChildren<SpriteRenderer>();
                    renderer.sortingOrder = 52 - y;
                }
            }
    }

    void Update()
    {
    }

    void OnDrawGizmos()
    {
    }
}
