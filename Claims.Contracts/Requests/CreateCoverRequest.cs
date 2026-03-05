using Claims.Contracts.Validation;
using Claims.Core.Clock;
using Claims.Domain;
using Microsoft.Extensions.Internal;
using System.ComponentModel.DataAnnotations;
using System.Threading;

namespace Claims.Contracts.Requests
{
    public class CreateCoverRequest(DateTime startDate, DateTime endDate, CoverType type): IValidatableObject
    {
        /// <summary>
        /// Start date of the cover
        /// </summary>
        public DateTime StartDate { get; set; } = startDate;

        /// <summary>
        /// End date of the cover
        /// </summary>
        public DateTime EndDate { get; set; } = endDate;

        /// <summary>
        /// Type of the cover
        /// </summary>
        public CoverType Type { get; set; } = type;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            IClock clock = validationContext.GetService(typeof(IClock)) as IClock
                ?? new Core.Clock.SystemClock();

            if (StartDate.Date < clock.UtcNow.Date)
            {
                return [new ValidationResult(ValidationErrors.START_DATE_IN_PAST, [nameof(StartDate)])];
            }

            if (EndDate.Date > StartDate.Date.AddYears(1))
            {
                return [new ValidationResult(ValidationErrors.END_DATE_TOO_FAR, [nameof(StartDate), nameof(EndDate)])];
            }

            if (EndDate.Date < StartDate.Date)
            {
                return [new ValidationResult(ValidationErrors.END_DATE_BEFORE_START_DATE, [nameof(StartDate), nameof(EndDate)])];
            }

            return [];
        }
    }
}
