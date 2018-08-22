using System;

namespace BlackCoat
{
    public static class MathHelper
    {
        private const int _LOOKUP_SIZE = 1024; //has to be power of 2
        private static readonly float[] _SIN, _COS;


        static MathHelper()
        {
            _SIN = new float[_LOOKUP_SIZE];
            _COS = new float[_LOOKUP_SIZE];

            for (int i = 0; i < _LOOKUP_SIZE; i++)
            {
                _SIN[i] = (float)Math.Sin(i * Math.PI / _LOOKUP_SIZE * 2);
                _COS[i] = (float)Math.Cos(i * Math.PI / _LOOKUP_SIZE * 2);
            }
        }


        public static float Cos(float rotation)
        {
            return _COS[MapToLookupIndex(rotation)];
        }

        public static float Sin(float rotation)
        {
            return _SIN[MapToLookupIndex(rotation)];
        }

        private static int MapToLookupIndex(float rotation)
        {
            return (int)(rotation * (_LOOKUP_SIZE / 360f) + 0.5f) & (_LOOKUP_SIZE - 1);
        }

        public static float ValidateAngle(float angle)
        {
            return (angle %= 360) < 0 ? angle + 360 : angle;
        }
    }
}