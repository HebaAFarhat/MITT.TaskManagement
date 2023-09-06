using System.Net;

namespace MITT.EmployeeDb.Models;

public class Errors
{
    public record ExceptionResponse(HttpStatusCode StatusCode, string Description);

}