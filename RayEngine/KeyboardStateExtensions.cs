using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RayEngine
{
    public static class KeyboardStateExtensions
    {
        public static bool WasKeyJustPressed(this KeyboardState currentState, KeyboardState previousState, params Keys[] keys)
            => keys.Aggregate(false, (acc, key) => acc || currentState.IsKeyDown(key) && !previousState.IsKeyDown(key));
    }
}
