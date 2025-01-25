using System;

namespace Bubble
{
    public interface IPlayerStatus
    {
        int oxygen { get; set; }
        event Action<int> OnOxygenChanged;
    }
}