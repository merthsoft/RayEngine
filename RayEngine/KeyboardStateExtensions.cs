using Microsoft.Xna.Framework.Input;
using System.Linq;

namespace RayEngine
{
    public static class KeyboardStateExtensions
    {
        public static bool WasKeyJustPressed(this KeyboardState currentState, KeyboardState previousState, params Keys[] keys)
            => keys.Any(key => currentState.IsKeyDown(key) && !previousState.IsKeyDown(key));

        public static bool IsKeyDown(this KeyboardState currentState, params Keys[] keys)
            => keys.Any(key => currentState.IsKeyDown(key));
    }
}
