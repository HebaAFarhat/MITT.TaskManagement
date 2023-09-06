namespace MITT.API;

public static class ApplicationBuilderExceptions
{
    public static IApplicationBuilder AddGlobalErrorHandler(this IApplicationBuilder applicationBuilder)
        => applicationBuilder.UseMiddleware<ExceptonMiddlewere>();
}