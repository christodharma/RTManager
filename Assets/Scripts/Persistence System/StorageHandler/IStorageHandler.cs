using System.IO;

public interface IStorageHandler
{
    void Write(GameData data);
    GameData Read();
}