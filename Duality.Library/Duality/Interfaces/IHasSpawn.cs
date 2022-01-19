namespace Duality
{
    public interface IHasSpawnPoint
    {
        bool HasSpawn => Spawn != null;
        ObjectSpawn Spawn { get; set; }
    }
}