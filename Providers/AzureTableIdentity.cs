namespace Starship.Azure.Providers {
    public static class AzureTableIdentity {

        public static string Padded(int id) {
            return id.ToString(IdentityFormat);
        }

        public static string Reversed(int id) {
            return id <= 0 ? long.MaxValue.ToString(IdentityFormat) : (long.MaxValue - id).ToString(IdentityFormat);
        }

        private const string IdentityFormat = "0000000000000000000";
    }
}