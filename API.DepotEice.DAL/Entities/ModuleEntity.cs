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
        public string Name { get; set; }

        /// <summary>
        /// <see cref="ModuleEntity"/>'s description column in the database
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Instanciate an object <see cref="ModuleEntity"/> with all its properties
        /// </summary>
        /// <param name="id">Module's ID</param>
        /// <param name="name">Module's name</param>
        /// <param name="description">Module's description</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public ModuleEntity(int id, string name, string description)
        {
            if (id < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrEmpty(description))
            {
                throw new ArgumentNullException(nameof(description));
            }

            Id = id;
            Name = name;
            Description = description;
        }

        /// <summary>
        /// Instanciate an object <see cref="ModuleEntity"/> with all its properties except
        /// <see cref="Id"/> that is 0
        /// </summary>
        /// <param name="name">Module's name</param>
        /// <param name="description">Module's description</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ModuleEntity(string name, string description)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrEmpty(description))
            {
                throw new ArgumentNullException(nameof(description));
            }

            Id = 0;
            Name = name;
            Description = description;
        }
    }
}
