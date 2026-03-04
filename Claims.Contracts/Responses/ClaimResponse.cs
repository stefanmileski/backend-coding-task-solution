using Claims.Domain;

namespace Claims.Contracts.Responses
{
    public class ClaimResponse(string id, string coverId, DateTime created, string name, ClaimType type, decimal damageCost)
    {
        /// <summary>
        /// Claim id
        /// </summary>
        public string Id { get; } = id;

        /// <summary>
        /// Related cover id
        /// </summary>
        public string CoverId { get; } = coverId;

        /// <summary>
        /// Date when the claim was created
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
    }
}
