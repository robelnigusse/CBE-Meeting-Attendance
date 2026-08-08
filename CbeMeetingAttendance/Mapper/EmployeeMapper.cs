using CbeMeetingAttendance.DTOs.Employee;
using CbeMeetingAttendance.Models;

namespace CbeMeetingAttendance.Mappers
{
    public static class EmployeeMapper
    {
        public static Employee ToEmployee(RegisterEmployeeDto dto)
        {
            return new Employee
            {
                EmployeeId = dto.EmployeeId,
                FullName = dto.FullName,
                Division = dto.Division,
                JobTitle = dto.JobTitle,
                Department = dto.Department,
                PhoneNumber = dto.PhoneNumber
            };
        }

        public static EmployeeResponseDto ToEmployeeResponseDto(Employee employee)
        {
            return new EmployeeResponseDto
            {
                Id = employee.Id,
                EmployeeId = employee.EmployeeId,
                FullName = employee.FullName,
                Division = employee.Division,
                JobTitle = employee.JobTitle,
                Department = employee.Department,
                PhoneNumber = employee.PhoneNumber,
                CreatedAt = employee.CreatedAt
            };
        }

        public static List<EmployeeResponseDto> ToEmployeeResponseDtoList(List<Employee> employees)
        {
            return employees
                .Select(ToEmployeeResponseDto)
                .ToList();
        }

        public static void UpdateEmployee(Employee employee, UpdateEmployeeDto dto)
        {
            employee.FullName = dto.FullName;
            employee.Division = dto.Division;
            employee.JobTitle = dto.JobTitle;
            employee.Department = dto.Department;
            employee.PhoneNumber = dto.PhoneNumber;
        }
    }
}