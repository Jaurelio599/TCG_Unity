using UnityEngine;
using System; 

[CreateAssetMenu(menuName = "TCG/Effects/Instant Seals")]
public class SealEffect : EffectData
{
    public ElementType sealType; 
    public int amount;

    public override void Activate(CardDisplay card)
    {
        if (GameManager.instance != null)
        {
            Debug.Log($"[EFECTO] {card.originalData.cardName} te dio {amount} sello(s) de {sealType}.");
            
            try 
            {
                CostType typeToGive = (CostType)Enum.Parse(typeof(CostType), sealType.ToString() + "Seal");
                
                // --- ¡EL ERROR ESTABA AQUÍ! Ya le quité las // ---
                // OJO: Asumo que en tu GameManager la función se llama "AddSeal". 
                // Si la llamaste diferente (por ejemplo, "AddMana" o "AddSeals" en plural), 
                // solo cámbiale el nombre en esta línea de abajo:
                GameManager.instance.AddSeal(typeToGive, amount); 
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[SealEffect] No se encontró el CostType equivalente para {sealType}Seal. Error: {e.Message}");
            }
        }
    }
}