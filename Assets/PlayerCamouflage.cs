﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerCamouflage : MonoBehaviour
{
    public UnityAction onCamouflageStart;
    public UnityAction onCamouflageStop;

    public float camouflageTime = 2f;
    public float cooldownTime = 2f;

    public GameObject map;

    Renderer spriteRenderer;
    Renderer pooferRenderer;
    Animator pooferAnimator;


    public bool isOn
    {
        get
        {
            return running;
        }
    }

    bool running;
    bool allowNextCamouflage;
    bool allowBreak;
    bool breakRequested;
    List<GameObject> poofedObjs;

    private IEnumerator cor;

    void Start()
    {
        poofedObjs = new List<GameObject>();
        running = false;
        allowNextCamouflage = true;
        allowBreak = false;
        breakRequested = false;

        spriteRenderer = GetComponent<SpriteRenderer>();
        var renderers = GetComponentsInChildren<SpriteRenderer>();
        var animators = GetComponentsInChildren<Animator>();
        pooferAnimator = animators[animators.Length - 1];
        pooferRenderer = renderers[renderers.Length - 1];
        pooferRenderer.enabled = false;
        pooferAnimator.enabled = false;

        onCamouflageStart += camouflageStarted;
        onCamouflageStop += camouflageStopped;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (allowNextCamouflage)
                StartCoroutine(cor = camouflage());
        }
        else if (Input.GetKeyUp(KeyCode.Q))
        {
            breakRequested = true;

        }

        if (breakRequested)
        {
            if (running && allowBreak)
            {
                StopCoroutine(cor);
                camouflageStopped();
            }

        }
    }

    IEnumerator camouflage()
    {
        Debug.Log("cam begin");

        allowNextCamouflage = false;
        allowBreak = false;
        running = true;
        breakRequested = false;

        if (onCamouflageStart != null)
            onCamouflageStart();

        yield return new WaitForSeconds(0.2f);
        pooferRenderer.enabled = false;
        pooferAnimator.enabled = false;
        poof();

        allowBreak = true;

        yield return new WaitForSeconds(camouflageTime - 0.2f);

        if (onCamouflageStop != null)
            onCamouflageStop();
    }

    IEnumerator cooldown()
    {
        Debug.Log("cooldown begin");
        yield return new WaitForSeconds(cooldownTime);
        allowNextCamouflage = true;
        Debug.Log("cooldown end");
    }


    void poof()
    {
        var rayOffset = new Vector3(0.17f, -1.6f, 0);
        var gridPos = transform.position + rayOffset;
        int x = (int)Mathf.Clamp(gridPos.x % 52, 0, 51);
        int y = (int)Mathf.Clamp(gridPos.y % 52, 0, 51);

        // Debug.Log("x: " + x);
        // Debug.Log("y: " + y);

        var section = (int)map.GetComponent<MapEntity>().level.data.grid[x, y];
        // Debug.Log("s: " + section);
        var obstaclesLength = map.GetComponent<MapEntity>().sections[section].obstacles.Length;
        var obstacles = map.GetComponent<MapEntity>().sections[section].obstacles;
        int obstacleIndex = Random.Range(0, obstaclesLength);
        var prefab = obstacles[obstacleIndex];
        Vector3 offset = Vector3.zero;
        Vector3 pos = Vector3.zero;
        if (prefab.GetComponent<BoxCollider2D>())
        {
            var col = prefab.GetComponent<BoxCollider2D>();
            offset = new Vector3(col.offset.x, -col.offset.y + col.size.y / 2, 0);
            pos = new Vector3(x + 0.5f, y + 0.5f, 0) - offset;

        }
        var instance = Instantiate(prefab, transform.position, Quaternion.identity, transform);
        instance.transform.Translate(rayOffset + offset);
        instance.GetComponentInChildren<SpriteRenderer>().sortingOrder = spriteRenderer.sortingOrder;

        poofedObjs.Add(instance);
    }

    void camouflageStarted()
    {
        pooferAnimator.enabled = true;
        pooferRenderer.enabled = true;
        spriteRenderer.enabled = false;

        pooferAnimator.Play(0);
    }

    void camouflageStopped()
    {
        Debug.Log("cam end");
        running = false;
        allowBreak = false;
        StartCoroutine(cooldown());

        foreach (var o in poofedObjs)
            Destroy(o);

        poofedObjs.Clear();

        pooferAnimator.enabled = false;
        pooferRenderer.enabled = false;
        spriteRenderer.enabled = true;
    }

}
