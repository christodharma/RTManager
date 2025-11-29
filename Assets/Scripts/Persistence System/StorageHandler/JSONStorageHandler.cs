using System;
using System.IO;
using UnityEngine;

public class JSONStorageHandler : MonoBehaviour, IStorageHandler
{
    public string FileName = "GameSave";
    public string FileDirectory;

    void Awake()
    {
        FileDirectory = Path.Combine(Application.persistentDataPath, "Saves");
    }

    public GameData Read()
    {
        string fullPath = Path.Combine(FileDirectory, FileName);
        fullPath = Path.ChangeExtension(fullPath, ".json");
        GameData readData = null;
        if (File.Exists(fullPath))
        {
            Debug.Log($"Reading file at {fullPath}");
            try
            {
                string serializedData = "";

                using FileStream stream = new(fullPath, FileMode.Open);
                using StreamReader reader = new(stream);
                serializedData = reader.ReadToEnd();

                readData = JsonUtility.FromJson<GameData>(serializedData);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error occured when reading data from file at {fullPath}: \n{e}");
            }
        }
        return readData;
    }

    public void Write(GameData data)
    {
        string fullPath = Path.Combine(FileDirectory, $"{FileName}{DateTime.Now:yyyy-MM-ddTHH-mm-ss}");
        fullPath = Path.ChangeExtension(fullPath, ".json");

        long freeSpace = GetTotalFreeSpace(FileDirectory);
        if (freeSpace == -1)
        {
            Debug.LogError("There's no storage to save");
            return;
        }

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToStore = JsonUtility.ToJson(data, true);

            using FileStream stream = new(fullPath, FileMode.Create);
            using StreamWriter writer = new(stream);
            writer.Write(dataToStore);
        }
        catch (Exception e)
        {
            // Catching exception like directory not existing, etc.
            Debug.LogError($"[JSON StorageHandler] {e}");
        }
    }

    private long GetTotalFreeSpace(string driveName)
    {
        // Get the DriveInfo for the path
        DriveInfo drive = new(Path.GetPathRoot(driveName));

        if (drive.IsReady)
        {
            return drive.AvailableFreeSpace;
        }
        return -1;
    }
}