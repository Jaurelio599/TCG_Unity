using UnityEngine;
using System.Collections.Generic; // Necesario para usar List<>

[CreateAssetMenu(fileName = "NewCard", menuName = "TCG/Card")]
public class CardData : ScriptableObject
{
    [Header("Basic Info")]
    public string id;
    public string cardName;
    [TextArea] public string description;
    public Sprite art;

    [Header("Base Stats")]
    public int basePd; // Attack
    public int basePs; // Health

    [Header("Types")]
    public string type;       // Ej. "Water"
    public string archetype;  // Ej. "Monk"
    public string element;    // Ej. "Ice"

    [Header("Cost & Requirements")]
    // Antes era: public int cost;
    // AHORA ES UNA LISTA:
    public List<CardCost> costs;

    [Header("--- PASSIVE STATES ---")]
    [Tooltip("Estados que la carta tiene SIEMPRE mientras viva (Monje, Recuperación)")]
    public List<StateData> passiveStates; 

    [Header("--- INSTANT TRIGGERS ---")]
    [Tooltip("Cosas que pasan UNA vez al entrar")]
    public List<EffectData> onSummonEffects; 

    [Tooltip("Cosas que pasan UNA vez al morir")]
    public List<EffectData> onDeathEffects;
}