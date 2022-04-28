using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<CarContext>(opt => opt.UseInMemoryDatabase("CarsList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();

app.MapGet("/", () => "Welcome to my site");
app.MapGet("/cars", async (CarContext db) => await db.Cars.ToListAsync());

app.MapGet(
    "/cars/{id}",
    async (int id, CarContext db) =>
        await db.Cars.FindAsync(id) is Car car ? Results.Ok(car) : Results.NotFound()
);

app.MapPost(
    "/cars",
    async (Car car, CarContext db) =>
    {
        db.Cars.Add(car);
        await db.SaveChangesAsync();

        return Results.Created($"/cars/{car.Id}", car);
    }
);

app.MapPut(
    "/cars/{id}",
    async (int id, Car car, CarContext db) =>
    {
        var item = await db.Cars.FindAsync(id);

        if (item is null)
            return Results.NotFound();

        try
        {
            item.Name = car.Name;
            item.Model = car.Model;
            item.Year = car.Year;
        }
        catch (System.Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        await db.SaveChangesAsync();

        return Results.NoContent();
    }
);

app.MapDelete(
    "/cars/{id}",
    async (int id, CarContext db) =>
    {
        if (await db.Cars.FindAsync(id) is Car car)
        {
            db.Cars.Remove(car);
            await db.SaveChangesAsync();
            return Results.Ok(car);
        }

        return Results.NotFound("Record Not Found");
    }
);

app.MapGet(
    "writeToFile",
    async (CarContext db) =>
    {
        List<Car> cars = await db.Cars.ToListAsync();
        using StreamWriter file = new("cars.txt");

        try
        {
            foreach (Car car in cars)
            {
                await file.WriteLineAsync(car.Name + " " + car.Model);
            }
        }
        catch (System.Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
);

app.MapGet(
    "count",
    (CarContext db) =>
    {
        int count = (from car in db.Cars where car.Year > 2019 select car).Count();
        return JsonConvert.SerializeObject(count);
    }
);

app.Run();
