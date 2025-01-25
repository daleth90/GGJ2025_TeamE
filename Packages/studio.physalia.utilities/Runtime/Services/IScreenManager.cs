using System;

namespace Physalia
{
    public interface IScreenManager
    {
        event Action<ScreenData> ScreenChanged;

        ScreenData ScreenData { get; }
        OrientationData OrientationData { get; }
    }
}
