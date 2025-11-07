// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 06/11/2025 - Beginning of the class

namespace Com.IsartDigital.Rush.Ticks
{
    
    public static class TickProviderLocator
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        public static ITickProvider Instance { get; private set; }

        public static void Register(ITickProvider pProvider)
        {
            Instance = pProvider;
        }
    }
}
