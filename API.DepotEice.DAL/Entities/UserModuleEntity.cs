using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.DAL.Entities
{
    /// <summary>
    /// The entity representing the UserModules table
    /// </summary>
    public class UserModuleEntity
    {
        /// <summary>
        /// The id of the user
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// The id of the module
        /// </summary>
        public int ModuleId { get; set; }

        /// <summary>
        /// The status of the user in the module
        /// </summary>
        public bool IsAccepted { get; set; }
    }
}
