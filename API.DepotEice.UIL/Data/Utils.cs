namespace API.DepotEice.UIL.Data
{
    /// <summary>
    /// Utils class contains static functions that can be used in the whole project
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// The key of the default profile picture in AWS S3
        /// </summary>
        public const string DefaultProfilePicture = "NO-PICTURE.svg";

        /// <summary>
        /// The range of the date that can be used in the project
        /// </summary>
        public enum DateRange
        {
            /// <summary>
            /// The day range
            /// </summary>
            Day,

            /// <summary>
            /// The week range
            /// </summary>
            Week,

            /// <summary>
            /// The month range
            /// </summary>
            Month,

            /// <summary>
            /// The year range
            /// </summary>
            Year
        }
    }
}
