namespace BirthCenter.Domain.Enums
{
    /// <summary>
    /// FHIR date search prefixes
    /// </summary>
    public enum DatePrefix
    {
        /// <summary>equal</summary>
        Eq,
        /// <summary>not equal</summary>
        Ne,
        /// <summary>greater than</summary>
        Gt,
        /// <summary>less than</summary>
        Lt,
        /// <summary>greater or equal</summary>
        Ge,
        /// <summary>less or equal</summary>
        Le,
        /// <summary>starts after</summary>
        Sa,
        /// <summary>ends before</summary>
        Eb,
        /// <summary>approximately</summary>
        Ap
    }
}