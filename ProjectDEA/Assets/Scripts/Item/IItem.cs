namespace Item
{
    public interface IItem
    {
        // ItemKind ItemKind { get; }
        // void UseEffect();
        // bool IsConsumable { get; }
    }

    public enum ItemKind
    {
        Key,
        Dynamite,
        Born,
        PowerPotion,
        PowerApple
    }
}