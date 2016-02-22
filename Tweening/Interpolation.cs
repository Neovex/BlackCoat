using System;

namespace BlackCoat.Tweening
{
    /// <summary>
    /// Common interpolations for Tweening
    /// </summary>
    public static class Interpolation
    {
        public static float Linear(float start, float distance, float elapsedTime, float duration)
		{
			if (elapsedTime > duration) elapsedTime = duration;
	  		return distance * (elapsedTime / duration) + start;
		}

        public static float InQuad(float start, float distance, float elapsedTime, float duration)
		{
			elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
	  		return distance * elapsedTime * elapsedTime + start;
		}

        public static float OutQuad(float start, float distance, float elapsedTime, float duration)
		{
			elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
			return -distance * elapsedTime * (elapsedTime - 2.0f) + start;
		}

        public static float InOutQuad(float start, float distance, float elapsedTime, float duration)
		{
			elapsedTime = (elapsedTime > duration) ? 2.0f : elapsedTime / (duration / 2);
			if (elapsedTime < 1) return distance / 2.0f * elapsedTime * elapsedTime + start;
			elapsedTime--;
			return -distance / 2.0f * (elapsedTime * (elapsedTime - 2.0f) - 1.0f) + start;
		}

        public static float InCubic(float start, float distance, float elapsedTime, float duration)
		{
			elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
			return distance * elapsedTime * elapsedTime * elapsedTime + start;
		}

        public static float OutCubic(float start, float distance, float elapsedTime, float duration)
		{
			elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
			elapsedTime--;
			return distance * (elapsedTime * elapsedTime * elapsedTime + 1) + start;
		}

        public static float InOutCubic(float start, float distance, float elapsedTime, float duration)
		{
			elapsedTime = (elapsedTime > duration) ? 2.0f : elapsedTime / (duration / 2);
			if (elapsedTime < 1)
				return distance / 2 * elapsedTime * elapsedTime * elapsedTime + start;
			elapsedTime -= 2;
			return distance / 2 * (elapsedTime * elapsedTime * elapsedTime + 2) + start;
		}

        public static float InQuart(float start, float distance, float elapsedTime, float duration)
		{
			elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
			return distance * elapsedTime * elapsedTime * elapsedTime * elapsedTime + start;
		}

        public static float OutQuart(float start, float distance, float elapsedTime, float duration)
		{
			elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
			elapsedTime--;
			return -distance * (elapsedTime * elapsedTime * elapsedTime * elapsedTime - 1) + start;
		}

        public static float InOutQuart(float start, float distance, float elapsedTime, float duration)
		{
			elapsedTime = (elapsedTime > duration) ? 2.0f : elapsedTime / (duration / 2);
			if (elapsedTime < 1)
				return distance / 2 * elapsedTime * elapsedTime * elapsedTime * elapsedTime + start;
			elapsedTime -= 2;
			return -distance / 2 * (elapsedTime * elapsedTime * elapsedTime * elapsedTime - 2) + start;
		}

        public static float InQuint(float start, float distance, float elapsedTime, float duration)
		{
			elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
			return distance * elapsedTime * elapsedTime * elapsedTime * elapsedTime * elapsedTime + start;
		}

        public static float OutQuint(float start, float distance, float elapsedTime, float duration)
		{
			elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
			elapsedTime--;
			return distance * (elapsedTime * elapsedTime * elapsedTime * elapsedTime * elapsedTime + 1) + start;
		}

        public static float InOutQuint(float start, float distance, float elapsedTime, float duration)
		{
			elapsedTime = (elapsedTime > duration) ? 2.0f : elapsedTime / (duration / 2);
			if (elapsedTime < 1)
				return distance / 2 * elapsedTime * elapsedTime * elapsedTime * elapsedTime * elapsedTime + start;
			elapsedTime -= 2;
			return distance / 2 * (elapsedTime * elapsedTime * elapsedTime * elapsedTime * elapsedTime + 2) + start;
		}

        public static float InSine(float start, float distance, float elapsedTime, float duration)
		{
			if (elapsedTime > duration)
				elapsedTime = duration;
			return (float)(-distance * Math.Cos(elapsedTime / duration * (Math.PI / 2)) + distance + start);
		}

        public static float OutSine(float start, float distance, float elapsedTime, float duration)
		{
			if (elapsedTime > duration)
				elapsedTime = duration;
			return (float)(distance * Math.Sin(elapsedTime / duration * (Math.PI / 2)) + start);
		}

        public static float InOutSine(float start, float distance, float elapsedTime, float duration)
		{
			if (elapsedTime > duration)
				elapsedTime = duration;
			return (float)(-distance / 2 * (Math.Cos(Math.PI * elapsedTime / duration) - 1) + start);
		}

        public static float InExpo(float start, float distance, float elapsedTime, float duration)
		{
			if (elapsedTime > duration)
				elapsedTime = duration;
			return (float)(distance * Math.Pow(2, 10 * (elapsedTime / duration - 1)) + start);
		}

        public static float OutExpo(float start, float distance, float elapsedTime, float duration)
		{
			if (elapsedTime > duration)
				elapsedTime = duration;
            return (float)(distance * (-Math.Pow(2, -10 * elapsedTime / duration) + 1) + start);
		}

        public static float InOutExpo(float start, float distance, float elapsedTime, float duration)
		{
			elapsedTime = (elapsedTime > duration) ? 2.0f : elapsedTime / (duration / 2);
			if (elapsedTime < 1)
				return (float)(distance / 2 * Math.Pow(2, 10 * (elapsedTime - 1)) + start);
			elapsedTime--;
            return (float)(distance / 2 * (-Math.Pow(2, -10 * elapsedTime) + 2) + start);
		}

        public static float InCirc(float start, float distance, float elapsedTime, float duration)
		{
			elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
			return (float)(-distance * (Math.Sqrt(1 - elapsedTime * elapsedTime) - 1) + start);
		}

        public static float OutCirc(float start, float distance, float elapsedTime, float duration)
		{
			elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
			elapsedTime--;
			return (float)(distance * Math.Sqrt(1 - elapsedTime * elapsedTime) + start);
		}

        public static float InOutCirc(float start, float distance, float elapsedTime, float duration)
		{
			elapsedTime = (elapsedTime > duration) ? 2.0f : elapsedTime / (duration / 2);
			if (elapsedTime < 1)
				return (float)(-distance / 2 * (Math.Sqrt(1 - elapsedTime * elapsedTime) - 1) + start);
			elapsedTime -= 2;
			return (float)(distance / 2 * (Math.Sqrt(1 - elapsedTime * elapsedTime) + 1) + start);
		}

        /*public static float _inElastic(float start, float distance, float elapsedTime, float duration)
        {
            if (elapsedTime > duration)
                elapsedTime = duration;
	
            if (elapsedTime == 0.0f)
                return start;
	
            if ((elapsedTime /= duration) == 1.0f)
                    return start + distance;
	
            float p = duration * 0.3f;
            float a = distance;
            float s = p / 4.0f;
            float postFix = a * Mathf.Pow(2.0f, 10.0 * (elapsedTime -= 1.0f));
	
            return -(postFix * Mathf.Sin((elapsedTime * duration - s) * (2.0 * Mathf.PI) / p)) + start;
        }
	
        public static float _outElastic(float start, float distance, float elapsedTime, float duration)
        {
            if (elapsedTime > duration)
                elapsedTime = duration;
	
            if (elapsedTime == 0.0f)
                return start;
	
            if ((elapsedTime /= duration) == 1.0f)
                    return start + distance;
	
            float p = duration * 0.3f;
            float a = distance;
            float s = p / 4.0f;
	
            return a * Mathf.Pow(2.0, -10.0 * elapsedTime) * Mathf.Sin((elapsedTime * duration - s) * (2.0 * Mathf.PI) / p) + distance + start;
        }
	
        public static float _inOutElastic(float start, float distance, float elapsedTime, float duration)
        {
            // TODO: implement
            return 0.0f;
        }*/
    }
}