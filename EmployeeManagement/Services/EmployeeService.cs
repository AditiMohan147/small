using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using EmployeeManagement.Models;

namespace EmployeeManagement.Services;

public class EmployeeService
{
    private readonly string _connectionString;

    public EmployeeService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new ArgumentNullException("Connection string is null");
    }

    private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

    public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
    {
        using var db = CreateConnection();
        return await db.QueryAsync<Employee>("SELECT * FROM Employees");
    }

    public async Task<Employee?> GetEmployeeByIdAsync(int id)
    {
        using var db = CreateConnection();
        return await db.QueryFirstOrDefaultAsync<Employee>(
            "SELECT * FROM Employees WHERE Eid = @Eid", new { Eid = id });
    }

    public async Task AddEmployeeAsync(Employee employee)
    {
        using var db = CreateConnection();
        var sql = @"
            INSERT INTO Employees (Name, Age, Position, Salary) 
            VALUES (@Name, @Age, @Position, @Salary)";
        await db.ExecuteAsync(sql, employee);
    }

    public async Task UpdateEmployeeAsync(Employee employee)
    {
        using var db = CreateConnection();
        var sql = @"
            UPDATE Employees 
            SET Name = @Name, Age = @Age, Position = @Position, Salary = @Salary 
            WHERE Eid = @Eid";
        await db.ExecuteAsync(sql, employee);
    }

    public async Task DeleteEmployeeAsync(int id)
    {
        using var db = CreateConnection();
        await db.ExecuteAsync("DELETE FROM Employees WHERE Eid = @Eid", new { Eid = id });
    }
}
