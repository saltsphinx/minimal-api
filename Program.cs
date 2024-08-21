using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<HCSCDb>(opt => opt.UseInMemoryDatabase("HCSC"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();


app.MapGet("/", () => "Welcome!");

RouteGroupBuilder employeesGroup = app.MapGroup("/employees");

employeesGroup.MapGet("/", async (HCSCDb db) =>
    await db.Employees.ToListAsync());
 
employeesGroup.MapGet("/managers", async (HCSCDb db) =>
    await db.Employees.Where(employee => employee.IsManager).ToListAsync());

employeesGroup.MapGet("/{id}", async (int id, HCSCDb db) =>
    await db.Employees.FindAsync(id)
        is Employee employee
            ? Results.Ok(employee)
            : Results.NotFound());

employeesGroup.MapPost("/", async (Employee employee, HCSCDb db) =>
{
    db.Employees.Add(employee);
    await db.SaveChangesAsync();

    return Results.Created($"/{employee.Id}", employee);
});

employeesGroup.MapPut("/{id}", async (int Id, Employee inputEmployee, HCSCDb db) =>
{
    var employee = await db.Employees.FindAsync(Id);

    if (employee is null) return Results.NotFound();

    employee.Name = inputEmployee.Name;
    employee.IsManager = inputEmployee.IsManager;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

employeesGroup.MapDelete("/{id}", async (int id, HCSCDb db) =>
{
    if (await db.Employees.FindAsync(id) is Employee employee)
    {
        db.Employees.Remove(employee);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    return Results.NotFound();
});

RouteGroupBuilder departmentGroup = app.MapGroup("/departments");

departmentGroup.MapGet("/", getAllDepartments);
departmentGroup.MapGet("/{id}", getDepartment);
departmentGroup.MapPost("/", postDepartment );

static async Task<IResult> getAllDepartments(HCSCDb db) {
    return TypedResults.Ok(await db.Departments.ToArrayAsync());
}

static async Task<IResult> getDepartment(int id, HCSCDb db) {
    // return TypedResults.Ok(await db.Departments.FindAsync(id));

    return await db.Departments.FindAsync(id)
        is Department department ?
        TypedResults.Ok(department) :
        TypedResults.NotFound();
}

static async Task<IResult> postDepartment(Department department, HCSCDb db) {
    db.Departments.Add(department);
    await db.SaveChangesAsync();

    return TypedResults.Created($"/departments/{department.Id}");
}

RouteGroupBuilder reviewGroup = app.MapGroup("/reviews");

reviewGroup.MapGet("/", async (HCSCDb db) => {
    return await db.Reviews.Select(x => new ReviewDTO(x)).ToListAsync();
});
 
reviewGroup.MapGet("/incomplete", async (HCSCDb db) =>
    await db.Reviews.Where(review => review.IsComplete == false).Select(x => new ReviewDTO(x)).ToListAsync());

reviewGroup.MapPost("/", async (ReviewDTO reviewDTO, HCSCDb db) =>
{
    var review = new Review
    {
        IsComplete = reviewDTO.IsComplete,
        EmployeId = reviewDTO.EmployeId,
        Date = reviewDTO.Date,
        Id = reviewDTO.Id
    };

    db.Reviews.Add(review);
    await db.SaveChangesAsync();

    reviewDTO = new ReviewDTO(review);

    return TypedResults.Created($"/reviews/{review.Id}", reviewDTO);
});

app.Run();