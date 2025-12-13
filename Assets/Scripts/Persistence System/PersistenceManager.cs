using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistenceManager : MonoBehaviour
{
    public GameData GameData;

    private StorageType _storageType;
    public StorageType StorageType
    {
        get => _storageType;
        set
        {
            _storageType = value;
            switch (_storageType)
            {
                case StorageType.JSON:
                    if (StorageHandler != null)
                    {
                        Destroy((MonoBehaviour)StorageHandler);
                    }
                    StorageHandler = gameObject.AddComponent<JSONStorageHandler>();
                    break;

                default:
                    break;
            }
        }
    }
    public IStorageHandler StorageHandler;

    List<IPersistable> Subscribers = new();
    public event Action LoadStart;
    public event Action LoadEnd;
    public event Action SaveStart;
    public event Action SaveEnd;
    public static PersistenceManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        StorageHandler = gameObject.AddComponent<JSONStorageHandler>();

        SceneManager.sceneUnloaded += (_) => PullFromPersistables();
        SceneManager.sceneLoaded += (_, _) => FindAllPersistableScripts();
        SceneManager.sceneLoaded += OnMapSceneActive;
    }

    void Start()
    {
        FindAllPersistableScripts();
    }

    [ContextMenu("Trigger New Game")]
    public void TriggerNew()
    {
        GameData = new();
        StorageHandler.Write(GameData);
    }

    [ContextMenu("Trigger Load Game")]
    public void TriggerLoad()
    {
        LoadStart?.Invoke();

        if (!TryGetSaveFile(out GameData))
        {
            throw new NullReferenceException($"[{GetType().Name}] Read null save file");
            // TODO automatically makes a new game?
        }

        PushToPersistables();
        // Optional: reload scene

        LoadEnd?.Invoke();
    }

    [ContextMenu("Trigger Save Game")]
    public void TriggerSave()
    {
        SaveStart?.Invoke();

        PullFromPersistables();

        StorageHandler.Write(GameData);

        SaveEnd?.Invoke();
    }

    public void PushToPersistables()
    {
        foreach (var item in Subscribers)
        {
            item.Load(GameData);
        }
    }

    public void PullFromPersistables()
    {
        foreach (var item in Subscribers)
        {
            item.Save(ref GameData);
        }
    }

    public void AddSubcriber(IPersistable persistable)
    {
        Subscribers.Add(persistable);
    }

    void FindAllPersistableScripts()
    {
        // Finds every MonoBehaviour script that also implements IPersistable
        IEnumerable<IPersistable> persistables = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IPersistable>();
        Subscribers = new List<IPersistable>(persistables);
        Debug.Log($"[PersistenceManager] Found {Subscribers.Count} persistables");
    }

    public bool TryGetSaveFile(out GameData data)
    {
        GameData read = StorageHandler.Read("save");
        if (read == null)
        {
            data = null;
            return false;
        }
        data = read;
        return true;
    }

    public void OnMapSceneActive(Scene next, LoadSceneMode loadSceneMode)
    {
        if (next.name == "Game")
        {
            PushToPersistables();
        }
    }
}

[Serializable]
public enum StorageType
{
    JSON,
    // Binary,
    // Cloud
    // add more if needed
}