namespace Gimmick
{
    public interface IGimmickID
    {
        int InRoomID { get; set; }
    }
    
    public enum GimmickKind
    {
        ExitObelisk,
        ObeliskKeyOut,
        TreasureBox,
        EnemySpawnArea,
        BornOut,
        Monument
    }
}
