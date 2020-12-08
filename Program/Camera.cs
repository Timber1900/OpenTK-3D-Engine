using System;
using OpenTK;
using OpenTK.Mathematics;

namespace Program
{
    /// <summary>
    /// Camera class
    /// </summary>
    public class Camera
    {
        // Those vectors are directions pointing outwards from the camera to define how it rotated
        private Vector3 _front = -Vector3.UnitZ;

        // Rotation around the X axis (radians)
        private float _pitch;

        // Rotation around the Y axis (radians)
        private float _yaw = -MathHelper.PiOver2; // Without this you would be started rotated 90 degrees right

        // The field of view of the camera (radians)
        private float _fov = MathHelper.PiOver2;

        /// <summary>
        /// Camera constructor
        /// </summary>
        /// <param name="position">Position of the camera</param>
        /// <param name="aspectRatio">Aspect ratio of the camera</param>
        public Camera(Vector3 position, float aspectRatio)
        {
            Position = position;
            AspectRatio = aspectRatio;
        }
        
        /// <summary>
        /// The position of the camera
        /// </summary>
        public Vector3 Position { get; set; }
        
        /// <summary>
        /// This is simply the aspect ratio of the viewport, used for the projection matrix
        /// </summary>
        private float AspectRatio { get; }

        /// <summary>
        /// 
        /// </summary>
        public Vector3 Front => _front;

        /// <summary>
        /// 
        /// </summary>
        public Vector3 Up { get; private set; } = Vector3.UnitY;

        /// <summary>
        /// 
        /// </summary>
        public Vector3 Right { get; private set; } = Vector3.UnitX;
        
        /// <summary>
        /// We convert from degrees to radians as soon as the property is set to improve performance
        /// </summary>
        public float Pitch
        {
            get => MathHelper.RadiansToDegrees(_pitch);
            set
            {
                // We clamp the pitch value between -89 and 89 to prevent the camera from going upside down, and a bunch
                // of weird "bugs" when you are using euler angles for rotation.
                // If you want to read more about this you can try researching a topic called gimbal lock
                var angle = MathHelper.Clamp(value, -89f, 89f);
                _pitch = MathHelper.DegreesToRadians(angle);
                UpdateVectors();
            }
        }
        
        /// <summary>
        /// We convert from degrees to radians as soon as the property is set to improve performance
        /// </summary>
        public float Yaw
        {
            get => MathHelper.RadiansToDegrees(_yaw);
            set
            {
                _yaw = MathHelper.DegreesToRadians(value);
                UpdateVectors();
            }
        }

        /// <summary>
        /// The field of view (FOV) is the vertical angle of the camera view, this has been discussed more in depth in a
        /// previous tutorial, but in this tutorial you have also learned how we can use this to simulate a zoom feature.
        /// We convert from degrees to radians as soon as the property is set to improve performance
        /// </summary>
        public float Fov
        {
            get => MathHelper.RadiansToDegrees(_fov);
            set
            {
                var angle = MathHelper.Clamp(value, 1f, 45f);
                _fov = MathHelper.DegreesToRadians(angle);
            }
        }

        /// <summary>
        /// Get the view matrix using the amazing LookAt function described more in depth on the web tutorials
        /// </summary>
        /// <returns></returns>
        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(Position, Position + _front, Up);
        }
        
        /// <summary>
        /// Get the projection matrix using the same method we have used up until this point
        /// </summary>
        /// <returns></returns>
        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, 0.01f, 1000f);
        }

        // This function is going to update the direction vertices using some of the math learned in the web tutorials
        private void UpdateVectors()
        {
            // First the front matrix is calculated using some basic trigonometry
            _front.X = (float)Math.Cos(_pitch) * (float)Math.Cos(_yaw);
            _front.Y = (float)Math.Sin(_pitch);
            _front.Z = (float)Math.Cos(_pitch) * (float)Math.Sin(_yaw);

            // We need to make sure the vectors are all normalized, as otherwise we would get some funky results
            _front = Vector3.Normalize(_front);

            // Calculate both the right and the up vector using cross product
            // Note that we are calculating the right from the global up, this behaviour might
            // not be what you need for all cameras so keep this in mind if you do not want a FPS camera
            Right = Vector3.Normalize(Vector3.Cross(_front, Vector3.UnitY));
            Up = Vector3.Normalize(Vector3.Cross(Right, _front));
        }
    }
} 