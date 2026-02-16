using UnityEngine;
using TMPro; // Necesario para texto profesional
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    [Header("Referencia al ADN")]
    public CardData datosOriginales;

    [Header("Estadísticas EN VIVO (Editables)")]
    public int pdActual;
    public int psActual;
    public int costoActual;

    [Header("Referencias visuales (Arrastra desde Unity)")]
    public Image imagenArte;
    public TextMeshProUGUI textoNombre;
    public TextMeshProUGUI textoDescripcion;
    public TextMeshProUGUI textoPD;
    public TextMeshProUGUI textoPS;
    public TextMeshProUGUI textoCosto;
    
    // Esta función se llama al iniciar la carta (Invocar)
    void Start()
    {
        if (datosOriginales != null)
        {
            CargarDatos();
        }
    }

    // 1. Copiamos los datos del ScriptableObject a nuestras variables locales
    public void CargarDatos()
    {
        // Visuales
        textoNombre.text = datosOriginales.nombreCarta;
        textoDescripcion.text = datosOriginales.descripcion;
        imagenArte.sprite = datosOriginales.arte;
        // Lógica: Aquí es donde ocurre la magia
        // Copiamos el valor BASE al valor ACTUAL
        pdActual = datosOriginales.pdBase;
        psActual = datosOriginales.psBase;
        costoActual = datosOriginales.costo;
       // 

        ActualizarUI();
    }

    // 2. Función para modificar la vida (Daño o Cura)
    public void ModificarVida(int cantidad)
    {
        psActual += cantidad;
        
        // Efecto visual si muere
        if (psActual <= 0)
        {
            psActual = 0;
            Debug.Log(datosOriginales.nombreCarta + " ha sido destruida.");
            // Aquí llamarías a una animación de muerte
        }

        ActualizarUI(); // Refrescamos los números en pantalla
    }

    // 3. Función para modificar ataque (Buffs/Debuffs)
    public void ModificarAtaque(int cantidad)
    {
        pdActual += cantidad;
        ActualizarUI();
    }

    // 4. Actualiza los textos en pantalla para que coincidan con las variables
    private void ActualizarUI()
    {
        textoPD.text = pdActual.ToString();
        textoPS.text = psActual.ToString();
        textoCosto.text = costoActual.ToString();

        // Cambio de color dinámico (opcional)
        // Si el ataque subió, ponlo verde. Si bajó, rojo.
        if (pdActual > datosOriginales.pdBase) textoPD.color = Color.blue;
        else if (pdActual < datosOriginales.pdBase) textoPD.color = Color.red;
        else textoPD.color = Color.blue;
    }
}