using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BirthCenter.Infrastructure.Services
{
    public static class FhirDateParser
    {
        private static readonly string[] Prefixes = { "eq", "ne", "gt", "lt", "ge", "le", "sa", "eb", "ap" };

        public class DateSearchCriteria
        {
            public string Prefix { get; set; } = "eq"; // по умолчанию exact match
            public DateTime? ExactDate { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public bool IsPartial { get; set; }
            public bool IsRange { get; set; }
        }

        public static DateSearchCriteria Parse(string dateParam)
        {
            if (string.IsNullOrWhiteSpace(dateParam))
                throw new ArgumentException("Date parameter cannot be empty");

            var result = new DateSearchCriteria();
            var param = dateParam.Trim();

            // 1. Определяем префикс
            foreach (var prefix in Prefixes)
            {
                if (param.StartsWith(prefix))
                {
                    result.Prefix = prefix;
                    param = param.Substring(prefix.Length);
                    break;
                }
            }

            // 2. Проверяем частичные даты по длине строки
            if (param.Length == 4 && int.TryParse(param, out var year))
            {
                // YYYY
                result.IsPartial = true;
                result.IsRange = true;
                result.StartDate = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                result.EndDate = new DateTime(year, 12, 31, 23, 59, 59, DateTimeKind.Utc);
                return result;
            }

            if (param.Length == 7 && param.Contains('-'))
            {
                // YYYY-MM
                var parts = param.Split('-');
                if (parts.Length == 2 &&
                    int.TryParse(parts[0], out year) &&
                    int.TryParse(parts[1], out var month) &&
                    month >= 1 && month <= 12)
                {
                    result.IsPartial = true;
                    result.IsRange = true;
                    result.StartDate = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
                    result.EndDate = new DateTime(year, month,
                        DateTime.DaysInMonth(year, month), 23, 59, 59, DateTimeKind.Utc);
                    return result;
                }
            }

            if (param.Length == 10 && DateTime.TryParse(param, out var exactDate))
            {
                // YYYY-MM-DD
                result.ExactDate = DateTime.SpecifyKind(exactDate, DateTimeKind.Utc);
                result.IsPartial = false;
                result.IsRange = false;
                return result;
            }

            if (DateTime.TryParse(param, out var fullDate))
            {
                // Полная дата с временем
                result.ExactDate = DateTime.SpecifyKind(fullDate, DateTimeKind.Utc);
                result.IsPartial = false;
                result.IsRange = false;
                return result;
            }

            throw new ArgumentException($"Invalid date format: {param}");
        }
    }
}