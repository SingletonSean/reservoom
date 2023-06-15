using Microsoft.Extensions.DependencyInjection;
using Reservoom.DbContexts;
using Reservoom.Models;
using Reservoom.Services.ReservationConflictValidators;
using Reservoom.Services.ReservationCreators;
using Reservoom.Services.ReservationProviders;
using Reservoom.Services;
using Reservoom.Stores;
using Reservoom.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Reservoom.DTOs;

namespace Reservoom.Test.ViewModels
{
    public class MakeReservationViewModelTest
    {
        [Test]
        public async Task ExecuteSubmitCommand_WithValidReservation_CreatesReservation()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<MakeReservationViewModel>();
            services.AddSingleton<HotelStore>();
            services.AddSingleton<Hotel>(s => new Hotel("Hotel Name", s.GetRequiredService<ReservationBook>()));
            services.AddSingleton<ReservationBook>();
            services.AddSingleton<IReservationProvider, DatabaseReservationProvider>();
            services.AddSingleton<IReservationCreator, DatabaseReservationCreator>();
            services.AddSingleton<IReservationConflictValidator, DatabaseReservationConflictValidator>();
            services.AddSingleton<IReservoomDbContextFactory, InMemoryReservoomDbContextFactory>();
            services.AddSingleton<NavigationService<ReservationListingViewModel>>();
            services.AddSingleton<NavigationStore>();
            services.AddSingleton<Func<ReservationListingViewModel>>(() => null!);

            ServiceProvider serviceProvider = services.BuildServiceProvider();

            IReservoomDbContextFactory dbContextFactory = serviceProvider.GetRequiredService<IReservoomDbContextFactory>();
            ReservoomDbContext migrationDbContext = dbContextFactory.CreateDbContext();
            await migrationDbContext.Database.MigrateAsync();

            MakeReservationViewModel viewModel = serviceProvider.GetRequiredService<MakeReservationViewModel>();

            viewModel.Username = "SingletonSean";
            viewModel.FloorNumber = 1;
            viewModel.RoomNumber = 2;
            viewModel.StartDate = new DateTime(2000, 1, 1);
            viewModel.EndDate = new DateTime(2000, 1, 2);

            await viewModel.SubmitCommand.ExecuteAsync(null);

            ReservoomDbContext dbContext = dbContextFactory.CreateDbContext();
            ReservationDTO createdReservation = await dbContext
                .Reservations
                .FirstOrDefaultAsync((r) =>
                    r.Username == "SingletonSean" &&
                    r.FloorNumber == 1 &&
                    r.RoomNumber == 2 &&
                    r.StartTime == new DateTime(2000, 1, 1) &&
                    r.EndTime == new DateTime(2000, 1, 2));

            Assert.That(createdReservation, Is.Not.Null);
        }
    }
}
