using UnityEngine;

[CreateAssetMenu(menuName = "TCG/Effects/Instant Seals")]
public class SealEffect : EffectData
{
    public string sealType;
    public int amount;

    public override void Activate(CardDisplay card)
    {
        Debug.Log($"[EFFECT] Added {amount} {sealType} Seals instantly.");
        // GameManager.AddSeals(...);
    }
}