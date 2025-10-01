using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cargar escenas

public class SceneLoader : MonoBehaviour
{
    //  Cargar una escena por nombre
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    //  Cerrar la aplicación (funciona al compilar el juego)
    public void QuitGame()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}
