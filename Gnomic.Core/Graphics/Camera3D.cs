using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Gnomic.Util;

namespace Gnomic.Graphics
{
    public interface ICamera3D
    {
        Matrix MatrixViewProj { get; }
        Matrix MatrixView { get; }
        Matrix MatrixViewInverse { get; }
        Matrix MatrixProj { get; }
        BoundingFrustum BoundingFrustum { get; }
    }

    public enum CameraMode
    {
        Static,
        Free,
        Max,
        Follow2D,
    }
    public class CameraService
    {
        private ICamera3D current;
        public ICamera3D Current { get { return current; } }

        public CameraService(ICamera3D defaultCam)
        {
            current = defaultCam;
        }
    }

    public class Camera3D : ICamera3D
    {
        IServiceProvider services;
        IGraphicsDeviceService graphicsSvc;

        CameraMode mode = CameraMode.Follow2D;

        public CameraMode Mode
        {
            get { return mode; }
            set
            {
                mode = value;
                Input.CaptureMouse = (mode == CameraMode.Free);
            }
        }

        public bool Enabled = true;

        internal Matrix matrixView;
        public Matrix MatrixView { get { return matrixView; } }
        internal Matrix matrixViewInv;
        public Matrix MatrixViewInverse { get { return matrixViewInv; } }
        internal Matrix matrixProj;
        public Matrix MatrixProj { get { return matrixProj; } set { matrixProj = value; } }
        internal Matrix matrixViewProj;
        public Matrix MatrixViewProj { get { return matrixViewProj; } }

        public Vector2 Min2D;
        public Vector2 Max2D;

        Vector3 position;
        public Vector3 Position 
        { 
            get { return position; } 
            set { position = value; } 
        }

        Vector3 targetPos;
        // Position camera is moving to in Follow2D mode
        public Vector3 TargetPos
        {
            get { return targetPos; }
            set { targetPos = value; }
        }

        // meters/sec cam will travel if target
        // is 1m away.
        float followSpeed = 2f;
        public float FollowSpeed
        {
            get { return followSpeed; }
            set { followSpeed = value; }
        }

        bool rotationDirty = true;

        Vector3 rotation;
        public Vector3 Rotation
        {
            get { return rotation; }
            set 
            {
                rotationDirty = rotation != value;
                rotation = value;
            }
        }


        public BoundingFrustum BoundingFrustum { get { return new BoundingFrustum(matrixViewProj); } }

        public Vector3 Forward { get { return matrixViewInv.Forward; } }
        public Vector3 Left { get { return matrixViewInv.Left; } }
        public Vector3 Up { get { return matrixViewInv.Up; } }

        float mouseSensitivity = 3.0f;
        float mousePanSensitivity = 30.0f;
        float mouseScrollSensitivity = 5.0f;

        public bool collided = false;
        public Vector3 collideNormal = Vector3.Zero;
        public Vector3 collidePos = Vector3.Zero;
        public Vector3 lookDirection = -Vector3.UnitZ;
        public Vector3 DEFAULT_CAMERA_POSITION = new Vector3(0.0f, 0.0f, 20.0f);
        public Vector3 accel = Vector3.Zero;
        public Vector3 vel = Vector3.Zero;
        public float damping = 8.0f;

        //private float pitch = 0.0f;
        //private float yaw = 0.0f;
        
        private Viewport viewport;
        public Viewport Viewport
        {
            get { return viewport; }
            set
            {
                viewport = value;
                aspect = viewport.AspectRatio;
                matrixProj = Matrix.CreatePerspectiveFieldOfView(fov, aspect, nearClip, farClip);
                graphicsSvc.GraphicsDevice.Viewport = value;
            }
        }

        private float farClip = 1000.0f;
        public float FarClip
        {
            get { return farClip; }
            set
            {
                farClip = value;
                matrixProj = Matrix.CreatePerspectiveFieldOfView(fov, aspect, nearClip, farClip);
            }
        }
        private float nearClip = 5.0f;
        public float NearClip
        {
            get { return nearClip; }
            set
            {
                nearClip = value;
                matrixProj = Matrix.CreatePerspectiveFieldOfView(fov, aspect, nearClip, farClip);
            }
        }
        private float fov = (float)MathHelper.PiOver4;
        public float FieldOfView
        {
            get { return fov; }
            set
            {
                fov = value;
                matrixProj = Matrix.CreatePerspectiveFieldOfView(fov, aspect, nearClip, farClip);
            }
        }
        private float aspect = 1.0f;
        public float AspectRatio
        {
            get { return aspect; }
            set
            {
                aspect = value;
                matrixProj = Matrix.CreatePerspectiveFieldOfView(fov, aspect, nearClip, farClip);
            }
        }

        public Camera3D(IServiceProvider services)
        {
            this.services = services;
            graphicsSvc = (IGraphicsDeviceService)services.GetService(typeof(IGraphicsDeviceService));
            graphicsSvc.GraphicsDevice.DeviceReset += new EventHandler<EventArgs>(graphicsSvc_DeviceReset);
            this.position = DEFAULT_CAMERA_POSITION;
            this.targetPos = DEFAULT_CAMERA_POSITION;
            this.vel = new Vector3(0.0f, 0.0f, 0.0f);

            Viewport = graphicsSvc.GraphicsDevice.Viewport;

            matrixProj = Matrix.CreatePerspectiveFieldOfView(fov, aspect, nearClip, farClip);
            matrixView = Matrix.CreateLookAt(position, position + lookDirection, Vector3.UnitY);
            onViewOrProjMatrixUpdated();
        }

        public bool AutoUpdateViewport = true;
        void graphicsSvc_DeviceReset(object sender, EventArgs e)
        {
            if (AutoUpdateViewport)
            {
                viewport = graphicsSvc.GraphicsDevice.Viewport;
                aspect = viewport.AspectRatio;
                matrixProj = Matrix.CreatePerspectiveFieldOfView(fov, aspect, nearClip, farClip);
            }
        }

        public Vector3 Project(Vector3 source, Matrix worldMatrix)
        {
            return viewport.Project(source, matrixProj, matrixView, worldMatrix);
        }

        public bool FrustrumIntersects(ref BoundingBox bb)
        {
            bool result;
            BoundingFrustum.Intersects(ref bb, out result);
            return result;
        }
        public bool FrustrumIntersects(ref BoundingSphere bs)
        {
            bool result;
            BoundingFrustum.Intersects(ref bs, out result);
            return result;
        }

        public void Update(float dt)
        {
            if (!Enabled)
                return;

            switch (mode)
            {
                case CameraMode.Follow2D:
                    UpdateFollow2DView(dt);
                    break;
                case CameraMode.Free:
                    UpdateFreeLook(dt);
                    break;
                case CameraMode.Max:
                    UpdateMax(dt);
                    break;
            }
        }

        private void UpdateFreeLook(float dt)
        {
            Vector3 a1 = Vector3.Zero;
            Vector3 a2 = Vector3.Zero;

            float accelFactor = 500.0f;

            if (Input.KeyDown(Keys.A))
            {
                a1 += matrixViewInv.Left; // matrixView.Left;
            }
            else if (Input.KeyDown(Keys.D))
            {
                a1 -= matrixViewInv.Left; //matrixView.Left;
            }

            if (Input.KeyDown(Keys.W))
            {
                a2 += matrixViewInv.Forward; //matrixView.Forward;
            }
            else if (Input.KeyDown(Keys.S))
            {
                a2 -= matrixViewInv.Forward; // matrixView.Forward;
            }

            if (Input.KeyDown(Keys.LeftShift))
                accelFactor *= 3.0f;

            accel = a1 + a2;
            if (accel != Vector3.Zero)
            {
                accel.Normalize();
                accel = accel * accelFactor;

                vel += (accel * dt);
            }

            vel *= (1.0f - damping * dt);

            if (vel.Length() > 640.0f)
            {
                vel.Normalize();
                vel *= 640.0f;
            }

            position += vel * dt;

            rotation.Y -= mouseSensitivity * ((float)Input.MouseDX / (float)graphicsSvc.GraphicsDevice.PresentationParameters.BackBufferWidth);
            rotation.X -= mouseSensitivity * ((float)Input.MouseDY / (float)graphicsSvc.GraphicsDevice.PresentationParameters.BackBufferHeight);

            rotation.X = Math.Min(rotation.X, (float)Math.PI / 2.0f);
            rotation.X = Math.Max(rotation.X, (float)-Math.PI / 2.0f);

            UpdateView();
        }

        private void UpdateView()
        {
            // Calculate camera rotation
            float sinMouseRotX = (float)Math.Sin(rotation.X);
            float verticalFactor = (1.0001f - Math.Abs(sinMouseRotX));

            Vector3 atVec = new Vector3(position.X + verticalFactor * (float)Math.Sin(rotation.Y),
                                        position.Y + sinMouseRotX,
                                        position.Z + verticalFactor * (float)Math.Cos(rotation.Y));
            matrixView = Matrix.CreateLookAt(position, atVec, Vector3.UnitY);
            onViewOrProjMatrixUpdated();
        }

        private void onViewOrProjMatrixUpdated()
        {
            Matrix.Multiply(ref matrixView, ref matrixProj, out matrixViewProj);
            BoundingFrustum.Matrix = matrixViewProj;
            Matrix.Invert(ref matrixView, out matrixViewInv);
        }

        private void UpdateMax(float dt)
        {
            if (Input.MouseDW != 0)
            {
                position += matrixViewInv.Forward * (float)Input.MouseDW / 120.0f * mouseScrollSensitivity;
            }

            if (Input.MouseDown(MouseButton.Middle))
            {
                if (Input.KeyDown(Keys.LeftAlt) &&
                    Input.KeyDown(Keys.LeftControl))
                {
                    position -= matrixViewInv.Forward * mousePanSensitivity * ((float)Input.MouseDY / (float)graphicsSvc.GraphicsDevice.PresentationParameters.BackBufferWidth);
                }
                else if (Input.KeyDown(Keys.LeftAlt))
                {
                    rotation.Y -= mouseSensitivity * ((float)Input.MouseDX / (float)graphicsSvc.GraphicsDevice.PresentationParameters.BackBufferWidth);
                    rotation.X -= mouseSensitivity * ((float)Input.MouseDY / (float)graphicsSvc.GraphicsDevice.PresentationParameters.BackBufferHeight);
                    rotation.X = Math.Min(rotation.X, (float)Math.PI / 2.0f);
                    rotation.X = Math.Max(rotation.X, (float)-Math.PI / 2.0f);
                }
                else
                {
                    position += matrixViewInv.Left * mousePanSensitivity * ((float)Input.MouseDX / (float)graphicsSvc.GraphicsDevice.PresentationParameters.BackBufferWidth);
                    position += matrixViewInv.Up * mousePanSensitivity * ((float)Input.MouseDY / (float)graphicsSvc.GraphicsDevice.PresentationParameters.BackBufferWidth);
                }
            }

            UpdateView();
        }

        public void DoShake(float intensity, float time)
        {
            shakeTimeTotal = time;
            shakeTimer = time;
            shakeIntensity = intensity;
            shakeTimeOffset = (float)Rand.NextDouble(); 
        }

        float shakeTimer = 0.0f;
        float shakeTimeTotal = 0.0f;
        float shakeIntensity = 0f;
        float shakeInvPeriod = 10.0f;
        float shakeTimeOffset = 0.0f;
        float epsilon = 1e-6f;
        public Random Rand = new Random();
        Vector3 shakeOffset;

        private void UpdateFollow2DView(float dt)
        {
            Vector3 diff = targetPos - position;

            if (diff.LengthSquared() < epsilon)
                position = targetPos;
            else
                position = position + diff * followSpeed * dt;

            if (shakeTimer > 0.0f)
            {
                shakeTimer -= dt;
                float shakeProgress = shakeTimer / shakeTimeTotal;
                float shakeScale = 1.0f - 2.0f * Math.Abs(shakeProgress - 0.5f);

                shakeOffset = new Vector3(
                    shakeScale * shakeIntensity * (float)Math.Cos(1.8f * shakeInvPeriod * shakeProgress + shakeTimeOffset),
                    shakeScale * shakeIntensity * (float)Math.Sin(0.6f * shakeInvPeriod * shakeProgress + shakeTimeOffset),
                    shakeScale * shakeIntensity * (float)Math.Cos(0.4f * shakeInvPeriod * shakeProgress + shakeTimeOffset));
            }
            Vector3 pos = position + shakeOffset;
            matrixView = Matrix.CreateLookAt(pos, pos + lookDirection, Vector3.UnitY);
            onViewOrProjMatrixUpdated();
        }

        public Vector2 Unproject(Vector2 screenPos)
        {
            return Unproject(screenPos, 0.0f);
        }

        public Vector2 Unproject(Vector2 screenPos, float z)
        {
            Ray ray = UnprojectPoint(Matrix.Identity, screenPos);
            float? dist = ray.Intersects(new Plane(new Vector3(0, 0, -1), z));
            Vector3 pos = ray.Position;

            if (dist.HasValue)
                pos += ray.Direction * dist.Value;

            return new Vector2(pos.X, pos.Y);
        }

        public Ray UnprojectPoint(Matrix matrixWorld, Vector2 screenPos)
        {
            Vector3 screenNear = new Vector3(screenPos.X, screenPos.Y, 0.0f);
            Vector3 screenFar = new Vector3(screenPos.X, screenPos.Y, 1.0f); // * 10f);
            //multiplying by 10 makes sure the line's direction is accurate - by casting out   
            //a very far distance. The direction is all we really care about, so the further  
            //away the point, the more accurate it will be....  
            Vector3 worldNear = ViewportEx.UnprojectEx(ref viewport, ref screenNear, ref matrixProj, ref matrixView, ref matrixWorld);
            Vector3 worldFar = ViewportEx.UnprojectEx(ref viewport, ref screenFar, ref matrixProj, ref matrixView, ref matrixWorld);
            Vector3 direction = worldFar - worldNear;
            float len = 1f / direction.Length();
            direction = direction * len; //manual normalisation  
            //direction.Z = -direction.Z;

            Ray r = new Ray(worldNear, direction);
            return (r);
        }
    }
}