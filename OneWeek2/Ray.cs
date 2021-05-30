using System.Numerics;

namespace OneWeek2
{
    public class Ray
    {
        private readonly Vector3 _origin = default;
        private readonly Vector3 _direction = default;

        public Vector3 Origin => _origin;
        public Vector3 Direction => _direction;

        public Ray(Vector3 origin, Vector3 direction)
        {
            _origin = origin;
            _direction = direction;
        }

        public Vector3 At(float t)
        {
            return _origin * t * _direction;
        }
    }
}