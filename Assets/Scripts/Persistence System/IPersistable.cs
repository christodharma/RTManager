public interface IPersistable
{
    void Save(ref GameData data);
    void Load(GameData data);
}