using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class CuttingRecipeSO : ScriptableObject
{
    public enum InteractType { press, hold, pressAndHold };
    public KitchenObjectSO input;
    public KitchenObjectSO output;
    public int cuttingInteractionsMax;
    public InteractType interactType;
    public float? interactTimeMax;
}
