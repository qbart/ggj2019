using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class MapEntity : MonoBehaviour
{
    Map map;

    MeshRenderer mesh_Renderer;
    MeshFilter mesh_Filter;

    void Start()
    {
        mesh_Renderer = GetComponent<MeshRenderer>();
        mesh_Filter = GetComponent<MeshFilter>();
        map = new Map(52, 52);
        mesh_Filter.mesh = map.generateMesh();
        transform.position = map.position;
    }

    void Update()
    {

    }
}
