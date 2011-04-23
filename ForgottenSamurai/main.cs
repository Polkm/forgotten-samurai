using System;
using System.Windows;

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
        public static bool gamePaused;
        public player player1;
        public Terrain terrain;
        public Camera camera;

        public bool previusKeyTilde;

        public Game() : base(800, 600, GraphicsMode.Default, "Forgotten Samurai")
        {
            VSync = VSyncMode.On;
            GL.Enable(EnableCap.CullFace);
            player1 = new player();
            player1.position = new Vector3(0, 16, 0);
            terrain = new Terrain();
            camera = new Camera();
            previusKeyTilde = false;
            ResumeGame();
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
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(Camera.cameraFOV, Width / (float)Height, 1.0f, Camera.cameraFarClip);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (Keyboard[Key.Escape])
                Exit();

            if (!gamePaused)
            {
                if (Keyboard[Key.A])
                {
                    player1.position.X += (float)Math.Cos(Camera.cameraLookAngle.X - (Math.PI / 2));
                    player1.position.Z += (float)Math.Sin(Camera.cameraLookAngle.X - (Math.PI / 2));
                }
                if (Keyboard[Key.D])
                {
                    player1.position.X -= (float)Math.Cos(Camera.cameraLookAngle.X - (Math.PI / 2));
                    player1.position.Z -= (float)Math.Sin(Camera.cameraLookAngle.X - (Math.PI / 2));
                }

                if (Keyboard[Key.W])
                {
                    player1.position.X += (float)Math.Cos(Camera.cameraLookAngle.X);
                    player1.position.Z += (float)Math.Sin(Camera.cameraLookAngle.X);
                }
                if (Keyboard[Key.S])
                {
                    player1.position.X -= (float)Math.Cos(Camera.cameraLookAngle.X);
                    player1.position.Z -= (float)Math.Sin(Camera.cameraLookAngle.X);
                }

                Camera.cameraPos = player1.position + new Vector3(0, player1.height, 0);
                float deltaX = (System.Windows.Forms.Cursor.Position.X - (Bounds.Left + (Bounds.Width / 2))) * 0.002f;
                float deltaY = (System.Windows.Forms.Cursor.Position.Y - (Bounds.Top + (Bounds.Height / 2))) * 0.002f;
                Camera.cameraLookAngle.X += deltaX;
                Camera.cameraLookAngle.Y -= deltaY;
                Camera.cameraLookVector = (new Vector3((float)Math.Cos(Camera.cameraLookAngle.X), 0, (float)Math.Sin(Camera.cameraLookAngle.X)) * (float)Math.Cos(Camera.cameraLookAngle.Y)) + new Vector3(0, (float)Math.Sin(Camera.cameraLookAngle.Y), 0);
                Camera.cameraLookPos = Camera.cameraPos + Camera.cameraLookVector;

                System.Windows.Forms.Cursor.Position = new System.Drawing.Point(Bounds.Left + (Bounds.Width / 2), Bounds.Top + (Bounds.Height / 2));
            }

            if (Keyboard[Key.Tilde] && !previusKeyTilde)
            {
                if (!gamePaused)
                    PauseGame();
                else if (gamePaused)
                    ResumeGame();
            }

            previusKeyTilde = Keyboard[Key.Tilde];
        }

        public void PauseGame()
        {
            System.Windows.Forms.Cursor.Show();
            gamePaused = true;
            
        }
        public void ResumeGame()
        {
            System.Windows.Forms.Cursor.Hide();
            System.Windows.Forms.Cursor.Position = new System.Drawing.Point(Bounds.Left + (Bounds.Width / 2), Bounds.Top + (Bounds.Height / 2));
            gamePaused = false;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            if (gamePaused)
                GL.ClearColor(new Color4(50, 50, 50, 100));
            else
                GL.ClearColor(new Color4(50, 50, 50, 100));

            Matrix4 modelview = Matrix4.LookAt(Camera.cameraPos, Camera.cameraLookPos, Vector3.UnitY);
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
