using System;
using artiso.AdsdHotel.Blue.Api;
using Microsoft.Extensions.DependencyInjection;

// Run.

var dbConfig = new DatabaseConfiguration(
    host: "localhost",
    port: 3306,
    database: "adsd-blue",
    username: "root",
    password: null);

var services = Startup.ConfigureServices(dbConfig);

using var scope = services.CreateScope();

await Startup.SetupDatabaseAsync(scope.ServiceProvider);

Console.ReadLine();
