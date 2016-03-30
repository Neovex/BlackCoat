using System;

namespace BlackCoat.Animation
{
    public enum InterpolationType
    {
        Linear,
        InQuad,
        OutQuad,
        InOutQuad,
        InCubic,
        OutCubic,
        InOutCubic,
        InQuart,
        OutQuart,
        InOutQuart,
        InQuint,
        OutQuint,
        InOutQuint,
        InSine,
        OutSine,
        InOutSine,
        InExpo,
        OutExpo,
        InOutExpo,
        InCirc,
        OutCirc,
        InOutCirc,
        InElastic,
        OutElastic
    }

    /// <summary>
    /// Common interpolations for Tweening
    /// </summary>
    public static class Interpolation
    {
        /// <summary>
        /// Retrieves an interpolation delegate according to the requested type.
        /// </summary>
        /// <param name="type">Desired interpolation type</param>
        /// <returns>Delegate representing the requested interpolation type</returns>
        public static Func<float, float, float, float, float> Get(InterpolationType type)
        {
            switch (type)
            {
                case InterpolationType.Linear: return Linear;
                case InterpolationType.InQuad: return InQuad;
                case InterpolationType.OutQuad: return OutQuad;
                case InterpolationType.InOutQuad: return InOutQuad;
                case InterpolationType.InCubic: return InCubic;
                case InterpolationType.OutCubic: return OutCubic;
                case InterpolationType.InOutCubic: return InOutCubic;
                case InterpolationType.InQuart: return InQuart;
                case InterpolationType.OutQuart: return OutQuart;
                case InterpolationType.InOutQuart: return InOutQuart;
                case InterpolationType.InQuint: return InQuint;
                case InterpolationType.OutQuint: return OutQuint;
                case InterpolationType.InOutQuint: return InOutQuint;
                case InterpolationType.InSine: return InSine;
                case InterpolationType.OutSine: return OutSine;
                case InterpolationType.InOutSine: return InOutSine;
                case InterpolationType.InExpo: return InExpo;
                case InterpolationType.OutExpo: return OutExpo;
                case InterpolationType.InOutExpo: return InOutExpo;
                case InterpolationType.InCirc: return InCirc;
                case InterpolationType.OutCirc: return OutCirc;
                case InterpolationType.InOutCirc: return InOutCirc;
                case InterpolationType.InElastic: return InElastic;
                case InterpolationType.OutElastic: return OutElastic;
            }
            return Linear;
        }

        /// <summary>
        /// Calculates an interpolation value according to the provided informations.
        /// </summary>
        /// <param name="type">Desired interpolation type</param>
        /// <returns>Interpolated value</returns>
        public static float Calculate(InterpolationType type, float start, float distance, float elapsedTime, float duration)
        {
            return Get(type).Invoke(start, distance, elapsedTime, duration);
        }

        internal static float Linear(float start, float distance, float elapsedTime, float duration)
		{
			if (elapsedTime > duration) elapsedTime = duration;
	  		return distance * (elapsedTime / duration) + start;
		}

        internal static float InQuad(float start, float distance, float elapsedTime, float duration)
		{
			elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
	  		return distance * elapsedTime * elapsedTime + start;
		}

        internal static float OutQuad(float start, float distance, float elapsedTime, float duration)
		{
			elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
			return -distance * elapsedTime * (elapsedTime - 2.0f) + start;
		}

        internal static float InOutQuad(float start, float distance, float elapsedTime, float duration)
		{
			elapsedTime = (elapsedTime > duration) ? 2.0f : elapsedTime / (duration / 2);
			if (elapsedTime < 1) return distance / 2.0f * elapsedTime * elapsedTime + start;
			elapsedTime--;
			return -distance / 2.0f * (elapsedTime * (elapsedTime - 2.0f) - 1.0f) + start;
		}

        internal static float InCubic(float start, float distance, float elapsedTime, float duration)
		{
			elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
			return distance * elapsedTime * elapsedTime * elapsedTime + start;
		}

        internal static float OutCubic(float start, float distance, float elapsedTime, float duration)
		{
			elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
			elapsedTime--;
			return distance * (elapsedTime * elapsedTime * elapsedTime + 1) + start;
		}

        internal static float InOutCubic(float start, float distance, float elapsedTime, float duration)
		{
			elapsedTime = (elapsedTime > duration) ? 2.0f : elapsedTime / (duration / 2);
			if (elapsedTime < 1)
				return distance / 2 * elapsedTime * elapsedTime * elapsedTime + start;
			elapsedTime -= 2;
			return distance / 2 * (elapsedTime * elapsedTime * elapsedTime + 2) + start;
		}

        internal static float InQuart(float start, float distance, float elapsedTime, float duration)
		{
			elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
			return distance * elapsedTime * elapsedTime * elapsedTime * elapsedTime + start;
		}

        internal static float OutQuart(float start, float distance, float elapsedTime, float duration)
		{
			elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
			elapsedTime--;
			return -distance * (elapsedTime * elapsedTime * elapsedTime * elapsedTime - 1) + start;
		}

        internal static float InOutQuart(float start, float distance, float elapsedTime, float duration)
		{
			elapsedTime = (elapsedTime > duration) ? 2.0f : elapsedTime / (duration / 2);
			if (elapsedTime < 1)
				return distance / 2 * elapsedTime * elapsedTime * elapsedTime * elapsedTime + start;
			elapsedTime -= 2;
			return -distance / 2 * (elapsedTime * elapsedTime * elapsedTime * elapsedTime - 2) + start;
		}

        internal static float InQuint(float start, float distance, float elapsedTime, float duration)
		{
			elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
			return distance * elapsedTime * elapsedTime * elapsedTime * elapsedTime * elapsedTime + start;
		}

        internal static float OutQuint(float start, float distance, float elapsedTime, float duration)
		{
			elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
			elapsedTime--;
			return distance * (elapsedTime * elapsedTime * elapsedTime * elapsedTime * elapsedTime + 1) + start;
		}

        internal static float InOutQuint(float start, float distance, float elapsedTime, float duration)
		{
			elapsedTime = (elapsedTime > duration) ? 2.0f : elapsedTime / (duration / 2);
			if (elapsedTime < 1)
				return distance / 2 * elapsedTime * elapsedTime * elapsedTime * elapsedTime * elapsedTime + start;
			elapsedTime -= 2;
			return distance / 2 * (elapsedTime * elapsedTime * elapsedTime * elapsedTime * elapsedTime + 2) + start;
		}

        internal static float InSine(float start, float distance, float elapsedTime, float duration)
		{
			if (elapsedTime > duration)
				elapsedTime = duration;
			return (float)(-distance * Math.Cos(elapsedTime / duration * (Math.PI / 2)) + distance + start);
		}

        internal static float OutSine(float start, float distance, float elapsedTime, float duration)
		{
			if (elapsedTime > duration)
				elapsedTime = duration;
			return (float)(distance * Math.Sin(elapsedTime / duration * (Math.PI / 2)) + start);
		}

        internal static float InOutSine(float start, float distance, float elapsedTime, float duration)
		{
			if (elapsedTime > duration)
				elapsedTime = duration;
			return (float)(-distance / 2 * (Math.Cos(Math.PI * elapsedTime / duration) - 1) + start);
		}

        internal static float InExpo(float start, float distance, float elapsedTime, float duration)
		{
			if (elapsedTime > duration)
				elapsedTime = duration;
			return (float)(distance * Math.Pow(2, 10 * (elapsedTime / duration - 1)) + start);
		}

        internal static float OutExpo(float start, float distance, float elapsedTime, float duration)
		{
			if (elapsedTime > duration)
				elapsedTime = duration;
            return (float)(distance * (-Math.Pow(2, -10 * elapsedTime / duration) + 1) + start);
		}

        internal static float InOutExpo(float start, float distance, float elapsedTime, float duration)
		{
			elapsedTime = (elapsedTime > duration) ? 2.0f : elapsedTime / (duration / 2);
			if (elapsedTime < 1)
				return (float)(distance / 2 * Math.Pow(2, 10 * (elapsedTime - 1)) + start);
			elapsedTime--;
            return (float)(distance / 2 * (-Math.Pow(2, -10 * elapsedTime) + 2) + start);
		}

        internal static float InCirc(float start, float distance, float elapsedTime, float duration)
		{
			elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
			return (float)(-distance * (Math.Sqrt(1 - elapsedTime * elapsedTime) - 1) + start);
		}

        internal static float OutCirc(float start, float distance, float elapsedTime, float duration)
		{
			elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
			elapsedTime--;
			return (float)(distance * Math.Sqrt(1 - elapsedTime * elapsedTime) + start);
		}

        internal static float InOutCirc(float start, float distance, float elapsedTime, float duration)
		{
			elapsedTime = (elapsedTime > duration) ? 2.0f : elapsedTime / (duration / 2);
			if (elapsedTime < 1)
				return (float)(-distance / 2 * (Math.Sqrt(1 - elapsedTime * elapsedTime) - 1) + start);
			elapsedTime -= 2;
			return (float)(distance / 2 * (Math.Sqrt(1 - elapsedTime * elapsedTime) + 1) + start);
		}

        internal static float InElastic(float start, float distance, float elapsedTime, float duration)
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
            double postFix = a * Math.Pow(2.0f, 10.0 * (elapsedTime -= 1.0f));
	
            return (float)(-(postFix * Math.Sin((elapsedTime * duration - s) * (2.0 * Math.PI) / p)) + start);
        }

        internal static float OutElastic(float start, float distance, float elapsedTime, float duration)
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
	
            return (float)(a * Math.Pow(2.0, -10.0 * elapsedTime) * Math.Sin((elapsedTime * duration - s) * (2.0 * Math.PI) / p) + distance + start);
        }
    }
}