using Microsoft.EntityFrameworkCore;

class Car
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Model { get; set; }
    public int Year { get; set; }
}

class CarContext : DbContext
{
    public CarContext(DbContextOptions<CarContext> options) : base(options) { }

    public DbSet<Car> Cars => Set<Car>();
}
