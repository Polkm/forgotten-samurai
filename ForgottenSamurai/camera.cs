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
    class Camera
    {
        public Vector3 position = Vector3.Zero;
        public float height = 0f;
        public static Vector3 cameraPos = Vector3.Zero;
        public static Vector3 cameraLookPos = Vector3.Zero;
        public static Vector3 cameraLookVector = Vector3.Zero;
        public static Vector2 cameraLookAngle = Vector2.Zero;
        public static float cameraFOV = 0f;
        public static float cameraFarClip = 0f;

        public Camera()
        {
            cameraPos = new Vector3(0, 20, -30);
            cameraLookVector = Vector3.UnitZ;
            cameraLookAngle = Vector2.Zero;
            cameraFOV = (float)Math.PI / 4;
            cameraFarClip = 800.0f;
        }
    }
}
