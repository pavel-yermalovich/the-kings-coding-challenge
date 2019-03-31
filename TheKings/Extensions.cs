namespace TheKings
{
    public static class Extensions
    {
        public static bool IsNotNullOrEmpty(this string str)
        {
            return !string.IsNullOrEmpty(str);
        } 
    }
}
