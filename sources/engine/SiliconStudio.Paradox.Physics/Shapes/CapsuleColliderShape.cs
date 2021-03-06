﻿// Copyright (c) 2014 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.
using System;

using SiliconStudio.Core.Mathematics;
using SiliconStudio.Paradox.Graphics;

namespace SiliconStudio.Paradox.Physics
{
    public class CapsuleColliderShape : ColliderShape
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CapsuleColliderShape"/> class.
        /// </summary>
        /// <param name="is2D">if set to <c>true</c> [is2 d].</param>
        /// <param name="radius">The radius.</param>
        /// <param name="height">The height.</param>
        /// <param name="upAxis">Up axis.</param>
        public CapsuleColliderShape(bool is2D, float radius, float height, Vector3 upAxis)
        {
            Type = ColliderShapeTypes.Capsule;
            Is2D = is2D;

            Radius = radius;
            Height = height;
            UpAxis = upAxis;

            BulletSharp.CapsuleShape shape;

            Matrix rotation;

            //http://en.wikipedia.org/wiki/Capsule_(geometry)
            var h = radius * 2 + height;

            if (upAxis == Vector3.UnitX)
            {
                shape = new BulletSharp.CapsuleShapeX(radius, height);

                rotation = Matrix.RotationZ((float)Math.PI / 2.0f);
            }
            else if (upAxis == Vector3.UnitZ)
            {
                shape = new BulletSharp.CapsuleShapeZ(radius, height);

                rotation = Matrix.RotationX((float)Math.PI / 2.0f);
            }
            else //default to Y
            {
                UpAxis = Vector3.UnitY;
                shape = new BulletSharp.CapsuleShape(radius, height);

                rotation = Matrix.Identity;
            }

            if (Is2D)
            {
                InternalShape = new BulletSharp.Convex2DShape(shape) { LocalScaling = new Vector3(1, 1, 0) };
            }
            else
            {
                InternalShape = shape;
            }

            if (!PhysicsEngine.Singleton.CreateDebugPrimitives) return;
            DebugPrimitive = GeometricPrimitive.Capsule.New(PhysicsEngine.Singleton.DebugGraphicsDevice);
            DebugPrimitiveScaling = Matrix.Scaling(new Vector3(radius * 2, h / 2, radius * 2) * 1.01f) * rotation;
        }

        /// <summary>
        /// Gets the radius.
        /// </summary>
        /// <value>
        /// The radius.
        /// </value>
        public float Radius { get; private set; }

        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public float Height { get; private set; }

        /// <summary>
        /// Gets up axis.
        /// </summary>
        /// <value>
        /// Up axis.
        /// </value>
        public Vector3 UpAxis { get; private set; }
    }
}
