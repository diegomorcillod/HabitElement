using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConfirmDialog : MonoBehaviour
{
    [Header("Refs")]
    public GameObject root;     // instancia raíz "ConfirmDialog" en escena
    public TMP_Text title;
    public TMP_Text message;
    public Button btnConfirm;   // Botón "Eliminar"
    public Button btnCancel;    // Botón "Cancelar"

    private Action onYes;

    private void Awake()
    {
        if (btnConfirm != null)
        {
            btnConfirm.onClick.RemoveAllListeners();
            btnConfirm.onClick.AddListener(OnConfirm);
        }
        if (btnCancel != null)
        {
            btnCancel.onClick.RemoveAllListeners();
            btnCancel.onClick.AddListener(OnCancel);
        }
        Hide();
    }

    public void Show(string titleText, string messageText, Action onConfirm)
    {
        Debug.Log($"[ConfirmDialog] Show → {messageText}");
        onYes = onConfirm;

        if (title)   title.text   = string.IsNullOrEmpty(titleText) ? "Confirmación" : titleText;
        if (message) message.text = messageText ?? "";

        if (root) root.SetActive(true);
        else gameObject.SetActive(true);

        transform.SetAsLastSibling(); // por encima del resto
    }

    public void Hide()
    {
        if (root) root.SetActive(false);
        else gameObject.SetActive(false);
        // ¡No limpies onYes aquí! Deja que OnConfirm/OnCancel decidan.
    }

    private void OnConfirm()
    {
        Debug.Log("[ConfirmDialog] Confirm pressed");
        var cb = onYes;   // guarda la referencia
        onYes = null;     // limpia para evitar dobles invocaciones
        Hide();           // oculta el diálogo
        cb?.Invoke();     // ahora sí, ejecuta la acción
    }

    private void OnCancel()
    {
        Debug.Log("[ConfirmDialog] Cancel pressed");
        onYes = null;
        Hide();
    }
}
