using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Claims.Contracts.Validation
{
    public static class ValidationErrors
    {
        public const string START_DATE_IN_PAST = "Start date cannot be in the past.";
        public const string END_DATE_TOO_FAR = "End date cannot be more than 1 year after the start date.";
        public const string END_DATE_BEFORE_START_DATE = "End date cannot be before the start date.";
        public const string CLAIM_DAMAGE_COST_EXCEEDS_LIMIT = "Damage cost cannot exceed 100,000.";
    }
}
