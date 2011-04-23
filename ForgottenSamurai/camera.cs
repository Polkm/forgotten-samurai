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

        public Vector3 GetDepthPos(int x, int y)
        {
		    int[] viewport = new int[4];
		    double[] modelview = new double[16];
            double[] projection = new double[16];
		    float winX, winY, winZ;
		    double posX, posY, posZ;

            GL.GetDouble(GetPName.ModelviewMatrix, modelview);
            GL.GetDouble(GetPName.ProjectionMatrix, projection);
            GL.GetInteger(GetPName.Viewport, viewport);

		    winX = (float)x;
		    winY = (float)viewport[3] - (float)y;
		    if (z == 1)
			    winZ = 0;
		    glReadPixels(x, int(winY), 1, 1, GL_DEPTH_COMPONENT, GL_FLOAT, &winZ);

		    gluUnProject(winX, winY, winZ, modelview, projection, viewport, &posX, &posY, &posZ);

            return new Vector3((float)posX, (float)posY, (float)posZ);
        }
    }
}
