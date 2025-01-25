using System;

namespace Player.Interfaces
{
    public interface IPlayerStatus
    {
        int oxygen { get; set; }
        event Action<int> OnOxygenChanged;
    }
}