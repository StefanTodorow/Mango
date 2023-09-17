namespace Mango.Web.Utility
{
    public class SD
    {
        public const string RoleAdmin = "ADMIN";
        public const string RoleCustomer = "CUSTOMER";

        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE
        }
        public static string CouponAPIBase { get; set; }
        public static string AuthAPIBase { get; set; }

    }
}
