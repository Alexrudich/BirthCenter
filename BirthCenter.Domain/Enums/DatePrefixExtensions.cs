namespace BirthCenter.Domain.Enums
{
    /// <summary>
    /// Extension methods for DatePrefix enum
    /// </summary>
    public static class DatePrefixExtensions
    {
        /// <summary>
        /// Converts DatePrefix enum to its string representation
        /// </summary>
        public static string ToPrefixString(this DatePrefix prefix)
        {
            return prefix switch
            {
                DatePrefix.Eq => "eq",
                DatePrefix.Ne => "ne",
                DatePrefix.Gt => "gt",
                DatePrefix.Lt => "lt",
                DatePrefix.Ge => "ge",
                DatePrefix.Le => "le",
                DatePrefix.Sa => "sa",
                DatePrefix.Eb => "eb",
                DatePrefix.Ap => "ap",
                _ => throw new ArgumentException($"Unknown prefix: {prefix}")
            };
        }

        /// <summary>
        /// Parses string to DatePrefix enum
        /// </summary>
        public static DatePrefix ParsePrefix(this string prefix)
        {
            return prefix?.ToLowerInvariant() switch
            {
                "eq" => DatePrefix.Eq,
                "ne" => DatePrefix.Ne,
                "gt" => DatePrefix.Gt,
                "lt" => DatePrefix.Lt,
                "ge" => DatePrefix.Ge,
                "le" => DatePrefix.Le,
                "sa" => DatePrefix.Sa,
                "eb" => DatePrefix.Eb,
                "ap" => DatePrefix.Ap,
                _ => throw new ArgumentException($"Invalid prefix: {prefix}")
            };
        }

        /// <summary>
        /// All valid prefixes as strings
        /// </summary>
        public static string[] AllPrefixStrings =>
            Enum.GetValues<DatePrefix>().Select(p => p.ToPrefixString()).ToArray();
    }
}