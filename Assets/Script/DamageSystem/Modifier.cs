using System.Collections.Generic;
using TheTD.StatSystem;

public class Modifier : BaseModifier
{
    #region Constuctors
    public Modifier(float value, StatModifierType type, StatFlags applicableTo) : base(value, type, applicableTo)
    {
    }

    public Modifier(float value, StatModifierType type, StatFlags applicableTo, int order) : base(value, type, applicableTo, order)
    {
    }

    public Modifier(float value, StatModifierType type, StatFlags applicableTo, object source) : base(value, type, applicableTo, source)
    {
    }

    public Modifier(float value, StatModifierType type, StatFlags applicableTo, int order, object source) : base(value, type, applicableTo, order, source)
    {
    }

    #endregion
}
