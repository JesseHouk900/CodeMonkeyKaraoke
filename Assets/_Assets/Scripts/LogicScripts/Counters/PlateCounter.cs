using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateCounter : BaseCounter
{
    public event EventHandler OnPlateSpawned;

    public event EventHandler OnPlateRemoved;

    [SerializeField] private KitchenObjectSO plateSO;
    [SerializeField] private float spawnPlateTimerMax;
    private float spawnPlateTimer;
    private int plateSpawnedAmount;

    private void Start()
    {
        plateSpawnedAmount = 0;
    }

    private void Update()
    {
        spawnPlateTimer += Time.deltaTime;

        if (spawnPlateTimer > spawnPlateTimerMax)
        {
            //KitchenObject.SpawnKitchenObject(plateSO, this);
            OnPlateSpawned?.Invoke(this, EventArgs.Empty);
            spawnPlateTimer = 0f;
            plateSpawnedAmount++;
        }
    }

    public int GetPlateSpawnedAmount()
    {
        return plateSpawnedAmount;
    }

    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            if (plateSpawnedAmount > 0)
            {
                plateSpawnedAmount--;

                KitchenObject.SpawnKitchenObject(plateSO, player);
                OnPlateRemoved?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
