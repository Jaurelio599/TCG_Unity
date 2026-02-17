using UnityEngine;

[System.Serializable] // Importante para que se vea en el Inspector
public struct CardCost
{
    public CostType type; // Ej. WaterSeal
    public int amount;    // Ej. 2
}