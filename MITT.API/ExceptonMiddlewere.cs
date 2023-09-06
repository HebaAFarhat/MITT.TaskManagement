using System.Net;
using MITT.EmployeeDb.Models;
using MITT.Services.Abstracts;

namespace MITT.API;

public class ExceptonMiddlewere
{
    private readonly RequestDelegate _next;

    public ExceptonMiddlewere(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception e)
        {
            var response = e switch
            {
                ApplicationException _ => new OperationResult(OperationResult.ResultType.TechError,
                    new List<string>{"Application exception occurred."}),
                KeyNotFoundException _ => new OperationResult(OperationResult.ResultType.TechError,
                    new List<string>{"The request key not found."}),
                UnauthorizedAccessException _ => new OperationResult(OperationResult.ResultType.Unauthorized,
                    new List<string>{"Unauthorized."}),
                _ => new OperationResult(OperationResult.ResultType.TechError,
                    new List<string>{"Internal server error. Please retry later."})
            };
            context.Response.ContentType = "application/json";

            var result = response.Type switch
            {
                OperationResult.ResultType.Success => context.Response.StatusCode = (int)HttpStatusCode.OK,
                OperationResult.ResultType.TechError => context.Response.StatusCode = (int)HttpStatusCode.BadRequest,
                _ => context.Response.StatusCode = (int)HttpStatusCode.Unauthorized,
            };
                
            await context.Response.WriteAsJsonAsync(response);
        }
    }
}