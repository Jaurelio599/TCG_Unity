using UnityEngine;

[CreateAssetMenu(menuName = "TCG/States/Recovery")]
public class RecoveryState : StateData
{
    [Range(0, 100)] public float healPercent;

    // Esto se ejecutará automágicamente al final de cada turno
    public override void OnTurnEnd(CardDisplay card)
    {
        int healAmount = Mathf.RoundToInt(card.originalData.basePs * (healPercent / 100f));
        Debug.Log($"[STATE] {card.originalData.cardName} regenerates {healAmount} HP.");
        // card.Heal(healAmount);
    }
}