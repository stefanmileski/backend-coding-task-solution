using Claims.Contracts.Validation;
using Claims.Domain;
using System.ComponentModel.DataAnnotations;
namespace Claims.Contracts.Requests
{
    public class CreateClaimRequest(string coverId, DateTime created, string name, ClaimType type, decimal damageCost): IValidatableObject
    {
        /// <summary>
        /// Related cover id
        /// </summary>
        public string CoverId { get; } = coverId;

        /// <summary>
        /// The date when the claim was created
        /// </summary>
        public DateTime Created { get; } = created;

        /// <summary>
        /// Name of the claim
        /// </summary>
        public string Name { get; } = name;

        /// <summary>
        /// Type of the claim
        /// </summary>
        public ClaimType Type { get; } = type;

        /// <summary>
        /// Cost of the damage
        /// </summary>
        public decimal DamageCost { get; } = damageCost;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DamageCost > 100000)
            {
                return [new ValidationResult(ValidationErrors.CLAIM_DAMAGE_COST_EXCEEDS_LIMIT, [nameof(DamageCost)])];
            }

            return [];
        }
    }
}