using System;

namespace Bubble
{
    public interface IPlayerOxygenStatus
    {
        int oxygen { get; set; }
        event Action<int> OnOxygenChanged;
    }
}
