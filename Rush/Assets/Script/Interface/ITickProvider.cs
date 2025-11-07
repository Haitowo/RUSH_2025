using System;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 06/11/2025 - Beginning of the class

namespace Com.IsartDigital.Rush.Ticks
{

    public interface ITickProvider
    {
        float RatioTimeTick { get; }
        float TickSpeed { get; set; }
        event Action TickEvent;
    }
}
