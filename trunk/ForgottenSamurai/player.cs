using System;
using System.Collections;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using OpenTK.Input;

namespace ForgottenSamurai
{
    class player
    {
        public Vector3 position;
        public float height;
        
        public player()
        {
            position = Vector3.Zero;
            height = 4.0f;
        }

        public void LeftClick()
        {
        }

        public void RightClick()
        {
        }
    }
}
