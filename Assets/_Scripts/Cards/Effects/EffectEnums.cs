using UnityEngine;

// ¿CUÁNDO se activa el efecto?
public enum EffectTrigger
{
    None,               // Para efectos continuos o nulos
    OnSummon,           // Al invocarse
    OnDeath,            // Al morir
    OnPlayerTurnStart,  // Al inicio de tu turno
    OnPlayerTurnEnd,    // Al finalizar tu turno
    OnOpponentTurnStart,// Al inicio del turno oponente
    OnOpponentTurnEnd,  // Al finalizar el turno oponente
    WhileOnField        // Efecto pasivo en el campo
}

// ESTADOS PREDEFINIDOS (Los "Keywords" de tus cartas)
public enum SpiritState
{
    None,
    MonkState,      // Genera sellos al final del turno
    RecoveryState,  // Recupera PS al final del turno rival
    // Aquí agregaremos más en el futuro (ej. Veneno, Escudo, etc.)
}