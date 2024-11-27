using System;

namespace Gimmick
{
    public interface IGimmickID
    {
        GimmickID GimmickIdInfo { get; set; }
        event Action<IGimmickID> Returned;
    }

    public struct GimmickID
    {
        public int RoomID;
        public int InRoomGimmickID;
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
