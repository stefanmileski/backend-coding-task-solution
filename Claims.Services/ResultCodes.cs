namespace Claims.Services
{
    public static class ResultCodes
    {
        public const string COVER_NOT_FOUND = "COVER_NOT_FOUND";

        public const string CLAIM_CREATED_NOT_WITHIN_COVER_PERIOD = "CLAIM_CREATED_NOT_WITHIN_COVER_PERIOD";

        public const string CLAIM_NOT_FOUND = "CLAIM_NOT_FOUND";

        public const string END_DATE_BEFORE_START_DATE = "END_DATE_BEFORE_START_DATE";
    }
}
