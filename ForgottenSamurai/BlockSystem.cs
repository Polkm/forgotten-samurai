﻿using System;
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
    class BlockSystem
    {
        public Vector3 position;
        public Vector3 chunkPos;
        public static int size;
        public static float radius;
        public byte[, ,] blockIDs;

        uint[] VBOid;
        float[] vertices;
        float[] colors;

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
                        blockIDs[x, y, z] = (byte)Math.Round(Terrain.random.NextDouble() * 6);
                    }
                }
            }

            VBOid = new uint[2];
            GL.GenBuffers(2, VBOid);  // generate a new VBO and get the associated ID
        }
        ~BlockSystem()  // destructor
        {
            //GL.DeleteBuffers(2, VBOid);
        }


        Vector3 pos;
        byte currentType;
        byte lastType;
        bool front, back, left, right, top, bottom;
        public void GenerateVertexData()
        {
            Hashtable tempHash = new Hashtable();
            List<float> tempVerts = new List<float>();
            List<float> tempColors = new List<float>();
            List<ushort> tempInds = new List<ushort>();


            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    for (int z = 0; z < size; z++)
                    {
                        front = false;
                        back = false;
                        left = false;
                        right = false;
                        top = false;
                        bottom = false;
                        currentType = blockIDs[x, y, z];
                        if (currentType != 0)
                        {
                            if ((z - 1 >= 0 && blockIDs[x, y, z - 1] == 0) ||
                                (z - 1 < 0 && chunkPos.Z - 1 >= 0 && Terrain.chunks[(int)chunkPos.X, (int)chunkPos.Y, (int)chunkPos.Z - 1].blockIDs[x, y, size - 1] == 0))
                                front = true;
                            if ((z + 1 < size && blockIDs[x, y, z + 1] == 0) ||
                                (z + 1 >= size && chunkPos.Z + 1 < Terrain.size && Terrain.chunks[(int)chunkPos.X, (int)chunkPos.Y, (int)chunkPos.Z + 1].blockIDs[x, y, 0] == 0))
                                back = true;
                            if ((x + 1 < size && blockIDs[x + 1, y, z] == 0) ||
                                (x + 1 >= size && chunkPos.X + 1 < Terrain.size && Terrain.chunks[(int)chunkPos.X + 1, (int)chunkPos.Y, (int)chunkPos.Z].blockIDs[0, y, z] == 0))
                                left = true;
                            if ((x - 1 >= 0 && blockIDs[x - 1, y, z] == 0) ||
                                (x - 1 < 0 && chunkPos.X - 1 >= 0 && Terrain.chunks[(int)chunkPos.X - 1, (int)chunkPos.Y, (int)chunkPos.Z].blockIDs[size - 1, y, z] == 0))
                                right = true;
                            if ((y + 1 >= size && (position.Y / size) + 1 >= Terrain.height) || (y + 1 < size && blockIDs[x, y + 1, z] == 0))
                                top = true;
                            if ((y - 1 >= 0 && blockIDs[x, y - 1, z] == 0))
                                bottom = true;

                            pos = position + new Vector3(x, y, z);
                            float[] v110 = new float[] { pos.X + 1, pos.Y + 1, pos.Z + 0};
                            float[] v100 = new float[] { pos.X + 1, pos.Y + 0, pos.Z + 0};
                            float[] v000 = new float[] { pos.X + 0, pos.Y + 0, pos.Z + 0};
                            float[] v010 = new float[] { pos.X + 0, pos.Y + 1, pos.Z + 0};

                            float[] v011 = new float[] { pos.X + 0, pos.Y + 1, pos.Z + 1};
                            float[] v001 = new float[] { pos.X + 0, pos.Y + 0, pos.Z + 1};
                            float[] v101 = new float[] { pos.X + 1, pos.Y + 0, pos.Z + 1};
                            float[] v111 = new float[] { pos.X + 1, pos.Y + 1, pos.Z + 1};

                            float[] color = new float[] { 0.0f, 0.0f, 0.0f };
                            if (currentType == 1)
                                color = new float[] { 1.0f, 0.0f, 0.0f };
                            if (currentType == 2)
                                color = new float[] { 0.0f, 1.0f, 0.0f };
                            if (currentType == 3)
                                color = new float[] { 0.0f, 0.0f, 1.0f };
                            if (currentType == 4)
                                color = new float[] { 1.0f, 1.0f, 0.0f };
                            if (currentType == 5)
                                color = new float[] { 0.0f, 1.0f, 1.0f };
                            if (currentType == 6)
                                color = new float[] { 1.0f, 0.0f, 1.0f };

                            //Indices
                            if (front)
                            {
                                AddFace(tempVerts, tempColors, v110, v100, v000, v010, color, 0.7f);
                            }
                            if (back)
                            {
                                AddFace(tempVerts, tempColors, v011, v001, v101, v111, color, 0.8f);
                            }
                            if (left)
                            {
                                AddFace(tempVerts, tempColors, v110, v111, v101, v100, color, 0.85f);
                            }
                            if (right)
                            {
                                AddFace(tempVerts, tempColors, v011, v010, v000, v001, color, 0.75f);
                            }
                            if (top)
                            {
                                AddFace(tempVerts, tempColors, v111, v110, v010, v011, color, 1.0f);
                            }
                            if (bottom)
                            {
                                AddFace(tempVerts, tempColors, v001, v000, v100, v101, color, 0.5f);
                            }
                        }
                    }
                }
            }

            vertices = tempVerts.ToArray();
            colors = tempColors.ToArray();

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOid[0]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * sizeof(float)), vertices, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOid[1]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(colors.Length * sizeof(float)), colors, BufferUsageHint.DynamicDraw);
        }

        void AddFace(List<float> verts, List<float> colors, float[] v1, float[] v2, float[] v3, float[] v4, float[] color, float darkness)
        {
            float[] faceColor = new float[] { color[0] * darkness, color[1] * darkness, color[2] * darkness };
            verts.AddRange(v1);
            colors.AddRange(faceColor);
            verts.AddRange(v2);
            colors.AddRange(faceColor);
            verts.AddRange(v3);
            colors.AddRange(faceColor);
            verts.AddRange(v4);
            colors.AddRange(faceColor);
        }

        public void RemoveBlock(int x, int y, int z)
        {
            blockIDs[x, y, z] = 0;
            GenerateVertexData();
        }

        Color4 color;
        public void Draw()
        {
            // bind VBOs for vertex array and index array
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOid[0]);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.VertexPointer(3, VertexPointerType.Float, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOid[1]);
            GL.EnableClientState(ArrayCap.ColorArray);
            GL.ColorPointer(3, ColorPointerType.Float, 0, 0);

            GL.DrawArrays(BeginMode.Quads, 0, vertices.Length);

            GL.DisableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.ColorArray);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);












            if (false)
            {
                GL.Begin(BeginMode.Quads);

                currentType = 0;
                lastType = 0;

                for (int x = 0; x < size; x++)
                {
                    for (int y = 0; y < size; y++)
                    {
                        for (int z = 0; z < size; z++)
                        {
                            currentType = blockIDs[x, y, z];
                            if (currentType != 0)
                            {
                                pos = position + new Vector3(x, y, z);
                                if (lastType != currentType)
                                {
                                    if (currentType == 1)
                                        color = new Color4(1.0f, 0.0f, 0.0f, 1.0f);
                                    if (currentType == 2)
                                        color = new Color4(0.0f, 1.0f, 0.0f, 1.0f);
                                    if (currentType == 3)
                                        color = new Color4(0.0f, 0.0f, 1.0f, 1.0f);
                                    if (currentType == 4)
                                        color = new Color4(0.0f, 1.0f, 1.0f, 1.0f);
                                    if (currentType == 5)
                                        color = new Color4(1.0f, 0.0f, 1.0f, 1.0f);
                                    if (currentType == 6)
                                        color = new Color4(1.0f, 1.0f, 0.0f, 1.0f);
                                }

                                //front
                                if ((z - 1 < 0 && (position.Z / size) - 1 < 0) || (z - 1 > 0 && blockIDs[x, y, z - 1] == 0))
                                {
                                    GL.Color3(color.R * 0.7, color.G * 0.7, color.B * 0.7);
                                    GL.Vertex3(pos + new Vector3(1, 1, 0));
                                    GL.Vertex3(pos + new Vector3(1, 0, 0));
                                    GL.Vertex3(pos + new Vector3(0, 0, 0));
                                    GL.Vertex3(pos + new Vector3(0, 1, 0));
                                }
                                //back
                                if ((z + 1 >= size && (position.Z / size) + 1 >= Terrain.size) || (z + 1 < size && blockIDs[x, y, z + 1] == 0))
                                {
                                    GL.Color3(color.R * 0.7, color.G * 0.7, color.B * 0.7);
                                    GL.Vertex3(pos + new Vector3(0, 1, 1));
                                    GL.Vertex3(pos + new Vector3(0, 0, 1));
                                    GL.Vertex3(pos + new Vector3(1, 0, 1));
                                    GL.Vertex3(pos + new Vector3(1, 1, 1));
                                }

                                //left
                                if ((x + 1 >= size && (position.X / size) + 1 >= Terrain.size) || (x + 1 < size && blockIDs[x + 1, y, z] == 0))
                                {
                                    GL.Color3(color.R * 0.8, color.G * 0.8, color.B * 0.8);
                                    GL.Vertex3(pos + new Vector3(1, 1, 0));
                                    GL.Vertex3(pos + new Vector3(1, 1, 1));
                                    GL.Vertex3(pos + new Vector3(1, 0, 1));
                                    GL.Vertex3(pos + new Vector3(1, 0, 0));
                                }
                                //right
                                if ((x - 1 < 0 && (position.X / size) - 1 < 0) || (x - 1 > 0 && blockIDs[x - 1, y, z] == 0))
                                {
                                    GL.Color3(color.R * 0.6, color.G * 0.6, color.B * 0.6);
                                    GL.Vertex3(pos + new Vector3(0, 1, 1));
                                    GL.Vertex3(pos + new Vector3(0, 1, 0));
                                    GL.Vertex3(pos + new Vector3(0, 0, 0));
                                    GL.Vertex3(pos + new Vector3(0, 0, 1));
                                }

                                //top
                                if ((y + 1 >= size && (position.Y / size) + 1 >= Terrain.height) || (y + 1 < size && blockIDs[x, y + 1, z] == 0))
                                {
                                    GL.Color3(color.R, color.G, color.B);
                                    GL.Vertex3(pos + new Vector3(1, 1, 1));
                                    GL.Vertex3(pos + new Vector3(1, 1, 0));
                                    GL.Vertex3(pos + new Vector3(0, 1, 0));
                                    GL.Vertex3(pos + new Vector3(0, 1, 1));
                                }
                                //down
                                if ((y - 1 < 0 && (position.Y / size) - 1 < 0) || (y - 1 > 0 && blockIDs[x, y - 1, z] == 0))
                                {
                                    GL.Color3(color.R * 0.4, color.G * 0.4, color.B * 0.4);
                                    GL.Vertex3(pos + new Vector3(0, 0, 1));
                                    GL.Vertex3(pos + new Vector3(0, 0, 0));
                                    GL.Vertex3(pos + new Vector3(1, 0, 0));
                                    GL.Vertex3(pos + new Vector3(1, 0, 1));
                                }

                                lastType = currentType;
                                /*//bottom
                                if (y - 1 < 0 || (y - 1 > 0 && blockIDs[x, y - 1, z] == 0))
                                {
                                    GL.Color3(color.R * 0.4, color.G * 0.4, color.B * 0.4);
                                    GL.Vertex3(pos + new Vector3(1, 0, 1));
                                    GL.Vertex3(pos + new Vector3(0, 0, 1));
                                    GL.Vertex3(pos + new Vector3(0, 0, 0));
                                    GL.Vertex3(pos + new Vector3(1, 0, 0));
                                }*/


                            }
                        }
                    }
                    
                }

                GL.End();
            }

        }
    }
}