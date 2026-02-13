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

            // YYYY, YYYY-MM, YYYY-MM-DD, YYYY-MM-DDThh:mm:ss
            if (DateTime.TryParse(param, out var exactDate))
            {
                result.ExactDate = exactDate;
                result.IsPartial = param.Length < 10; // меньше чем YYYY-MM-DD

                if (result.IsPartial)
                {
                    result.IsRange = true;

                    if (param.Length == 4) // YYYY
                    {
                        var year = int.Parse(param);
                        result.StartDate = new DateTime(year, 1, 1, 0, 0, 0);
                        result.EndDate = new DateTime(year, 12, 31, 23, 59, 59);
                    }
                    else if (param.Length == 7) // YYYY-MM
                    {
                        var parts = param.Split('-');
                        var year = int.Parse(parts[0]);
                        var month = int.Parse(parts[1]);
                        result.StartDate = new DateTime(year, month, 1, 0, 0, 0);
                        result.EndDate = new DateTime(year, month,
                            DateTime.DaysInMonth(year, month), 23, 59, 59);
                    }
                }
            }

            return result;
        }
    }
}