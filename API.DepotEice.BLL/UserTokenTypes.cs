using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.BLL
{
    /// <summary>
    /// Static class with the different User Token types
    /// </summary>
    public static class UserTokenTypes
    {
        /// <summary>
        /// Token type for email confirmation when a user is created
        /// </summary>
        public const string EMAIL_CONFIRMATION_TOKEN = "EMAIL_CONFIRMATION_TOKEN";
    }
}
