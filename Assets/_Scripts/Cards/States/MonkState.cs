using UnityEngine;

[CreateAssetMenu(menuName = "TCG/States/Persistent/Monk")]
public class MonkState : StateData
{
    [Header("Monk Logic")]
    [Tooltip("El tipo de sello que este monje genera automáticamente cada turno.")]
    public string sealType; // Ej: "Water", "Fire"

    [Tooltip("Cuántos sellos genera.")]
    public int amount;      // Ej: 1

    // Esta función se ejecuta sola al FINAL DE CADA TURNO mientras el monje esté vivo
    public override void OnTurnEnd(CardDisplay card)
    {
        // Aquí iría la conexión real con tu GameManager
        Debug.Log($"[STATE - END TURN] {card.originalData.cardName} is meditating... Generates {amount} <{sealType}> Seal(s).");
        
        // Ejemplo de uso real:
        // GameManager.Instance.AddSeals(sealType, amount);
    }
}