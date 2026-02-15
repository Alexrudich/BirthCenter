using BirthCenter.Domain.Enums;

namespace BirthCenter.Domain.Specifications
{
    /// <summary>
    /// Represents parsed FHIR date search criteria
    /// </summary>
    public class DateSearchCriteria
    {
        /// <summary>
        /// Search prefix (eq, ne, gt, lt, ge, le, sa, eb, ap)
        /// </summary>
        public DatePrefix Prefix { get; set; } = DatePrefix.Eq;

        /// <summary>
        /// Exact date value for non-range searches
        /// </summary>
        public DateTime? ExactDate { get; set; }

        /// <summary>
        /// Start of date range for partial dates
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// End of date range for partial dates
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Indicates if this is a partial date (YYYY or YYYY-MM)
        /// </summary>
        public bool IsPartial { get; set; }

        /// <summary>
        /// Indicates if this is a range search
        /// </summary>
        public bool IsRange { get; set; }
    }
}