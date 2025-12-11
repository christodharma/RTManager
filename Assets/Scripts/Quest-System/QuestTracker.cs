using UnityEngine;

public class QuestTracker : MonoBehaviour
{
    public static QuestTracker Instance;

    [Header("UI References")]
    public RectTransform arrowPointer;
    public GameObject compassContainer;

    [Header("Settings")]
    public Transform playerTransform;
    private Transform currentTarget;

    private void Awake() => Instance = this;

    private void Start() => compassContainer.SetActive(false);

    public void TrackQuest(Transform target)
    {
        if (target == null) return;

        currentTarget = target.transform;
        compassContainer.SetActive(true);
    }

    public void StopTracking()
    {
        currentTarget = null;

        if (compassContainer != null)
        {
            compassContainer.SetActive(false);
        }

        Debug.Log("Quest tracking dihentikan.");
    }

    private void Update()
    {
        if (currentTarget == null || playerTransform == null) return;

        Vector3 direction = currentTarget.position - playerTransform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        arrowPointer.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }
}