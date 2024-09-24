namespace Item
{
    public interface IItem
    {
        // アイテムの名前
        string Name { get; }

        // アイテムのID
        int ItemID { get; }
        ItemKind ItemKind { get; }

        // アイテムの説明
        string Description { get; }

        // アイテムの価値
        int Value { get; }

        // アイテムの使用メソッド
        void UseEffect();

        // アイテムをインベントリに追加するときに呼ばれるメソッド
        //void AddToInventory();

        // アイテムを削除するときに呼ばれるメソッド
        //void RemoveFromInventory();

        // アイテムの消費型かどうか
        bool IsConsumable { get; }
    }

    public enum ItemKind
    {
        Dynamite
    }
}