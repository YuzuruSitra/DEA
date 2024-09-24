namespace Item
{
    public interface IItem
    {
        // アイテムの名前
        string Name { get; }

        ItemKind ItemKind { get; }

        // アイテムの説明
        string Description { get; }

        // アイテムの使用メソッド
        void UseEffect();

        // アイテムの消費型かどうか
        bool IsConsumable { get; }
    }

    public enum ItemKind
    {
        Dynamite
    }
}