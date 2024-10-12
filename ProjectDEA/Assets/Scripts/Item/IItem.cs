namespace Item
{
    public interface IItem
    {
        string Name { get; }
        ItemKind ItemKind { get; }
        string Description { get; }
        void UseEffect();
        bool IsConsumable { get; }
    }

    public enum ItemKind
    {
        Key,
        Dynamite
    }
}