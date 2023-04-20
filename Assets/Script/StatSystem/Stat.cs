using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace TheTD.StatSystem
{
    [Serializable]
    public class Stat
    {
        //add original value, and ability to reset the stat without needing to create new
        public float BaseValue;
        protected bool isDirty = true;
        protected float lastBaseValue = float.MinValue;

        [SerializeField]protected StatFlags _statFlags = StatFlags.None;
        public StatFlags StatFlags { get => _statFlags; private set => _statFlags = value; }

        protected readonly List<IModifier> modifiers;
        public readonly ReadOnlyCollection<IModifier> Modifiers;

        public int RoundedValue 
        {
            get => (int)Math.Round(Value, MidpointRounding.AwayFromZero); 
        }

        protected float _value;
        public virtual float Value { get => GetValue(); }

        public delegate void StatDelegate(Stat stat);
        public event StatDelegate OnStatChange;

        public Stat()
        {
            modifiers = new List<IModifier>();
            Modifiers = modifiers.AsReadOnly();
        }

        public Stat(float baseValue, StatFlags type) : this()
        {
            BaseValue = baseValue;
        }

        public Stat(Stat stat) : this()
        {
            BaseValue = stat.BaseValue;
            StatFlags = stat.StatFlags;
            isDirty = stat.isDirty;
            lastBaseValue = stat.lastBaseValue;
            modifiers = stat.modifiers;
            _value = CalculateFinalValue();
        }

        public virtual void AddModifier(IModifier modifier)
        {
            if (!HasStatFlags(modifier.ApplicableStatFlags)) return;
            isDirty = true;
            modifiers.Add(modifier);
            modifiers.Sort(CompareModifierOrder);
        }

        public bool HasStatFlags(StatFlags statFlags)
        {
            return statFlags == StatFlags.None ? false : (StatFlags & statFlags) != 0;
        }

        protected virtual int CompareModifierOrder(IModifier a, IModifier b)
        {
            if (a.Order < b.Order) return -1;
            else if (a.Order > b.Order) return 1;
            return 0;
        }

        public virtual bool RemoveModifier(IModifier modifier)
        {
            if(modifiers.Remove(modifier))
            {             
                return isDirty = true;
            }
            return isDirty;
        }

        public virtual bool RemoveAllModifiersFromSource(object source)
        {
            bool didRemove = false;

            for (int i = modifiers.Count - 1; i >= 0; i--)
            {
                if (modifiers[i].Source == source)
                {
                    isDirty = true;
                    didRemove = true;
                    modifiers.RemoveAt(i);
                }
            }

            return didRemove;
        }

        protected float CalculateFinalValue()
        {
            float finalValue = BaseValue;
            float sumPercentAdd = 0f;
            for (int i = 0; i < modifiers.Count; i++)
            {
                IModifier modifier = modifiers[i];
                switch (modifier.Type)
                {
                    case StatModifierType.Flat:
                        finalValue += modifier.Value;
                        break;
                    case StatModifierType.PercentMultiply:
                        finalValue *= 1f + modifier.Value;
                        break;
                    case StatModifierType.PercentAdd:
                        sumPercentAdd += modifier.Value;
                        if (i + 1 >= modifiers.Count || modifiers[i + 1].Type != StatModifierType.PercentAdd)
                        {
                            finalValue *= 1 + sumPercentAdd;
                            sumPercentAdd = 0f;
                        }
                        break;
                    default:
                        finalValue += modifier.Value;
                        break;
                }
            }

            return (float)Math.Round(finalValue, 4);
        }

        protected float GetValue()
        {
            if (isDirty || BaseValue != lastBaseValue)
            {
                lastBaseValue = BaseValue;
                _value = CalculateFinalValue();
                isDirty = false;
                OnStatChange?.Invoke(this);
            }
            return _value;        
        }
    }
}