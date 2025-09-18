using Microsoft.AspNetCore.Mvc;
using TheEmployeeAPI.Abstractions;

namespace TheEmployeeAPI.Employees;

[ApiController]
[Route("api/employees")]
public class EmployeesController : BaseController
{
    private readonly IRepository<Employee> _repository;
    private readonly ILogger<EmployeesController> _logger;
    public EmployeesController(IRepository<Employee> repository, ILogger<EmployeesController> logger)
    {
        _repository = repository;
        _logger = logger;
        
    }

    [HttpGet]
    public IActionResult GetAllEmployees()
    {
        var employees = _repository.GetAll().Select(employee => new GetEmployeeResponse
        {
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

        return Ok(employees);
    }

    [HttpGet("{id:int}")]
    public IActionResult GetEmployeeById(int id)
    {
        var employee = _repository.GetById(id);
        if (employee == null)
        {
            return NotFound();
        }

        var employeeResponse = new GetEmployeeResponse
        {
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Address1 = employee.Address1,
            Address2 = employee.Address2,
            City = employee.City,
            State = employee.State,
            ZipCode = employee.ZipCode,
            PhoneNumber = employee.PhoneNumber,
            Email = employee.Email
        };

        return Ok(employeeResponse);
    }

    [HttpPost]
    public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeRequest employeeRequest)
    {
        var validationResults = await ValidateAsync(employeeRequest);
        if (!validationResults.IsValid)
        {
            return ValidationProblem(validationResults.ToModelStateDictionary());
        }

        var newEmployee = new Employee
        {
            FirstName = employeeRequest.FirstName!,
            LastName = employeeRequest.LastName!,
            SocialSecurityNumber = employeeRequest.SocialSecurityNumber,
            Address1 = employeeRequest.Address1,
            Address2 = employeeRequest.Address2,
            City = employeeRequest.City,
            State = employeeRequest.State,
            ZipCode = employeeRequest.ZipCode,
            PhoneNumber = employeeRequest.PhoneNumber,
            Email = employeeRequest.Email
        };

        _repository.Create(newEmployee);
        return CreatedAtAction(nameof(GetEmployeeById), new { id = newEmployee.Id }, newEmployee);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateEmployee(int id, [FromBody] UpdateEmployeeRequest employeeRequest)
    {
        _logger.LogInformation($"Updating employee with id {id}");
        var existingEmployee = _repository.GetById(id);
        if (existingEmployee == null)
        {
            _logger.LogWarning($"Employee with id {id} not found");
            return NotFound();
        }

        _logger.LogDebug($"Updating employee details for id {id}");
        existingEmployee.Address1 = employeeRequest.Address1;
        existingEmployee.Address2 = employeeRequest.Address2;
        existingEmployee.City = employeeRequest.City;
        existingEmployee.State = employeeRequest.State;
        existingEmployee.ZipCode = employeeRequest.ZipCode;
        existingEmployee.PhoneNumber = employeeRequest.PhoneNumber;
        existingEmployee.Email = employeeRequest.Email;

        try
        {
            _repository.Update(existingEmployee);
            _logger.LogInformation($"Employee with id {id} was updated");
            return Ok(existingEmployee);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error updating employee with id {id}");
            return StatusCode(500, "An error occurred while updating the employee.");
        }

        
    }
}