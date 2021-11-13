using System.Numerics;

namespace OneWeek2
{
    public class Ray
    {
        private readonly Vector3 _origin;
        private readonly Vector3 _direction;

        public Vector3 Origin => _origin;
        public Vector3 Direction => _direction;
        public float Time { get; }

        public Ray(Vector3 origin, Vector3 direction, float time = 0f)
        {
            _origin = origin;
            _direction = direction;
            Time = time;
        }

        public Vector3 At(float t)
        {
            return _origin + t * _direction;
        }
    }
}