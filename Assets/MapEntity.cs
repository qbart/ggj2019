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
        float[,] grid = map.generateObstacleMap(3874623);

        var tree = sections[0].obstacles[0];

        for (int y = 0; y < 52; ++y)
            for (int x = 0; x < 52; ++x)
            {
                if (grid[x, y] == 0)
                {
                    Instantiate(tree, new Vector3(x, y - 1, 0) + map.position, Quaternion.identity);
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
