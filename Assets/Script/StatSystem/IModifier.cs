public interface IModifier
{
    int Order { get; }
    float Value { get; }
    StatModifierType Type { get; }
    StatFlags ApplicableStatFlags { get; }
    public object Source { get; }
}
