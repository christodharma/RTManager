using UnityEngine;

public class NPC : MonoBehaviour
{
    public string dialogFileName;
    public Sprite portraitOverride;

    public void Interact()
    {
        if (string.IsNullOrEmpty(dialogFileName))
        {
            Debug.LogWarning($"NPC '{name}' has no dialogFileName set.");
            return;
        }

        if (DialogController.Instance != null)
            DialogController.Instance.StartDialog(dialogFileName, portraitOverride);
        else
            Debug.LogError("No DialogController instance found in scene.");
    }
}
