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
        public static System.Drawing.Rectangle bounds;
        public static Camera camera;
        public static player player1;
        public Terrain terrain;

        public static MouseDevice mouse;

        public Game() : base(800, 600, GraphicsMode.Default, "Forgotten Samurai")
        {
            VSync = VSyncMode.On;
            GL.Enable(EnableCap.CullFace);
            terrain = new Terrain();
            player1 = new player();
            player1.position = new Vector3(Terrain.size / 2 * BlockSystem.size, BlockSystem.size * Terrain.height, Terrain.size / 2 * BlockSystem.size);
            camera = new Camera();
            ResumeGame();

            base.Mouse.ButtonDown += new EventHandler<MouseButtonEventArgs>(Game_ButtonDown);
            base.KeyPress += new EventHandler<KeyPressEventArgs>(Game_KeyPress);
        }

        void Game_ButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Left)
            {
                player1.LeftClick();
            }
            if (e.Button == MouseButton.Right)
            {
                player1.RightClick();
            }
        }

        void Game_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Keyboard[Key.Tilde])
            {
                if (!gamePaused)
                    PauseGame();
                else if (gamePaused)
                    ResumeGame();
            }
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
            bounds = Bounds;
            mouse = Mouse;

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

                if (Mouse[MouseButton.Left])
                    player1.LeftPress();

                camera.Update();
            }
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
