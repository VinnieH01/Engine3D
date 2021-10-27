using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Engine3D.Math;

namespace Engine3D
{
    static class Input
    {
        private static Dictionary<string, bool> keys = new Dictionary<string, bool>();
        public static Vector2 MouseDelta { get; private set; }

        public static void Update()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            keys["w"] = keyboardState.IsKeyDown(Keys.W);
            keys["a"] = keyboardState.IsKeyDown(Keys.A);
            keys["s"] = keyboardState.IsKeyDown(Keys.S);
            keys["d"] = keyboardState.IsKeyDown(Keys.D);
            keys["q"] = keyboardState.IsKeyDown(Keys.Q);
            keys["e"] = keyboardState.IsKeyDown(Keys.E);

            MouseDelta = new Vector2(Mouse.GetState().Position.X, Mouse.GetState().Position.Y) * 0.1f;

            Mouse.SetPosition(0, 0);
        }

        public static bool GetKey(string key)
        {
            return keys[key];
        }
    }
}
