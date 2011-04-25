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
    class Terrain
    {
        public static Random random;
        //public static BlockSystem[, ,] chunks;
        public static List<List<List<BlockSystem>>> chunks;
        public static int size = 10;
        public static int height = 2;
        float frustrumRadius;

        public Terrain()
        {
            random = new Random();
            chunks = new List<List<List<BlockSystem>>>();
            //chunks = new BlockSystem[size, 2, size];
            frustrumRadius = 0.0f;

            float genCounter = 0;
            
            for (int x = 0; x < size; x++)
            {
                chunks.Add(new List<List<BlockSystem>>());
                for (int y = 0; y < height; y++)
                {
                    chunks[x].Add(new List<BlockSystem>());
                    for (int z = 0; z < size; z++)
                    {
                        chunks[x][y].Add(new BlockSystem());
                        chunks[x][y][z].position = new Vector3(x * BlockSystem.size, y * BlockSystem.size, z * BlockSystem.size);
                        chunks[x][y][z].chunkPos = new Vector3(x, y, z);
                        Console.Clear();
                        Console.Write(Math.Round(genCounter / (size * size * height) * 100.0f));
                        Console.Write("% generated");
                        genCounter++;
                    }
                }
            }

            float buildCounter = 0;
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int z = 0; z < size; z++)
                    {
                        chunks[x][y][z].GenerateVertexData();
                        Console.Clear();
                        Console.Write(Math.Round(buildCounter / (size * size * height) * 100.0f));
                        Console.Write("% vertex built");
                        buildCounter++;
                    }
                }
            }
            Console.Clear();
        }

        public static bool IsValidChunk(Vector3 chunkPos)
        {
            return chunkPos.X >= 0 && chunkPos.X < Terrain.size && chunkPos.Y >= 0 && chunkPos.Y < Terrain.height && chunkPos.Z >= 0 && chunkPos.Z < Terrain.size;
        }

        int rendering = 0;
        public void Draw()
        {
            rendering = 0;

            if (frustrumRadius == 0)
                frustrumRadius = GetFrustrumRadius();

            float fViewLen = Camera.cameraFarClip - 1.0f;

            // get the look vector of the camera from the view matrix
            Vector3 vLookVector = Camera.cameraLookPos - Camera.cameraPos;

            // calculate the center of the sphere
            Vector3 FrustrumCenter = Camera.cameraPos + (vLookVector * ((fViewLen * 0.5f) + 1.0f));

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int z = 0; z < size; z++)
                    {
                        Vector3 chunkCenter = chunks[x][y][z].position + (new Vector3(16.0f, 16.0f, 16.0f) * 0.5f);
                        if (SphereIntersectsSphere(FrustrumCenter, frustrumRadius, chunkCenter, BlockSystem.radius))
                        {
                            if (ShpereIntersectsCone(chunkCenter, BlockSystem.radius, Camera.cameraPos, vLookVector, (float)Math.Cos(Math.PI / 3), (float)Math.Sin(Math.PI / 3)))
                            {
                                chunks[x][y][z].Draw();
                                rendering++;
                            }
                        }
                    }
                }
            }
            //Console.WriteLine(rendering);
        }

        public bool SphereIntersectsSphere(Vector3 pos1, float radius1, Vector3 pos2, float radius2)
        {
            return ((pos1 - pos2).Length < radius1 + radius2);
        }

        public bool ShpereIntersectsCone(Vector3 pos1, float radius, Vector3 pos2, Vector3 axis, float angleCos, float angleSin)
        {
            // Test whether cone vertex is in sphere.
            Vector3 diff = pos1 - pos2;
            float rSqr = radius * radius;
            float lenSqr = diff.LengthSquared;
            if (lenSqr <= rSqr)
                return true;

            // Test whether sphere center is in cone.
            float dot = Vector3.Dot(diff, Vector3.Normalize(axis));
            float dotSqr = dot * dot;
            float cosSqr = angleCos * angleCos;
            if (dotSqr >= lenSqr * cosSqr && dot > (float)0)
                return true;

            return false;


            /*float uLen = (float)Math.Sqrt(Math.Abs(lenSqr - dotSqr));
            float test = angleCos * dot + angleCos * uLen;
            float discr = (test * test) - lenSqr + rSqr;

            // compute point of intersection closest to vertex V
            float t = test - (float)Math.Sqrt(discr);
            Vector3 B = diff - dot * axis;
            float tmp = angleSin / uLen;
            mPoint = t * (angleCos * axis + tmp * B);

            return discr >= (float)0 && test >= (float)0;*/
        }


        public float GetFrustrumRadius()
        {
            float fViewLen = Camera.cameraFarClip - 1.0f;

            float fHeight = (float)(fViewLen * Math.Tan(Math.PI / 4.0 * 0.5));

            // with an aspect ratio of 1, the width will be the same
            float fWidth = fHeight;

            // halfway point between near/far planes starting at the origin and extending along the z axis
            Vector3 P = new Vector3(0.0f, 0.0f, 1.0f + fViewLen * 0.5f);

            // the calculate far corner of the frustum
            Vector3 Q = new Vector3(fWidth, fHeight, fViewLen);

            // the vector between P and Q
            Vector3 vDiff = new Vector3(P - Q);
            
            return vDiff.Length * 1.0f;
        }

    }
}
