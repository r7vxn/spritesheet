using System;

namespace spritesheet
{
    // Simple coin/currency tracker. Keeps a running balance and exposes methods
    // to add/spend coins. Fires an event when the balance changes so UI or
    // game systems can react.
    public class Coin
    {
        public int Amount { get; private set; }

        public event Action<int>? OnChanged;

        public Coin(int initialAmount = 0)
        {
            Amount = initialAmount;
        }

        public void Add(int value)
        {
            if (value <= 0) return;
            Amount += value;
            OnChanged?.Invoke(Amount);
        }

        public bool TrySpend(int value)
        {
            if (value <= 0) return true;
            if (Amount < value) return false;
            Amount -= value;
            OnChanged?.Invoke(Amount);
            return true;
        }

        public void Set(int value)
        {
            Amount = Math.Max(0, value);
            OnChanged?.Invoke(Amount);
        }
    }
}
