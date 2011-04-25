using System;
using System.Collections;
using System.Collections.Generic;

using BulletSharp;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using OpenTK.Input;

namespace ForgottenSamurai
{
    class BlockSystem
    {
        public Vector3 position;
        public Vector3 chunkPos;
        public static int size;
        public static float radius;
        public byte[, ,] blockIDs;

        uint[] VBOid;
        float[] vertices;
        int vertCount;
        float[] colors;

        List<RigidBody> physicsBoxes;

        public BlockSystem()
        {
            position = new Vector3(0, 0, 0);
            size = 16;
            radius = (float)Math.Sqrt(2.0 * Math.Pow((double)size, 2.0));
            blockIDs = new byte[size, size, size];
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    for (int z = 0; z < size; z++)
                    {
                        blockIDs[x, y, z] = (byte)Math.Round(Terrain.random.NextDouble() * 3);
                    }
                }
            }
            VBOid = new uint[2];
            GL.GenBuffers(2, VBOid);  // generate a new VBO and get the associated ID
            physicsBoxes = new List<RigidBody>();
        }

        public void GenerateVertexData()
        {
            byte currentType;
            bool front, back, left, right, top, bottom;

            vertices = new float[589824];
            colors = new float[589824];
            //List<float> tempColors = new List<float>();

            float[] v110, v100, v000, v010, v011, v001, v101, v111, color;
            color = new float[] { 0.0f, 0.0f, 0.0f };

            /*foreach (RigidBody body in physicsBoxes)
                Game.physics.World.RemoveRigidBody(body);
            physicsBoxes.Clear();*/

            vertCount = 0;
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    for (int z = 0; z < size; z++)
                    {
                        currentType = blockIDs[x, y, z];
                        if (currentType != 0)
                        {
                            front = false;
                            back = false;
                            left = false;
                            right = false;
                            top = false;
                            bottom = false;

                            if ((z - 1 >= 0 && blockIDs[x, y, z - 1] == 0) ||
                                (z - 1 < 0 && chunkPos.Z - 1 >= 0 && Terrain.chunks[(int)chunkPos.X][(int)chunkPos.Y][(int)chunkPos.Z - 1].blockIDs[x, y, size - 1] == 0))
                                front = true;
                            if ((z + 1 < size && blockIDs[x, y, z + 1] == 0) ||
                                (z + 1 >= size && chunkPos.Z + 1 < Terrain.size && Terrain.chunks[(int)chunkPos.X][(int)chunkPos.Y][(int)chunkPos.Z + 1].blockIDs[x, y, 0] == 0))
                                back = true;
                            if ((x + 1 < size && blockIDs[x + 1, y, z] == 0) ||
                                (x + 1 >= size && chunkPos.X + 1 < Terrain.size && Terrain.chunks[(int)chunkPos.X + 1][(int)chunkPos.Y][(int)chunkPos.Z].blockIDs[0, y, z] == 0))
                                left = true;
                            if ((x - 1 >= 0 && blockIDs[x - 1, y, z] == 0) ||
                                (x - 1 < 0 && chunkPos.X - 1 >= 0 && Terrain.chunks[(int)chunkPos.X - 1][(int)chunkPos.Y][(int)chunkPos.Z].blockIDs[size - 1, y, z] == 0))
                                right = true;
                            if ((y + 1 >= size && (position.Y / size) + 1 >= Terrain.height) || (y + 1 < size && blockIDs[x, y + 1, z] == 0))
                                top = true;
                            if ((y - 1 >= 0 && blockIDs[x, y - 1, z] == 0))
                                bottom = true;

                            if (front || back || left || right || top || bottom)
                            {
                                v110 = new float[] { x + 1, y + 1, z + 0 };
                                v100 = new float[] { x + 1, y + 0, z + 0 };
                                v000 = new float[] { x + 0, y + 0, z + 0 };
                                v010 = new float[] { x + 0, y + 1, z + 0 };
                                v011 = new float[] { x + 0, y + 1, z + 1 };
                                v001 = new float[] { x + 0, y + 0, z + 1 };
                                v101 = new float[] { x + 1, y + 0, z + 1 };
                                v111 = new float[] { x + 1, y + 1, z + 1 };

                                if (currentType == 1)
                                    color = new float[] { .266f, .349f, .145f };
                                if (currentType == 2)
                                    color = new float[] { .568f, .651f, .274f };
                                if (currentType == 3)
                                    color = new float[] { .749f, .686f, .561f };
                                if (currentType == 4)
                                    color = new float[] { 1.0f, 1.0f, 0.0f };
                                if (currentType == 5)
                                    color = new float[] { 0.0f, 1.0f, 1.0f };
                                if (currentType == 6)
                                    color = new float[] { 1.0f, 0.0f, 1.0f };

                                if (front)
                                    AddFace(v110, v100, v000, v010, color, 0.7f);
                                if (back)
                                    AddFace(v011, v001, v101, v111, color, 0.8f);
                                if (left)
                                    AddFace(v110, v111, v101, v100, color, 0.85f);
                                if (right)
                                    AddFace(v011, v010, v000, v001, color, 0.75f);
                                if (top)
                                    AddFace(v111, v110, v010, v011, color, 1.0f);
                                if (bottom)
                                {
                                    AddFace(v001, v000, v100, v101, color, 0.5f);
                                }

                                //physicsBoxes.Add(Game.physics.AddStaticBlock(position + new Vector3(x, y, z)));
                            }
                        }
                    }
                }
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOid[0]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertCount * sizeof(float)), vertices, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOid[1]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertCount * sizeof(float)), colors, BufferUsageHint.DynamicDraw);
        }

        void AddFace(float[] v1, float[] v2, float[] v3, float[] v4, float[] color, float darkness)
        {
            float[] faceColor = new float[] { color[0] * darkness, color[1] * darkness, color[2] * darkness };
            AddVertex(v1, faceColor);
            AddVertex(v2, faceColor);
            AddVertex(v3, faceColor);
            AddVertex(v1, faceColor);
            AddVertex(v3, faceColor);
            AddVertex(v4, faceColor);
        }

        void AddVertex(float[] v1, float[] color)
        {
            vertices[vertCount] = v1[0];
            vertices[vertCount + 1] = v1[1];
            vertices[vertCount + 2] = v1[2];
            colors[vertCount] = color[0];
            colors[vertCount + 1] = color[1];
            colors[vertCount + 2] = color[2];
            vertCount += 3;
        }

        public void Draw()
        {
            
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            GL.Translate(position);

            // bind VBOs for vertex array and index array
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOid[0]);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.VertexPointer(3, VertexPointerType.Float, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOid[1]);
            GL.EnableClientState(ArrayCap.ColorArray);
            GL.ColorPointer(3, ColorPointerType.Float, 0, 0);

            GL.DrawArrays(BeginMode.Triangles, 0, vertCount);

            GL.DisableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.ColorArray);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.PopMatrix();
        }


        public void RemoveBlock(int x, int y, int z)
        {
            blockIDs[x, y, z] = 0;
            GenerateVertexData();

            if (x == 0 && chunkPos.X > 0)
                Terrain.chunks[(int)chunkPos.X - 1][(int)chunkPos.Y][(int)chunkPos.Z].GenerateVertexData();
            if (y == 0 && chunkPos.Y > 0)
                Terrain.chunks[(int)chunkPos.X][(int)chunkPos.Y - 1][(int)chunkPos.Z].GenerateVertexData();
            if (z == 0 && chunkPos.Z > 0)
                Terrain.chunks[(int)chunkPos.X][(int)chunkPos.Y][(int)chunkPos.Z - 1].GenerateVertexData();

            if (x == BlockSystem.size - 1 && chunkPos.X < Terrain.size - 1)
                Terrain.chunks[(int)chunkPos.X + 1][(int)chunkPos.Y][(int)chunkPos.Z].GenerateVertexData();
            if (y == BlockSystem.size - 1 && chunkPos.Y < Terrain.height - 1)
                Terrain.chunks[(int)chunkPos.X][(int)chunkPos.Y + 1][(int)chunkPos.Z].GenerateVertexData();
            if (z == BlockSystem.size - 1 && chunkPos.Z < Terrain.size - 1)
                Terrain.chunks[(int)chunkPos.X][(int)chunkPos.Y][(int)chunkPos.Z + 1].GenerateVertexData();
        }
        public void AddBlock(int x, int y, int z, byte type)
        {
            AddBlock(x, y, z, type, true);
        }
        public void AddBlock(int x, int y, int z, byte type, bool generate)
        {
            blockIDs[x, y, z] = type;
            GenerateVertexData();

            if (generate)
            {
                if (x == 0 && chunkPos.X > 0)
                    Terrain.chunks[(int)chunkPos.X - 1][(int)chunkPos.Y][(int)chunkPos.Z].GenerateVertexData();
                if (y == 0 && chunkPos.Y > 0)
                    Terrain.chunks[(int)chunkPos.X][(int)chunkPos.Y - 1][(int)chunkPos.Z].GenerateVertexData();
                if (z == 0 && chunkPos.Z > 0)
                    Terrain.chunks[(int)chunkPos.X][(int)chunkPos.Y][(int)chunkPos.Z - 1].GenerateVertexData();

                if (x == BlockSystem.size - 1 && chunkPos.X < Terrain.size - 1)
                    Terrain.chunks[(int)chunkPos.X + 1][(int)chunkPos.Y][(int)chunkPos.Z].GenerateVertexData();
                if (y == BlockSystem.size - 1 && chunkPos.Y < Terrain.height - 1)
                    Terrain.chunks[(int)chunkPos.X][(int)chunkPos.Y + 1][(int)chunkPos.Z].GenerateVertexData();
                if (z == BlockSystem.size - 1 && chunkPos.Z < Terrain.size - 1)
                    Terrain.chunks[(int)chunkPos.X][(int)chunkPos.Y][(int)chunkPos.Z + 1].GenerateVertexData();
            }
        }

    }
}
