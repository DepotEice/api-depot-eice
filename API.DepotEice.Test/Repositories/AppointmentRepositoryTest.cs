using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.Test.Repositories
{
    public class AppointmentRepositoryTest
    {
        private Mock<IAppointmentRepository> _appointmentRepository;

        public List<AppointmentEntity> Appointments
        {
            get
            {
                return new List<AppointmentEntity>()
                {
                    new AppointmentEntity
                    {
                        Id= 1,
                        StartAt = new DateTime(2022,01,01,17,30,0),
                        EndAt= new DateTime(2022,01,01,18,0,0),
                        IsAccepted = false,
                        UserId = Guid.NewGuid().ToString()
                    },
                    new AppointmentEntity
                    {
                        Id= 1,
                        StartAt = new DateTime(2022,01,02,17,30,0),
                        EndAt= new DateTime(2022,01,02,18,0,0),
                        IsAccepted = false,
                        UserId = Guid.NewGuid().ToString()
                    },
                    new AppointmentEntity
                    {
                        Id= 1,
                        StartAt = new DateTime(2022,01,03,17,30,0),
                        EndAt= new DateTime(2022,01,03,18,0,0),
                        IsAccepted = false,
                        UserId = Guid.NewGuid().ToString()
                    },
                    new AppointmentEntity
                    {
                        Id= 1,
                        StartAt = new DateTime(2022,01,04,17,30,0),
                        EndAt= new DateTime(2022,01,04,18,0,0),
                        IsAccepted = false,
                        UserId = Guid.NewGuid().ToString()
                    },
                    new AppointmentEntity
                    {
                        Id= 1,
                        StartAt = new DateTime(2022,01,04,17,30,0),
                        EndAt= new DateTime(2022,01,04,18,0,0),
                        IsAccepted = false,
                        UserId = Guid.NewGuid().ToString()
                    },
                };
            }
        }

        [SetUp]
        public void Setup()
        {
            _appointmentRepository = new Mock<IAppointmentRepository>();
        }

        [Test(Author = "Soultan Hatsijev", Description = "Set the IsAccepted flag to false for an ")]
        public void SetAppointmentAccepted()
        {

        }
    }
}
