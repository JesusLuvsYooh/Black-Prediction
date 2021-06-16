using UnityEngine;

namespace Black.Utility
{
    public static class BlackUtility
    {
        public static void ApplyFixedTimestep(float framesPerSecond)
        {
            Time.fixedDeltaTime = 1.0f / framesPerSecond;
        }
    }
}