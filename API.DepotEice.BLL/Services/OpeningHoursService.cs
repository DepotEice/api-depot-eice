using API.DepotEice.BLL.IServices;
using API.DepotEice.BLL.Dtos;
using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.BLL.Services
{
    public class OpeningHoursService : IOpeningHoursService
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IOpeningHoursRepository _openingHoursRepository;

        public OpeningHoursService(ILogger<OpeningHoursService> logger, IMapper mapper,
            IOpeningHoursRepository openingHoursRepository)
        {
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (mapper is null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }

            if (openingHoursRepository is null)
            {
                throw new ArgumentNullException(nameof(openingHoursRepository));
            }

            _logger = logger;
            _mapper = mapper;
            _openingHoursRepository = openingHoursRepository;
        }

        /// <summary>
        /// Create an OpeningHours in the database
        /// </summary>
        /// <param name="model">
        /// The OpeningHours to create
        /// </param>
        /// <returns>
        /// <c>null</c> If the creation failed or if an error occured during retrieval. Otherwise 
        /// an instance of <see cref="OpeningHoursDto"/>
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public OpeningHoursDto? CreateOpeningHours(OpeningHoursDto model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            OpeningHoursEntity openingHoursEntity = _mapper.Map<OpeningHoursEntity>(model);

            int newId = _openingHoursRepository.Create(openingHoursEntity);

            if (newId <= 0)
            {
                _logger.LogWarning("{date} - The OpeningHours creation failed",
                    DateTime.Now);

                return null;
            }

            OpeningHoursEntity? openingHoursFromRepo = _openingHoursRepository.GetByKey(newId);

            if (openingHoursFromRepo is null)
            {
                _logger.LogError(
                    "{date} - The retrieved OpeningHours with new ID : {id} is null!",
                    DateTime.Now, newId);

                return null;
            }

            OpeningHoursDto openingHoursModel =
                _mapper.Map<OpeningHoursDto>(openingHoursFromRepo);

            return openingHoursModel;
        }

        /// <summary>
        /// Delete an OpeningHours from the database
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// <c>true</c> If the deletion was successful. <c>false</c> If the OpeningHours does not
        /// exist or if the deletion failed
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool DeleteOpeningHours(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentNullException(nameof(id));
            }

            OpeningHoursEntity? openingHours = _openingHoursRepository.GetByKey(id);

            if (openingHours is null)
            {
                _logger.LogWarning(
                    "{date} - The OpeningHours with ID \"{id}\" does not exist in the " +
                    "database!",
                    DateTime.Now, id);

                return false;
            }

            return _openingHoursRepository.Delete(openingHours);
        }

        /// <summary>
        /// Retrieve all OpeningHours from the database
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="Openinghours"/>
        /// </returns>
        public IEnumerable<OpeningHoursDto> GetOpeningHours()
        {
            IEnumerable<OpeningHoursEntity> openingHoursFromRepo = _openingHoursRepository.GetAll();

            foreach (OpeningHoursEntity? openinghours in openingHoursFromRepo)
            {
                OpeningHoursDto openingHoursModel = _mapper.Map<OpeningHoursDto>(openinghours);

                yield return openingHoursModel;
            }
        }

        /// <summary>
        /// Update and OpeningHours and return the updated value
        /// </summary>
        /// <param name="model">
        /// The OpeningHours to update
        /// </param>
        /// <returns>
        /// <c>null</c> If the update failed. Otherwise, an instance of 
        /// <see cref="OpeningHoursDto"/>
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public OpeningHoursDto? UpdateOpeningHours(OpeningHoursDto model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            OpeningHoursEntity openingHoursToCreate = _mapper.Map<OpeningHoursEntity>(model);

            if (!_openingHoursRepository.Update(openingHoursToCreate))
            {
                _logger.LogWarning(
                    "{date} - The update operation failed for OpeningHours with ID " +
                    "\"{id}\"!",
                    DateTime.Now, model.Id);

                return null;
            }

            OpeningHoursEntity? openingHoursFromRepo = _openingHoursRepository.GetByKey(model.Id);

            if (openingHoursFromRepo is null)
            {
                _logger.LogError(
                    "{date} - The retrieved OpeningHours with ID \"{id}\" is null!",
                    DateTime.Now, model.Id);

                return null;
            }

            OpeningHoursDto openingHoursModel =
                _mapper.Map<OpeningHoursDto>(openingHoursFromRepo);

            return openingHoursModel;
        }
    }
}
