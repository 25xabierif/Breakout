using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static int Score { get; private set; } = 0; 
    public static int Lives { get; private set; } = 3;
    public static int[] totalBricks = new int[] {0, 32, 32};

    // Método para actualizar la puntuación
    public static void UpdateScore(int points)
    {
        Score += points;
    }
    
    // Método para actualizar las vidas
    public static void Updatelives()
    {
        Lives--;
    }
    public static void ResetGame()
    {
        Score = 0;

        Lives = 3;

        SceneManager.LoadScene(0);
    }
}
