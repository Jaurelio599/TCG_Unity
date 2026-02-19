using UnityEngine;
using System.Collections.Generic;

// NUEVO: Creamos la lista fija de Elementos para evitar errores de texto
public enum ElementType
{
    None,       // Por si alguna carta no tiene elemento
    Fire,
    Water,
    Earth,
    Wind,
    Void,
    Light,
    Dark,
    Ice,        // Vi que usaste Ice en un ejemplo anterior
    Generic
}

public enum CardType
{
Spirit,
Ritual,
Soul,

Will

}

public enum CardArchetype
{
Bruma,
OlaDeFuedo,
Soul,

Will

}

[CreateAssetMenu(fileName = "NewCard", menuName = "TCG/Card")]
public class CardData : ScriptableObject
{
    [Header("Basic Info")]
    public string id;
    public string cardName;
    [TextArea] public string description;
    public Sprite art;

    [Header("Base Stats")]
    public int basePd; 
    public int basePs; 

    [Header("Types")]
    public CardType type;       
    public string archetype;  
    
    // CAMBIO AQUÍ: Ahora usa la lista desplegable en lugar de texto libre
    public ElementType element;    

    [Header("Cost & Requirements")]
    public List<CardCost> costs;

    [Header("--- PASSIVE STATES ---")]
    public List<StateData> passiveStates; 

    [Header("--- INSTANT TRIGGERS ---")]
    public List<EffectData> onSummonEffects; 
    public List<EffectData> onDeathEffects;

    [System.Serializable]
    public class SummonRequirement
    {
        public string specificCardName; 
        public int maxLevel;
        public string requiredArchetype;

        // CAMBIO AQUÍ: Ahora pide la lista de Elementos reales
        public List<ElementType> allowedElements; 
    }

    [Header("--- RITUAL SETTINGS ---")]
    public SummonRequirement summonRequirement; 
}