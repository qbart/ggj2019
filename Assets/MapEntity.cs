using UnityEngine;
using System.Collections;


[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class MapEntity : MonoBehaviour
{
    [System.Serializable]
    public class Section
    {
        public GameObject[] obstacles;
    }

    public GameObject[] boosters;

    public Section[] sections;

    Map map;
    Map.Level level;

    void Start()
    {
        System.Random prng = new System.Random();

        MeshRenderer mesh_Renderer = GetComponent<MeshRenderer>();
        MeshFilter mesh_Filter = GetComponent<MeshFilter>();
        map = new Map(52, 52);
        level = map.generateLevel(prng.Next());
        mesh_Filter.mesh = level.mesh;
        transform.position = map.position;

        float scale = 2;
        int octaves = 6;
        float persistance = 0.6f;
        float lacunarity = 3.41f;

        var mapData = map.generateObjects(prng.Next(), new Map.Range[0], scale, octaves, persistance, lacunarity);

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
                    var offset = new Vector3(collider.offset.x, collider.offset.y - collider.size.y, 0);
                    var pos = new Vector3(x + 0.5f, y + 0.5f, 0) + map.position - offset;
                    var obj = Instantiate(obstacle, pos, Quaternion.identity);
                    var renderer = obj.GetComponentInChildren<SpriteRenderer>();
                    renderer.sortingOrder = Mathf.RoundToInt(5200 - ((obj.transform.position.y + offset.y) * 100));
                }
            }

        StartCoroutine("spawnBoosters");
    }

    void Update()
    {
    }

    void OnDrawGizmos()
    {
    }

    IEnumerator spawnBoosters()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            // TODO: prevent collisions from grid
            int x = Random.Range(2, 50);
            int y = Random.Range(2, 50);

            var pos = new Vector3(x, y, 0);
            var obj = Instantiate(boosters[Random.Range(0, boosters.Length)], pos, Quaternion.identity);
            var renderer = obj.GetComponentInChildren<SpriteRenderer>();
            renderer.sortingOrder = Mathf.RoundToInt(5200 - (obj.transform.position.y * 100));
        }
    }
}
