using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class AndroidBackToMenu : MonoBehaviour
{
    [SerializeField] private string menuSceneName = "MainMenu";
    [SerializeField] private ConfirmDialog confirmDialog; // arrástralo si lo usas

    void Update()
    {
        bool backPressed = false;

        // --- Nuevo Input System ---
#if ENABLE_INPUT_SYSTEM
        // En Android, el botón "Atrás" llega como Escape en el Keyboard virtual.
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            backPressed = true;

        // (Opcional) mando: botón B/Circle como "volver"
        if (Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame)
            backPressed = true;

        // --- Input Manager clásico ---
#elif ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetKeyDown(KeyCode.Escape))
            backPressed = true;
#endif

        if (!backPressed) return;

        // 1) Si el modal está abierto, ciérralo
        if (confirmDialog != null)
        {
            bool dialogActive =
                (confirmDialog.root != null && confirmDialog.root.activeInHierarchy) ||
                 confirmDialog.gameObject.activeInHierarchy;

            if (dialogActive)
            {
                confirmDialog.Hide();
                return;
            }
        }

        // 2) Si hay un TMP_InputField con foco, primero quita el foco (cierra teclado)
        var go = EventSystem.current?.currentSelectedGameObject;
        var input = go ? go.GetComponent<TMP_InputField>() : null;
        if (input != null && input.isFocused)
        {
            input.DeactivateInputField();
            EventSystem.current.SetSelectedGameObject(null);
            return;
        }

        // 3) Navega al menú
        if (!string.IsNullOrEmpty(menuSceneName))
            SceneManager.LoadScene(menuSceneName);
    }
}
