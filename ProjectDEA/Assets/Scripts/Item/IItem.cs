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
        Dynamite,
        Born,
        PowerPotion,
        PowerApple,
        HardStone,
        SignCandle,
        RaggedMemoirs
    }
}