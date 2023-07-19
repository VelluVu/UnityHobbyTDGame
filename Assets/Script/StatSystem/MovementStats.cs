using UnityEngine;

namespace TheTD.StatSystem
{

    [System.Serializable]
    public class MovementStats
    {
        [SerializeField] private Stat _moveSpeed = null;
        public Stat MoveSpeed { get => _moveSpeed; }

        [SerializeField] private Stat _turnSpeed = null;
        public Stat TurnSpeed { get => _turnSpeed; }

        [SerializeField] private Stat _acceleration = null;
        public Stat Acceleration { get => _acceleration; }

        public MovementStats(Stat moveSpeed, Stat turnSpeed, Stat acceleration)
        {
            _moveSpeed = new Stat(moveSpeed);
            _turnSpeed = new Stat(turnSpeed);
            _acceleration = new Stat(acceleration);
        }

        public void ResetToBaseValue(float moveSpeed, float turnSpeed, float acceleration)
        {
            _moveSpeed.BaseValue = moveSpeed;
            _turnSpeed.BaseValue = turnSpeed;
            _acceleration.BaseValue = acceleration;
        }
    }
}