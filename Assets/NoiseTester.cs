using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseTester : MonoBehaviour
{
    Map map;
    public int seed = 0;
    public float scale = 50;
    [Range(1, 100)]
    public int octaves = 6;
    [Range(0,1)]
    public float persistance = 0.6f;
    public float lacunarity = 2;
    public Map.Range[] ranges;

    void Start()
    {
        map = new Map(52, 52);
    }

    // Update is called once per frame
    void Update()
    {
        var tex = map.generateNoiseMap(seed, ranges, scale, octaves, persistance, lacunarity);
        // var tex = map.generateNoiseMap(seed);
        var renderer = GetComponent<Renderer>();
        renderer.sharedMaterial.mainTexture = tex;
        renderer.transform.localScale = new Vector3(5.2f, 1, 5.2f);
    }
}
