using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BulletSharp;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using OpenTK.Input;

namespace ForgottenSamurai
{
    class block
    {
        public RigidBody body;
        Vector3 size;

        public block(Vector3 pos)
        {
            size = Vector3.One;
            body = Game.physics.AddDynamicBlock(pos);
            Game.blocks.Add(this);
        }

        public void Remove()
        {
            Game.blocks.Remove(this);
            Game.physics.World.RemoveRigidBody(body);
        }

        public void Draw()
        {
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();

		    Matrix4 modelview = new Matrix4();
            GL.GetFloat(GetPName.ModelviewMatrix, out modelview);

            Matrix4 matrix = body.MotionState.WorldTransform * modelview;
            GL.LoadMatrix(ref matrix);

            if (body.IsActive)
                DrawCube(Color4.Orange);
            else
                DrawCube(Color4.Gray);

            GL.PopMatrix();
        }

        private void DrawCube(Color4 color)
        {
            DrawCube(color, 0.5f);
        }

        private void DrawCube(Color4 color, float size)
        {
            GL.Begin(BeginMode.Quads);

            GL.Color4(color);
            GL.Vertex3(-size, -size, -size);
            GL.Vertex3(-size, size, -size);
            GL.Vertex3(size, size, -size);
            GL.Vertex3(size, -size, -size);

            GL.Vertex3(-size, -size, -size);
            GL.Vertex3(size, -size, -size);
            GL.Vertex3(size, -size, size);
            GL.Vertex3(-size, -size, size);

            GL.Vertex3(-size, -size, -size);
            GL.Vertex3(-size, -size, size);
            GL.Vertex3(-size, size, size);
            GL.Vertex3(-size, size, -size);

            GL.Vertex3(-size, -size, size);
            GL.Vertex3(size, -size, size);
            GL.Vertex3(size, size, size);
            GL.Vertex3(-size, size, size);

            GL.Vertex3(-size, size, -size);
            GL.Vertex3(-size, size, size);
            GL.Vertex3(size, size, size);
            GL.Vertex3(size, size, -size);

            GL.Vertex3(size, -size, -size);
            GL.Vertex3(size, size, -size);
            GL.Vertex3(size, size, size);
            GL.Vertex3(size, -size, size);

            GL.End();
        }
    }
}
