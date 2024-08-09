using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniProject3_CompanySystemWebAPIPostgreSQL.Interfaces;
using MiniProject3_CompanySystemWebAPIPostgreSQL.Models;
using MiniProject3_CompanySystemWebAPIPostgreSQL.Services;
using static MiniProject3_CompanySystemWebAPIPostgreSQL.Services.EmployeeService;

namespace MiniProject3_CompanySystemWebAPIPostgreSQL.Controllers
{
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[Controller]")]
    [ApiController]
    /// <summary>
    /// Handles operations related to employees, including adding, retrieving, updating, and deleting employees.
    /// </summary>
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeService _employeeService;

        /// <summary>
        /// Initializes the EmployeeController with the employee service.
        /// </summary>
        /// <param name="employeeService">The service used to manage employees.</param>
        public EmployeeController(EmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        /// <summary>
        /// Adds a new employee to the system.
        /// </summary>
        /// <remarks>
        /// Ensure that the employee data is not null and that both the first name and last name are provided.
        ///
        /// Sample request:
        ///
        ///     POST /Employee
        ///     {
        ///        "fname": "John",
        ///        "lname": "Doe",
        ///        "dob": "1990-01-01",
        ///        "departmentId": 1
        ///     }
        /// </remarks>
        /// <param name="employee">The employee to be added.</param>
        /// <returns>Success message if the employee is added successfully or an error message if validation fails.</returns>
        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<Employee>> AddEmployee([FromBody] Employee employee)
        {
            if (employee == null)
            {
                return BadRequest("Employee data is null.");
            }

            if (string.IsNullOrWhiteSpace(employee.Fname) || string.IsNullOrWhiteSpace(employee.Lname))
            {
                return BadRequest("Employee first name and last name are required.");
            }

            var success = await _employeeService.AddEmployee(employee);
            if (!success)
            {
                return BadRequest("Employee Number Already Exists.");
            }

            employee.ConvertDobObjectToDateOnly();
            await _employeeService.AddEmployee(employee);
            return Ok("Employee Data Successfully added.");
        }

        /// <summary>
        /// Retrieves a paginated list of all employees in the system.
        /// </summary>
        /// <remarks>
        /// Provide the page number and page size for pagination.
        ///
        /// Sample request:
        ///
        ///     GET /Employee/page/1/size/10
        /// </remarks>
        /// <param name="pageNumber">The page number for pagination.</param>
        /// <param name="pageSize">The number of employees to retrieve per page.</param>
        /// <returns>A paginated list of employees.</returns>
        [HttpGet("page/{pageNumber}/size/{pageSize}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetAllEmployees(int pageNumber, int pageSize)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest("Page number and page size must be greater than zero.");
            }
            var employees = await _employeeService.GetAllEmployees(pageNumber, pageSize);
            return Ok(employees);
        }

        /// <summary>
        /// Retrieves the details of an employee by their number.
        /// </summary>
        /// <remarks>
        /// Ensure that the employee number provided is valid.
        ///
        /// Sample request:
        ///
        ///     GET /Employee/1
        /// </remarks>
        /// <param name="empNo">The employee number of the employee to be retrieved.</param>
        /// <returns>Employee details if found or an error message if the employee is not found.</returns>
        [HttpGet("{empNo}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<Employee>> GetEmployeeById(int empNo)
        {
            if (empNo <= 0)
            {
                return BadRequest("Invalid employee number.");
            }
            var employee = await _employeeService.GetEmployeeById(empNo);
            if (employee == null)
            {
                return NotFound();
            }
            return Ok(employee);
        }

        /// <summary>
        /// Updates the details of an employee by their number.
        /// </summary>
        /// <remarks>
        /// Ensure that the employee number is valid and that the employee data is provided.
        ///
        /// Sample request:
        ///
        ///     PUT /Employee/1
        ///     {
        ///        "fname": "Jane",
        ///        "lname": "Doe",
        ///        "dob": "1992-02-02",
        ///        "departmentId": 2
        ///     }
        /// </remarks>
        /// <param name="empNo">The employee number of the employee to be updated.</param>
        /// <param name="editEmp">The updated employee data.</param>
        /// <returns>Success message if the employee is updated successfully or an error message if the employee is not found.</returns>
        [HttpPut("{empNo}")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> UpdateEmployee(int empNo, [FromBody] Employee editEmp)
        {
            if (empNo <= 0 || editEmp == null)
            {
                return BadRequest("Invalid employee number or data.");
            }

            var success = await _employeeService.UpdateEmployee(empNo, editEmp);
            if (!success)
            {
                return NotFound("Employee not found.");
            }
            return Ok("Employee Data Successfully Updated.");
        }

        /// <summary>
        /// Deletes an employee by their number.
        /// </summary>
        /// <remarks>
        /// Ensure that the employee number provided is valid.
        ///
        /// Sample request:
        ///
        ///     DELETE /Employee/1
        /// </remarks>
        /// <param name="empNo">The employee number of the employee to be deleted.</param>
        /// <returns>Success message if the employee is deleted successfully or an error message if the employee is not found.</returns>
        [HttpDelete("{empNo}")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> DeleteEmployee(int empNo)
        {
            if (empNo <= 0)
            {
                return BadRequest("Invalid employee number.");
            }

            var success = await _employeeService.DeleteEmployee(empNo);
            if (!success)
            {
                return NotFound("Employee not found.");
            }
            return Ok("Employee Data Successfully Deleted.");
        }

        /// <summary>
        /// Retrieves a list of employees from BRICS countries.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /Employee/from-brics
        /// </remarks>
        /// <returns>A list of employees from BRICS countries.</returns>
        [HttpGet("from-brics")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployeesFromBRICS()
        {
            var employees = await _employeeService.GetEmployeesFromBRICS();
            return Ok(employees);
        }

        /// <summary>
        /// Retrieves a list of employees born between 1980 and 1990.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /Employee/born-between-1980-1990
        /// </remarks>
        /// <returns>A list of employees born between 1980 and 1990.</returns>
        [HttpGet("born-between-1980-1990")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployeesBornBetween1980And1990()
        {
            var employees = await _employeeService.GetEmployeesBornBetween1980And1990();
            return Ok(employees);
        }

        /// <summary>
        /// Retrieves a list of female employees born after 1990.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /Employee/female-born-after-1990
        /// </remarks>
        /// <returns>A list of female employees born after 1990.</returns>
        [HttpGet("female-born-after-1990")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetFemaleEmployeesBornAfter1990()
        {
            var employees = await _employeeService.GetFemaleEmployeesBornAfter1990();
            return Ok(employees);
        }

        /// <summary>
        /// Retrieves a list of female managers.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /Employee/female-managers
        /// </remarks>
        /// <returns>A list of female managers.</returns>
        [HttpGet("female-managers")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetFemaleManagers()
        {
            var managers = await _employeeService.GetFemaleManagers();
            return Ok(managers);
        }

        /// <summary>
        /// Retrieves a list of employees who are not managers.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /Employee/not-managers
        /// </remarks>
        /// <returns>A list of employees who are not managers.</returns>
        [HttpGet("not-managers")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployeesManagers()
        {
            var employees = await _employeeService.GetEmployeesNotManagers();
            return Ok(employees);
        }

        /// <summary>
        /// Retrieves the number of female managers.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /Employee/female-managers-count
        /// </remarks>
        /// <returns>The number of female managers.</returns>
        [HttpGet("female-managers-count")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<int>> GetNumberOfFemaleManagers()
        {
            var count = await _employeeService.GetNumberOfFemaleManagers();
            return Ok(count);
        }

        /// <summary>
        /// Retrieves a list of departments with more than ten employees.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /Employee/departments-with-more-than-ten-employees
        /// </remarks>
        /// <returns>A list of departments with more than ten employees and their counts.</returns>
        [HttpGet("departments-with-more-than-ten-employees")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<IEnumerable<DepartmentEmployeeCount>>> GetDepartmentsWithMoreThanTenEmployees()
        {
            var result = await _employeeService.GetDepartmentsWithMoreThanTenEmployees();
            return Ok(result);
        }

    }


}
