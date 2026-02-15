using BirthCenter.Domain.Enums;
using BirthCenter.Domain.Specifications;

namespace BirthCenter.Infrastructure.Services
{
    public static class FhirDateParser
    {
        // Constants for date formats
        private const int YearLength = 4;
        private const int YearMonthLength = 7;
        private const int FullDateLength = 10;

        public static DateSearchCriteria Parse(string dateParam)
        {
            if (string.IsNullOrWhiteSpace(dateParam))
                throw new ArgumentException("Date parameter cannot be empty");

            var result = new DateSearchCriteria();
            var param = dateParam.Trim();

            ParsePrefix(result, ref param);
            return ParseDateByFormat(param, result);
        }

        private static void ParsePrefix(DateSearchCriteria result, ref string param)
        {
            foreach (var prefix in DatePrefixExtensions.AllPrefixStrings)
            {
                if (param.StartsWith(prefix))
                {
                    result.Prefix = prefix.ParsePrefix();
                    param = param.Substring(prefix.Length);
                    break;
                }
            }
        }

        private static DateSearchCriteria ParseDateByFormat(string param, DateSearchCriteria result)
        {
            // Year only (YYYY)
            if (param.Length == YearLength && int.TryParse(param, out var year))
            {
                return CreateYearRange(result, year);
            }

            // Year-month (YYYY-MM)
            if (param.Length == YearMonthLength && param.Contains('-'))
            {
                return ParseYearMonth(param, result);
            }

            // Full date (YYYY-MM-DD)
            if (param.Length == FullDateLength && DateTime.TryParse(param, out var exactDate))
            {
                return CreateExactDate(result, exactDate);
            }

            // Full date with time
            if (DateTime.TryParse(param, out var fullDate))
            {
                return CreateExactDate(result, fullDate);
            }

            throw new ArgumentException($"Invalid date format: {param}");
        }

        private static DateSearchCriteria ParseYearMonth(string param, DateSearchCriteria result)
        {
            var parts = param.Split('-');

            if (parts.Length != 2 ||
                !int.TryParse(parts[0], out var year) ||
                !int.TryParse(parts[1], out var month) ||
                !IsValidMonth(month))
            {
                throw new ArgumentException($"Invalid year-month format: {param}");
            }

            result.IsPartial = true;
            result.IsRange = true;
            result.StartDate = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
            result.EndDate = new DateTime(year, month,
                DateTime.DaysInMonth(year, month), 23, 59, 59, DateTimeKind.Utc);

            return result;
        }

        private static DateSearchCriteria CreateYearRange(DateSearchCriteria result, int year)
        {
            result.IsPartial = true;
            result.IsRange = true;
            result.StartDate = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            result.EndDate = new DateTime(year, 12, 31, 23, 59, 59, DateTimeKind.Utc);

            return result;
        }

        private static DateSearchCriteria CreateExactDate(DateSearchCriteria result, DateTime date)
        {
            result.ExactDate = DateTime.SpecifyKind(date, DateTimeKind.Utc);
            result.IsPartial = false;
            result.IsRange = false;

            return result;
        }

        private static bool IsValidMonth(int month)
            => month >= 1 && month <= 12;
    }
}