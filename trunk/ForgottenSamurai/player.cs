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
            Vector3 worldPos = Camera.Get2Dto3D(Game.mouse.X, Game.mouse.Y);
            Console.WriteLine(worldPos.X + " " + worldPos.Y + " " + worldPos.Z);
            worldPos.X = (float)Math.Floor(worldPos.X);
            worldPos.Y = (float)Math.Floor(worldPos.Y);
            worldPos.Z = (float)Math.Floor(worldPos.Z);
            Vector3 chunkPos = new Vector3(worldPos.X / BlockSystem.size, worldPos.Y / BlockSystem.size, worldPos.Z / BlockSystem.size);
            chunkPos.X = (float)Math.Floor(chunkPos.X);
            chunkPos.Y = (float)Math.Floor(chunkPos.Y);
            chunkPos.Z = (float)Math.Floor(chunkPos.Z);
            Vector3 blockPos = worldPos - (chunkPos * 16);
            
            Console.WriteLine(blockPos.X + " " + blockPos.Y + " " + blockPos.Z);
            if (chunkPos.X >= 0 && chunkPos.X < Terrain.size && chunkPos.Y >= 0 && chunkPos.Y < Terrain.height && chunkPos.Z >= 0 && chunkPos.Z < Terrain.size)
                Terrain.chunks[(int)chunkPos.X, (int)chunkPos.Y, (int)chunkPos.Z].RemoveBlock((int)blockPos.X, (int)blockPos.Y, (int)blockPos.Z);
        }

        public void RightClick()
        {
        }
    }
}
