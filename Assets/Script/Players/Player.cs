using UnityEngine;

namespace TheTD.Players
{

    public class Player : MonoBehaviour, IEventListener
    {
        [SerializeField] private Gold _gold = null;
        public Gold Gold { get => _gold; }

        [SerializeField] private Life _life = null;
        public Life Life { get => _life; }

        public delegate void PlayerDelegate(Player player);
        static public event PlayerDelegate OnTakeDamage;
        static public event PlayerDelegate OnDeath;
        static public event PlayerDelegate OnHeal;
        static public event PlayerDelegate OnMaxedLife;
        static public event PlayerDelegate OnInitialized;
        static public event PlayerDelegate OnSpendGold;
        static public event PlayerDelegate OnGainGold;

        private void Start()
        {
            InitializePlayer();
        }

        private void InitializePlayer()
        {
            AddListeners();
            OnInitialized?.Invoke(this);
        }

        public void AddListeners()
        {
            Gold.AddListeners();
            Life.AddListeners();
            Life.OnZero += OnZeroLife;
            Life.OnRemove += OnLoseLife;
            Life.OnMaxLife += OnMaxLife;
            Life.OnHeal += OnHealLife;
            Gold.OnGain += onGainGold;
            Gold.OnSpend += onSpendGold;
        }

        public void RemoveListeners()
        {
            Gold.RemoveListeners();
            Life.RemoveListeners();
            Life.OnZero -= OnZeroLife;
            Life.OnRemove -= OnLoseLife;
            Life.OnMaxLife -= OnMaxLife;
            Life.OnHeal -= OnHealLife;
            Gold.OnGain -= onGainGold;
            Gold.OnSpend -= onSpendGold;
        }

        private void onSpendGold(Gold gold)
        {
            OnSpendGold?.Invoke(this);
        }

        private void onGainGold(Gold gold)
        {
            OnGainGold?.Invoke(this);
        }

        private void OnHealLife(Life life)
        {
            OnHeal?.Invoke(this);
        }

        private void OnMaxLife(Life life)
        {
            OnMaxedLife?.Invoke(this);
        }

        private void OnLoseLife(Life life)
        {
            OnTakeDamage?.Invoke(this);
        }

        private void OnZeroLife(Life life)
        {
            OnDeath?.Invoke(this);
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }
    }
}