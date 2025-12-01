public interface IStorageHandler
{
    void Write(GameData data);
    GameData Read(string filename); // any form of profile, can be string, object, etc.
}