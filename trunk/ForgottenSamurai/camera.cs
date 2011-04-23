using System;
using System.Windows;
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

        public void Update()
        {
            Camera.cameraPos = Game.player1.position + new Vector3(0, Game.player1.height, 0);

            float deltaX = (System.Windows.Forms.Cursor.Position.X - (Game.bounds.Left + (Game.bounds.Width / 2))) * 0.002f;
            float deltaY = (System.Windows.Forms.Cursor.Position.Y - (Game.bounds.Top + (Game.bounds.Height / 2))) * 0.002f;
            Camera.cameraLookAngle.X += deltaX;
            Camera.cameraLookAngle.Y -= deltaY;
            Camera.cameraLookVector = (new Vector3((float)Math.Cos(Camera.cameraLookAngle.X), 0, (float)Math.Sin(Camera.cameraLookAngle.X)) * (float)Math.Cos(Camera.cameraLookAngle.Y)) + new Vector3(0, (float)Math.Sin(Camera.cameraLookAngle.Y), 0);
            Camera.cameraLookPos = Camera.cameraPos + Camera.cameraLookVector;

            System.Windows.Forms.Cursor.Position = new System.Drawing.Point(Game.bounds.Left + (Game.bounds.Width / 2), Game.bounds.Top + (Game.bounds.Height / 2));
        }
    }
}
