using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.DAL.Entities
{
    /// <summary>
    /// Represent the table <c>Roles</c> in the database
    /// </summary>
    public class RoleEntity
    {
        /// <summary>
        /// The ID of the role
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The name of the role
        /// </summary>
        public string Name { get; set; }

        public RoleEntity()
        {

        }
    }
}
