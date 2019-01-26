using UnityEngine;


[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class MapEntity : MonoBehaviour
{
    [System.Serializable]
    public class Section
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
        Map.Level level = map.generateLevel(38729148);
        mesh_Filter.mesh = level.mesh;
        transform.position = map.position;

        float scale = 2;
        int octaves = 6;
        float persistance = 0.6f;
        float lacunarity = 3.41f;

        var mapData = map.generateObjects(38714623, new Map.Range[0], scale, octaves, persistance, lacunarity);

        int zindex = 0;
        for (int y = 0; y < mapData.YN; ++y)
            for (int x = 0; x < mapData.XN; ++x)
            {
                var section = (int)level.data.grid[x, y];
                var obstaclesLength = sections[section].obstacles.Length;
                var obstacles = sections[section].obstacles;

                int obstacleIndex = Random.Range(0, obstaclesLength);
                var obstacle = obstacles[obstacleIndex];
                if (mapData.grid[x, y] == 1)
                {
                    var collider = obstacle.GetComponent<BoxCollider2D>();
                    var offset = new Vector3(collider.offset.x, collider.offset.y + collider.size.y / 2, 0);
                    var pos = new Vector3(x + 0.5f, y + 0.5f, 0) + map.position - offset;
                    var obj = Instantiate(obstacle, pos, Quaternion.identity);
                    var renderer = obj.GetComponentInChildren<SpriteRenderer>();
                    renderer.sortingOrder = (52 - y) * 100;
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
