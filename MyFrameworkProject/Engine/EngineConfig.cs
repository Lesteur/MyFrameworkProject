namespace MyFrameworkProject.Engine
{
    public static class EngineConfig
    {
        // Internal virtual resolution used for rendering
        public const int VirtualWidth = 640;
        public const int VirtualHeight = 360;

        // Window dimensions
        public const int WindowWidth = 1280;
        public const int WindowHeight = 720;

        // Frame rate settings
        public const bool VSync = true;
        public const bool IsFixedTimeStep = true;
        public const int TargetFPS = 60;
    }
}