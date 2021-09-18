using System.Numerics;

namespace OneWeek2
{
    public class Ray
    {
        private readonly Vector3 _origin = default;
        private readonly Vector3 _direction = default;
        private readonly float _time;

        public Vector3 Origin => _origin;
        public Vector3 Direction => _direction;
        public float Time => _time;

        public Ray(Vector3 origin, Vector3 direction, float time = 0f)
        {
            _origin = origin;
            _direction = direction;
            _time = time;
        }

        public Vector3 At(float t)
        {
            return _origin + t * _direction;
        }
    }
}