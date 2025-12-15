using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public GameObject TutorialCanvas;

    public static TutorialManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        if (PersistenceManager.Instance != null && PersistenceManager.Instance.IsNewGame)
        {
            PersistenceManager.Instance.IsNewGame = false;
            Instantiate(TutorialCanvas);
        }
    }
}