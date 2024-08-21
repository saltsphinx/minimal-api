using Microsoft.EntityFrameworkCore;

public class HCSCDb : DbContext
{
    public HCSCDb(DbContextOptions<HCSCDb> options) : base(options) {}

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Department> Departments => Set<Department>();
}