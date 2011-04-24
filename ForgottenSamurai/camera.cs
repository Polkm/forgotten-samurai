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
            cameraFarClip = 160.0f;
        }

        public void Update()
        {
            Camera.cameraPos = Game.player1.position + new Vector3(0, Game.player1.height, 0);

            float deltaX = (System.Windows.Forms.Cursor.Position.X - (Game.bounds.Left + (Game.bounds.Width / 2))) * 0.002f;
            float deltaY = (System.Windows.Forms.Cursor.Position.Y - (Game.bounds.Top + (Game.bounds.Height / 2))) * 0.002f;
            Camera.cameraLookAngle.X += deltaX;
            Camera.cameraLookAngle.Y -= deltaY;
            if (Camera.cameraLookAngle.Y < -Math.PI / 2 + 0.0001f)
                Camera.cameraLookAngle.Y = (float)-Math.PI / 2 + 0.0001f;
            Camera.cameraLookVector = (new Vector3((float)Math.Cos(Camera.cameraLookAngle.X), 0, (float)Math.Sin(Camera.cameraLookAngle.X)) * (float)Math.Cos(Camera.cameraLookAngle.Y)) + new Vector3(0, (float)Math.Sin(Camera.cameraLookAngle.Y), 0);
            Camera.cameraLookPos = Camera.cameraPos + Camera.cameraLookVector;

            System.Windows.Forms.Cursor.Position = new System.Drawing.Point(Game.bounds.Left + (Game.bounds.Width / 2), Game.bounds.Top + (Game.bounds.Height / 2));
        }

        public static Vector3 Get2Dto3D(int x, int y)
        {
            int[] viewport = new int[4];
            Matrix4 modelviewMatrix, projectionMatrix;
            GL.GetFloat(GetPName.ModelviewMatrix, out modelviewMatrix);
            GL.GetFloat(GetPName.ProjectionMatrix, out projectionMatrix);
            GL.GetInteger(GetPName.Viewport, viewport);

            // get depth of clicked pixel
            float[] t = new float[1];
            GL.ReadPixels(x, viewport[3] - y, 1, 1, OpenTK.Graphics.OpenGL.PixelFormat.DepthComponent, PixelType.Float, t);

            return UnProject(new Vector3(x, viewport[3] - y, t[0]), modelviewMatrix, projectionMatrix, viewport);
        }

        public static Vector3 UnProject(Vector3 screen, Matrix4 view, Matrix4 projection, int[] view_port)
        {
            Vector4 pos = new Vector4();

            // Map x and y from window coordinates, map to range -1 to 1 
            pos.X = (screen.X - (float)view_port[0]) / (float)view_port[2] * 2.0f - 1.0f;
            pos.Y = (screen.Y - (float)view_port[1]) / (float)view_port[3] * 2.0f - 1.0f;
            pos.Z = screen.Z * 2.0f - 1.0f;
            pos.W = 1.0f;

            Vector4 pos2 = Vector4.Transform(pos, Matrix4.Invert(Matrix4.Mult(view, projection)));
            Vector3 pos_out = new Vector3(pos2.X, pos2.Y, pos2.Z);

            return pos_out / pos2.W;
        }

        /*public static Vector3 GetDepthPos(int x, int y)
        {
		    int[] viewport = new int[4];
		    Matrix4d modelview = new Matrix4d();
            Matrix4d projection = new Matrix4d();
            Vector3 winPos = Vector3.Zero;

            GL.GetDouble(GetPName.ModelviewMatrix, out modelview);
            GL.GetDouble(GetPName.ProjectionMatrix, out projection);
            GL.GetInteger(GetPName.Viewport, viewport);

		    winPos.X = (float)x;
		    winPos.Y = (float)viewport[3] - (float)y;

            GL.ReadPixels(x, (int)winPos.Y, 1, 1, PixelFormat.DepthComponent, PixelType.Float, ref winPos.Z);
            
            return Unproject((Vector3d)winPos, projection, modelview, viewport);
        }

        public static Vector3 Unproject(Vector3d source, Matrix4d projection, Matrix4d modelview, int[] viewPort)
        {
            
            Matrix4d matrix = Matrix4d.Invert(Matrix4d.Mult(modelview, projection));
            source.X = (((source.X - viewPort[0]) / ((float)viewPort[2])) * 2f) - 1f;
            source.Y = -((((source.Y - viewPort[1]) / ((float)viewPort[3])) * 2f) - 1f);
            source.Z = (source.Z - 1.0f) / (Camera.cameraFarClip - 1.0f);
            Vector3d vector = Vector3d.Transform(source, matrix);
            float a = (float)((((source.X * matrix.M14) + (source.Y * matrix.M24)) + (source.Z * matrix.M34)) + matrix.M44);
            if (!WithinEpsilon(a, 1f))
            {
                vector = (vector / a);
            }
            return (Vector3)vector;
        }
 
        private static bool WithinEpsilon(float a, float b)
        {
            float num = a - b;
            return ((-1.401298E-45f <= num) && (num <= float.Epsilon));
        }*/
    }
}
