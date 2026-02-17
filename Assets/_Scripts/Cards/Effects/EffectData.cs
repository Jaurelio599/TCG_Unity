using UnityEngine;

// Base para cosas instantáneas: Daño, Robar Carta, Ganar Sello al entrar
public abstract class EffectData : ScriptableObject
{
    public abstract void Activate(CardDisplay card);
}