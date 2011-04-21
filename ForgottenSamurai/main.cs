using System;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using OpenTK.Input;

namespace ForgottenSamurai
{
    class Game : GameWindow
    {
        public Terrain terrain;
        public static Vector3 cammeraPos;
        public static Vector3 cammeraLookPos;
        public static float cammeraFOV;
        public static float cammeraFarClip;

        public Game() : base(800, 600, GraphicsMode.Default, "Forgotten Samurai")
        {
            VSync = VSyncMode.On;
            GL.Enable(EnableCap.CullFace);
            terrain = new Terrain();
            cammeraPos = new Vector3(0, 20, -30);
            cammeraLookPos = Vector3.UnitZ;
            cammeraFOV = (float)Math.PI / 4;
            cammeraFarClip = 800.0f;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GL.ClearColor(0.1f, 0.3f, 0.6f, 0.0f);
            GL.Enable(EnableCap.DepthTest);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);

            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(cammeraFOV, Width / (float)Height, 1.0f, cammeraFarClip);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);
        }

        int counter = 0;
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (counter < 4096)
            {
                //Random random = new Random();
                //newblockSystem.blocks[counter] = (byte)Math.Round(random.NextDouble() * 6);
                //counter++;
            }

            if (Keyboard[Key.Left])
                cammeraPos.X += 1;
            if (Keyboard[Key.Right])
                cammeraPos.X -= 1;

            if (Keyboard[Key.Up])
                cammeraPos.Y += 1;
            if (Keyboard[Key.Down])
                cammeraPos.Y -= 1;

            if (Keyboard[Key.Escape])
                Exit();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            cammeraLookPos = cammeraPos + Vector3.UnitZ + new Vector3(0, -0.4f, 0);
            Matrix4 modelview = Matrix4.LookAt(cammeraPos, cammeraLookPos, Vector3.UnitY);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);

            terrain.Draw();

            SwapBuffers();
        }

        [STAThread]
        static void Main()
        {
            using (Game game = new Game())
            {
                game.Run(30.0);
            }
        }
    }
}
