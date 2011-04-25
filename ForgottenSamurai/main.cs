using System;
using System.Collections.Generic;
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
        public static Physics physics;
        public static System.Drawing.Rectangle bounds;
        public static Camera camera;
        public static player player1;
        public Terrain terrain;

        public static List<block> blocks;

        public static MouseDevice mouse;

        public Game() : base(800, 600, GraphicsMode.Default, "Forgotten Samurai")
        {
            VSync = VSyncMode.On;
            GL.Enable(EnableCap.CullFace);
            physics = new Physics();
            terrain = new Terrain();
            blocks = new List<block>();
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
            Camera.viewPort[0] = ClientRectangle.X;
            Camera.viewPort[1] = ClientRectangle.Y;
            Camera.viewPort[2] = ClientRectangle.Width;
            Camera.viewPort[3] = ClientRectangle.Height;

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
                physics.Update((float)e.Time);

                block blk;
                for (int x = 0; x < blocks.Count; x++)
                {
                    blk = blocks[x];
                    if (!blk.body.IsActive)
                    {
                        Vector3 pos = blk.body.CenterOfMassPosition;
                        pos.X = (float)Math.Floor(pos.X);
                        pos.Y = (float)Math.Floor(pos.Y);
                        pos.Z = (float)Math.Floor(pos.Z);
                        Vector3 chunkPos = new Vector3(pos.X / BlockSystem.size, pos.Y / BlockSystem.size, pos.Z / BlockSystem.size);
                        chunkPos.X = (float)Math.Floor(chunkPos.X);
                        chunkPos.Y = (float)Math.Floor(chunkPos.Y);
                        chunkPos.Z = (float)Math.Floor(chunkPos.Z);
                        pos = pos - (chunkPos * BlockSystem.size);
                        if (Terrain.IsValidChunk(chunkPos))
                            Terrain.chunks[(int)chunkPos.X][(int)chunkPos.Y][(int)chunkPos.Z].AddBlock((int)pos.X, (int)pos.Y, (int)pos.Z, 1, false);
                        blk.Remove();
                    }
                }

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

            foreach (block blk in blocks)
                blk.Draw();

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
