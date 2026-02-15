namespace BirthCenter.Domain.Enums
{
    /// <summary>
    /// Represents the gender of a patient according to FHIR specification
    /// </summary>
    public enum Gender
    {
        /// <summary>
        /// Male gender
        /// </summary>
        Male,

        /// <summary>
        /// Female gender
        /// </summary>
        Female,

        /// <summary>
        /// Other gender (non-binary, intersex, etc.)
        /// </summary>
        Other,

        /// <summary>
        /// Unknown or not disclosed gender
        /// </summary>
        Unknown
    }
}