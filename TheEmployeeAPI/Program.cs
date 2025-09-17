using System.ComponentModel.DataAnnotations;
using TheEmployeeAPI;
using TheEmployeeAPI.Abstractions;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IRepository<Employee>, EmployeeRepository>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Define the Employee API endpoints
var employeeRoute = app.MapGroup("/employees");

// Get all employees
employeeRoute.MapGet(string.Empty, (IRepository<Employee> repository) => {
    return Results.Ok(repository.GetAll().Select(employee => new GetEmployeeResponse {
        FirstName = employee.FirstName,
        LastName = employee.LastName,
        Address1 = employee.Address1,
        Address2 = employee.Address2,
        City = employee.City,
        State = employee.State,
        ZipCode = employee.ZipCode,
        PhoneNumber = employee.PhoneNumber,
        Email = employee.Email
    }));
});

// Get employee by ID
employeeRoute.MapGet("{id:int}", (int id, IRepository<Employee> repository) => {
    var employee = repository.GetById(id);
    if (employee == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(new GetEmployeeResponse {
        FirstName = employee.FirstName,
        LastName = employee.LastName,
        Address1 = employee.Address1,
        Address2 = employee.Address2,
        City = employee.City,
        State = employee.State,
        ZipCode = employee.ZipCode,
        PhoneNumber = employee.PhoneNumber,
        Email = employee.Email
    });
});

// Create a new employee
employeeRoute.MapPost(string.Empty, (CreateEmployeeRequest employee, IRepository<Employee> repository) => {
    // Validate the incoming request
    var validationProblems = new List<ValidationResult>();
    var isValid = Validator.TryValidateObject(employee, new ValidationContext(employee), validationProblems, true);
    if (!isValid) // Return 400 Bad Request if validation fails
    {
        return Results.BadRequest(validationProblems.ToValidationProblemDetails());
    }
    var newEmployee = new Employee {
        FirstName = employee.FirstName!,
        LastName = employee.LastName!,
        SocialSecurityNumber = employee.SocialSecurityNumber,
        Address1 = employee.Address1,
        Address2 = employee.Address2,
        City = employee.City,
        State = employee.State,
        ZipCode = employee.ZipCode,
        PhoneNumber = employee.PhoneNumber,
        Email = employee.Email
    };
    repository.Create(newEmployee);
    return Results.Created($"/employees/{newEmployee.Id}", employee);
});

// Update an existing employee
employeeRoute.MapPut("{id}", (UpdateEmployeeRequest employee, int id, IRepository<Employee> repository) => {
    var existingEmployee = repository.GetById(id);
    if (existingEmployee == null)
    {
        return Results.NotFound();
    }

    existingEmployee.Address1 = employee.Address1;
    existingEmployee.Address2 = employee.Address2;
    existingEmployee.City = employee.City;
    existingEmployee.State =    employee.State;
    existingEmployee.ZipCode = employee.ZipCode;
    existingEmployee.PhoneNumber = employee.PhoneNumber;
    existingEmployee.Email = employee.Email;

    repository.Update(existingEmployee);
    return Results.Ok(existingEmployee);
});

app.UseHttpsRedirection();

app.Run();

public partial class Program { } // Make the Program class public for testing purposes