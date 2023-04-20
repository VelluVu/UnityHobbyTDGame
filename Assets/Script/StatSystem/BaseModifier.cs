namespace TheTD.StatSystem
{
    public abstract class BaseModifier : IModifier
    {
        public virtual int Order { get; }
        public virtual float Value { get; }
        public virtual StatModifierType Type { get; }
        public virtual object Source { get; }
 
        public virtual StatFlags ApplicableStatFlags { get; }

        public BaseModifier(float value, StatModifierType type, StatFlags applicableTo, int order, object source)
        {
            Value = value;
            Type = type;
            Order = order;
            Source = source;
            ApplicableStatFlags = applicableTo;
        }
        public BaseModifier(float value, StatModifierType type, StatFlags applicableTo) : this(value, type, applicableTo, (int)type, null)
        {
        }

        public BaseModifier(float value, StatModifierType type, StatFlags applicableTo, int order) : this(value, type, applicableTo, order, null)
        {
        }

        public BaseModifier(float value, StatModifierType type, StatFlags applicableTo, object source) : this(value, type, applicableTo, (int)type, source)
        {
        }
    }
}