﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Reservoom.DbConnections;
using Reservoom.DbContexts;
using Reservoom.DbInitializers;
using Reservoom.Exceptions;
using Reservoom.HostBuilders;
using Reservoom.Models;
using Reservoom.Services;
using Reservoom.Services.ReservationConflictValidators;
using Reservoom.Services.ReservationCreators;
using Reservoom.Services.ReservationProviders;
using Reservoom.Stores;
using Reservoom.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Reservoom
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost _host;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .AddViewModels()
                .ConfigureServices((hostContext, services) =>
                {
                    string connectionString = hostContext.Configuration.GetConnectionString("Default");

                    services.AddSingleton(new SqliteDbConnectionFactory(connectionString));
                    services.AddSingleton<SqliteDbInitializer>();

                    services.AddSingleton<IReservationProvider, DapperReservationProvider>();
                    services.AddSingleton<IReservationCreator, DapperReservationCreator>();
                    services.AddSingleton<IReservationConflictValidator, DapperReservationConflictValidator>();

                    services.AddTransient<ReservationBook>();

                    string hotelName = hostContext.Configuration.GetValue<string>("HotelName");
                    services.AddSingleton((s) => new Hotel(hotelName, s.GetRequiredService<ReservationBook>()));

                    services.AddSingleton<HotelStore>();
                    services.AddSingleton<NavigationStore>();

                    services.AddSingleton(s => new MainWindow()
                    {
                        DataContext = s.GetRequiredService<MainViewModel>()
                    });
                })
                .Build();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _host.Start();

            SqliteDbInitializer dbInitializer = _host.Services.GetRequiredService<SqliteDbInitializer>();
            dbInitializer.Initialize();

            NavigationService<ReservationListingViewModel> navigationService = _host.Services.GetRequiredService<NavigationService<ReservationListingViewModel>>();
            navigationService.Navigate();

            MainWindow = _host.Services.GetRequiredService<MainWindow>();
            MainWindow.Show();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _host.Dispose();

            base.OnExit(e);
        }
    }
}
