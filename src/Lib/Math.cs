namespace kowder {
    class Interpolation {
        public static float lerp(float a, float b, float f)
        {
            return a + f * (b - a);
        }

        public static int lerp(int a, int b, float f)
        {
            return (int) (a + f * (b - a));
        }
    }
}