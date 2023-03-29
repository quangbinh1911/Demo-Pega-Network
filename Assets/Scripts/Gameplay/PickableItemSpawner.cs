using System;
using System.Collections.Generic;
using System.Linq;
using FishNet;
using FishNet.Object;
using UnityEngine;
using Random = UnityEngine.Random;

public class PickableItemSpawner : NetworkBehaviour
{
    public List<GameObject> items;
    private List<Transform> _roots;


    private void Awake()
    {
        _roots = GetComponentsInChildren<Transform>().ToList();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        if (IsServer) SpawnItems();
    }

    private void SpawnItems()
    {
        foreach (var root in _roots)
        {
            var item = items[Random.Range(0, items.Count)];

            var itemGo = Instantiate(item);
            itemGo.transform.position = root.position;
            itemGo.transform.rotation = root.rotation;

            InstanceFinder.ServerManager.Spawn(itemGo);
        }
    }
}