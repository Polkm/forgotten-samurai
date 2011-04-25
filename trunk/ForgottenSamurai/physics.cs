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
    class Physics
    {
        public DiscreteDynamicsWorld World { get; set; }
        CollisionDispatcher dispatcher;
        AlignedCollisionShapeArray collisionShapes = new AlignedCollisionShapeArray();
        CollisionConfiguration collisionConf;

        public Physics()
        {
            // collision configuration contains default setup for memory, collision setup
            collisionConf = new DefaultCollisionConfiguration();
            dispatcher = new CollisionDispatcher(collisionConf);
            World = new DiscreteDynamicsWorld(dispatcher, new DbvtBroadphase(), null, collisionConf);
            World.Gravity = new Vector3(0, -40, 0);
        }

        public virtual void Update(float elapsedTime)
        {
            World.StepSimulation(elapsedTime);
        }

        public RigidBody AddStaticBlock(Vector3 pos)
        {
            CollisionShape shape = new BoxShape(0.5f, 0.5f, 0.5f);
            collisionShapes.Add(shape);

            Vector3 localInertia = Vector3.Zero;
            DefaultMotionState myMotionState = new DefaultMotionState(Matrix4.CreateTranslation(pos.X + 0.5f, pos.Y + 0.5f, pos.Z + 0.5f));
            RigidBodyConstructionInfo rbInfo = new RigidBodyConstructionInfo(0, myMotionState, shape, localInertia);
            RigidBody body = new RigidBody(rbInfo);

            World.AddRigidBody(body);

            return body;
        }

        public RigidBody AddDynamicBlock(Vector3 pos)
        {

            CollisionShape shape = new BoxShape(0.5f, 0.5f, 0.5f);
            collisionShapes.Add(shape);


            Vector3 localInertia = Vector3.Zero;
            shape.CalculateLocalInertia(5, out localInertia);
            DefaultMotionState myMotionState = new DefaultMotionState(Matrix4.CreateTranslation(pos.X + 0.5f, pos.Y + 0.5f, pos.Z + 0.5f));
            RigidBodyConstructionInfo rbInfo = new RigidBodyConstructionInfo(5, myMotionState, shape, localInertia);
            RigidBody body = new RigidBody(rbInfo);
            body.AngularFactor = new Vector3(0, 0, 0);
            body.LinearFactor = new Vector3(0, 1, 0);
            //body.Translate(new Vector3(pos.X, pos.Y, pos.Z));

            World.AddRigidBody(body);

            return body;
        }
    }
}
