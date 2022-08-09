using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.DAL.Entities
{
    /// <summary>
    /// Represents <c>Modules</c> table in the database
    /// </summary>
    public class ModuleEntity
    {
        /// <summary>
        /// <see cref="ModuleEntity"/>'s ID column in the database
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// <see cref="ModuleEntity"/>'s name column in the database
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// <see cref="ModuleEntity"/>'s description column in the database
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }
}
