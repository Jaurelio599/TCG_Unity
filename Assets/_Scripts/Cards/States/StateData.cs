using UnityEngine;

// Base para condiciones continuas: Monje, Recuperación, Veneno
public abstract class StateData : ScriptableObject
{
    [Header("State Info")]
    public string stateName;
    public Sprite icon;

    // Estas funciones se llamarán automáticamente en cada turno mientras la carta viva
    public virtual void OnTurnStart(CardDisplay card) { }
    public virtual void OnTurnEnd(CardDisplay card) { }
    
    // Opcional: Si el estado modifica stats pasivamente (ej. +100 Ataque)
    public virtual void OnStatUpdate(CardDisplay card) { }
}