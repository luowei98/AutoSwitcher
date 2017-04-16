namespace AutoSwitcher
{
    public static class StringEx
    {
        public static double ToNum(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return 0;
            }

            var result = 0d;
            try
            {
                double.TryParse(value, out result);
                return result;
            }
            catch
            {
                // ignored
            }
            return result;
        }
    }
}
