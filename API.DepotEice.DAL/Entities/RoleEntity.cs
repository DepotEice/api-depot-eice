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

        /// <summary>
        /// Instanciate an object <see cref="RoleEntity"/> with all its properties
        /// </summary>
        /// <param name="id"><see cref="RoleEntity"/>'s ID</param>
        /// <param name="name"><see cref="RoleEntity"/>'s name</param>
        /// <exception cref="ArgumentNullException"></exception>
        public RoleEntity(string id, string name)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            Id = id;
            Name = name;
        }

        /// <summary>
        /// Instanciate an object <see cref="RoleEntity"/> with its name however <see cref="Id"/>
        /// is a new generated guid
        /// </summary>
        /// <param name="name">Role's name</param>
        /// <exception cref="ArgumentNullException"></exception>
        public RoleEntity(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            Id = Guid.NewGuid().ToString();
            Name = name;
        }
    }
}
