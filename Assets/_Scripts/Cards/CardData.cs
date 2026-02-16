using UnityEngine;

[CreateAssetMenu(fileName = "NuevaCarta", menuName = "TCG/Carta")]
public class CardData : ScriptableObject
{
    [Header("Info Básica")]
    public string id;
    public string nombreCarta;
    [TextArea(3, 10)] public string descripcion;
    public Sprite arte; // La imagen

    [Header("Estadísticas Base")]
    public int costo;
    public int pdBase; // Poder (Ataque)
    public int psBase; // Salud (Vida)

    [Header("Tipos")]
    public string tipo; // Espiritu, Ritual, etc.
    public string arquetipo; // Monje, Guerrero...
    public string elemento; // Agua, Fuego...
}