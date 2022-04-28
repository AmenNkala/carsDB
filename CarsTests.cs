using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Xunit;

public class CarsTests
{
    [Fact]
    public async Task GetCars()
    {
        await using var application = new CarsApiApplication();

        var client = application.CreateClient();
        var cars = await client.GetFromJsonAsync<List<Car>>("/cars");

        Assert.Empty(cars);
    }

    [Fact]
    public async Task PostCars()
    {
        await using var application = new CarsApiApplication();

        var client = application.CreateClient();
        var response = await client.PostAsJsonAsync("/cars", new Car { Name = "Rolls Royce" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var cars = await client.GetFromJsonAsync<List<Car>>("/cars");

        var car = Assert.Single(cars);
        Assert.Equal("Rolls Royce", car.Name);
    }

    [Fact]
    public async Task DeleteCar()
    {
        await using var application = new CarsApiApplication();

        var client = application.CreateClient();
        var response = await client.PostAsJsonAsync("/cars", new Car { Name = "Rolls Royce" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var cars = await client.GetFromJsonAsync<List<Car>>("/cars");

        var car = Assert.Single(cars);
        Assert.Equal("Rolls Royce", car.Name);

        response = await client.DeleteAsync("/cars/{Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        response = await client.GetAsync("/cars/{Id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}

class CarsApiApplication : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        var root = new InMemoryDatabaseRoot();

        builder.ConfigureServices(
            services =>
            {
                services.RemoveAll(typeof(DbContextOptions<CarContext>));
                services.AddDbContext<CarContext>(
                    options => options.UseInMemoryDatabase("Testing", root)
                );
            }
        );

        return base.CreateHost(builder);
    }
}
