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
        public Vector3 velocity;
        public Vector3 gravity;
        
        public player()
        {
            position = Vector3.Zero;
            height = 4.0f;
            velocity = new Vector3(0.0f, 0.0f, 0.0f); // m/s
            gravity = new Vector3(0.0f, -9.81f, 0.0f); // m/s/s
        }

        public void UpdatePhysics()
        {

        }

        public void LeftPress()
        {
            //LeftClick();
        }
        public void LeftClick()
        {
            Vector3 worldPos = Camera.Get2Dto3D(Game.mouse.X, Game.mouse.Y);
            Vector3 mainAxis = GetAxisAndRoundWorld(ref worldPos);

            int block1 = 0;
            Vector3 chunkPos1 = new Vector3(worldPos.X / BlockSystem.size, worldPos.Y / BlockSystem.size, worldPos.Z / BlockSystem.size);
            chunkPos1 = FloorVector(chunkPos1);
            Vector3 blockPos1 = worldPos - (chunkPos1 * BlockSystem.size);
            block1 = GetBlockID(chunkPos1, blockPos1);

            int block2 = 0;
            Vector3 newWordPos = worldPos - mainAxis;
            Vector3 chunkPos2 = new Vector3(newWordPos.X / BlockSystem.size, newWordPos.Y / BlockSystem.size, newWordPos.Z / BlockSystem.size);
            chunkPos2 = FloorVector(chunkPos2);
            Vector3 blockPos2 = newWordPos - (chunkPos2 * BlockSystem.size);
            block2 = GetBlockID(chunkPos2, blockPos2);

            if (block1 != 0 && block1 != -1)
                Terrain.chunks[(int)chunkPos1.X][(int)chunkPos1.Y][(int)chunkPos1.Z].RemoveBlock((int)blockPos1.X, (int)blockPos1.Y, (int)blockPos1.Z);
            if (block2 != 0 && block2 != -1)
                Terrain.chunks[(int)chunkPos2.X][(int)chunkPos2.Y][(int)chunkPos2.Z].RemoveBlock((int)blockPos2.X, (int)blockPos2.Y, (int)blockPos2.Z);
            //Console.WriteLine(blockPos.X + " " + blockPos.Y + " " + blockPos.Z);
        }

        public void RightClick()
        {
            Vector3 worldPos = Camera.Get2Dto3D(Game.mouse.X, Game.mouse.Y);
            Vector3 mainAxis = GetAxisAndRoundWorld(ref worldPos);

            int block1 = 0;
            Vector3 chunkPos1 = new Vector3(worldPos.X / BlockSystem.size, worldPos.Y / BlockSystem.size, worldPos.Z / BlockSystem.size);
            chunkPos1 = FloorVector(chunkPos1);
            Vector3 blockPos1 = worldPos - (chunkPos1 * BlockSystem.size);
            block1 = GetBlockID(chunkPos1, blockPos1);

            int block2 = 0;
            Vector3 newWordPos = worldPos - mainAxis;
            Vector3 chunkPos2 = new Vector3(newWordPos.X / BlockSystem.size, newWordPos.Y / BlockSystem.size, newWordPos.Z / BlockSystem.size);
            chunkPos2 = FloorVector(chunkPos2);
            Vector3 blockPos2 = newWordPos - (chunkPos2 * BlockSystem.size);
            block2 = GetBlockID(chunkPos2, blockPos2);

            if (block1 == 0 && block1 != -1)
                Terrain.chunks[(int)chunkPos1.X][(int)chunkPos1.Y][(int)chunkPos1.Z].AddBlock((int)blockPos1.X, (int)blockPos1.Y, (int)blockPos1.Z, 1);
            if (block2 == 0 && block2 != -1)
                Terrain.chunks[(int)chunkPos2.X][(int)chunkPos2.Y][(int)chunkPos2.Z].AddBlock((int)blockPos2.X, (int)blockPos2.Y, (int)blockPos2.Z, 1);
        }

        Vector3 GetAxisAndRoundWorld(ref Vector3 worldPos)
        {
            Vector3 axisCloseness = ClosesnessVector(worldPos);
            Vector3 mainAxis = Vector3.Zero;
            if (axisCloseness.X < axisCloseness.Y && axisCloseness.X < axisCloseness.Z)
            {
                mainAxis = Vector3.UnitX;
                worldPos.X = (float)Math.Round(worldPos.X);
                worldPos.Y = (float)Math.Floor(worldPos.Y);
                worldPos.Z = (float)Math.Floor(worldPos.Z);
            }
            if (axisCloseness.Y < axisCloseness.X && axisCloseness.Y < axisCloseness.Z)
            {
                mainAxis = Vector3.UnitY;
                worldPos.X = (float)Math.Floor(worldPos.X);
                worldPos.Y = (float)Math.Round(worldPos.Y);
                worldPos.Z = (float)Math.Floor(worldPos.Z);
            }
            if (axisCloseness.Z < axisCloseness.Y && axisCloseness.Z < axisCloseness.X)
            {
                mainAxis = Vector3.UnitZ;
                worldPos.X = (float)Math.Floor(worldPos.X);
                worldPos.Y = (float)Math.Floor(worldPos.Y);
                worldPos.Z = (float)Math.Round(worldPos.Z);
            }
            return mainAxis;
        }

        int GetBlockID(Vector3 chunkPos, Vector3 blockPos)
        {
            if (chunkPos.X >= 0 && chunkPos.X < Terrain.size && chunkPos.Y >= 0 && chunkPos.Y < Terrain.height && chunkPos.Z >= 0 && chunkPos.Z < Terrain.size)
                if (blockPos.X >= 0 && blockPos.X < BlockSystem.size && blockPos.Y >= 0 && blockPos.Y < BlockSystem.size && blockPos.Z >= 0 && blockPos.Z < BlockSystem.size)
                    return Terrain.chunks[(int)chunkPos.X][(int)chunkPos.Y][(int)chunkPos.Z].blockIDs[(int)blockPos.X, (int)blockPos.Y, (int)blockPos.Z];
            return -1;
        }

        Vector3 FloorVector(Vector3 vector)
        {
            Vector3 rounded = vector;
            rounded.X = (float)Math.Floor(rounded.X);
            rounded.Y = (float)Math.Floor(rounded.Y);
            rounded.Z = (float)Math.Floor(rounded.Z);
            return rounded;
        }

        Vector3 ClosesnessVector(Vector3 input)
        {
            return new Vector3(Closeness(input.X), Closeness(input.Y), Closeness(input.Z));
        }

        float Closeness(float input)
        {
            return (float)Math.Abs(input - Math.Round(input));
        }
    }
}
