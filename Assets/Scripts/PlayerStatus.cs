using System;
using Player.Interfaces;

namespace Player.Components
{
    public class PlayerStatus : IPlayerStatus
    {
        int _oxygen;

        public int oxygen
        {
            get => _oxygen;
            set
            {
                _oxygen = value;
                OnOxygenChanged.Invoke(_oxygen);
            }
        }

        public event Action<int> OnOxygenChanged;
    }
}