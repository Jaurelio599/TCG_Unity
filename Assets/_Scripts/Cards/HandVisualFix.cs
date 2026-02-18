using UnityEngine;

public class HandVisualFix : MonoBehaviour
{
    void Update()
    {
        // Recorremos todas las cartas en la mano
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform carta = transform.GetChild(i);
            
            // Buscamos su Canvas (el que le pusimos para el Pop-up)
            Canvas canvasCarta = carta.GetComponent<Canvas>();
            
            if (canvasCarta != null)
            {
                // Si la carta NO está siendo señalada por el mouse (Orden normal)
                // (Asumimos que el Pop-up pone el orden en 50 o 100)
                if (canvasCarta.sortingOrder < 10) 
                {
                    // Le damos un orden basado en su posición en la fila
                    // La primera (0) tendrá orden 1. La última tendrá orden N.
                    // Así la de la derecha SIEMPRE tapará a la de la izquierda.
                    canvasCarta.sortingOrder = i + 1;
                }
            }
        }
    }
}