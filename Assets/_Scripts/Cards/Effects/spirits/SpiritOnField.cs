using UnityEngine;

public class SpiritOnField : MonoBehaviour
{
    [Header("Datos Base")]
    public CardData baseData; // De dónde viene

    [Header("Stats Editables (Actuales)")]
    public int currentPD; // Poder de Daño
    public int currentPS; // Puntos de Salud
    public int maxPS;     // Salud máxima (para saber cuánto curar)
    
    // Su estado o condición actual
    public SpiritState currentState;

    // Esta función se llama justo al momento de invocarlo al campo
    public void InitializeSpirit(CardData data)
    {
        baseData = data;
        
        // Copiamos los stats de la carta a las variables vivas
        currentPD = data.basePd;
        currentPS = data.basePs;
        maxPS = data.basePs+10000000;
        
        // Asignamos su estado si la carta base tiene uno definido
        // (Asumiendo que agregarás un campo 'defaultState' a tu CardData)
        // currentState = data.defaultState; 

        // Aquí disparamos el efecto de invocación
        TriggerEffect(EffectTrigger.OnSummon);
    }

    // El GameManager o UIManager llamarán a esta función cuando reciba daño o se cure
    public void ModifyStats(int pdChange, int psChange)
    {
        currentPD += pdChange;
        currentPS += psChange;
        
        // Evitar que la salud supere el máximo
        if (currentPS > maxPS) currentPS = maxPS;

        // Comprobar muerte
        if (currentPS <= 0)
        {
            Die();
        }
    }

    // --- SISTEMA DE TRIGGERS ---

    // Esta función la llamará el GameManager cada vez que cambie de fase/turno
    public void OnPhaseChange(EffectTrigger triggerType)
    {
        TriggerEffect(triggerType);
        ProcessStates(triggerType);
    }

    private void TriggerEffect(EffectTrigger trigger)
    {
        // Aquí conectaremos la lógica de los efectos de la propia carta
        // Ej: Si baseData.effectTrigger == trigger, entonces ejecutar efecto.
    }

    // --- PROCESAMIENTO DE ESTADOS PREDEFINIDOS ---
    private void ProcessStates(EffectTrigger currentTrigger)
    {
        // Ejemplo del MONJE
        if (currentState == SpiritState.MonkState && currentTrigger == EffectTrigger.OnPlayerTurnEnd)
        {
            Debug.Log($"{baseData.cardName} (Monje) ha generado 1 Sello de Agua.");
            GameManager.instance.AddSeal(CostType.WaterSeal, 1);
        }

        // Ejemplo de RECUPERACIÓN
        if (currentState == SpiritState.RecoveryState && currentTrigger == EffectTrigger.OnOpponentTurnEnd)
        {
            int healAmount = Mathf.RoundToInt(maxPS * 0.10f); // Ejemplo: recupera 10%
            ModifyStats(0, healAmount);
            Debug.Log($"{baseData.cardName} se ha curado {healAmount} PS por su estado de Recuperación.");
        }
    }

    private void Die()
    {
        TriggerEffect(EffectTrigger.OnDeath);
        Debug.Log($"{baseData.cardName} ha sido destruido.");
        // Mover al Grave_deck, destruir este objeto del campo, etc.
        Destroy(gameObject);
    }
}