using Microsoft.EntityFrameworkCore;
using MITT.EmployeeDb;
using MITT.EmployeeDb.Models;
using MITT.Services.Abstracts;
using MITT.Services.Helpers;
using MITT.Services.Helpers.JwtHelper;

namespace MITT.Services;

public class AuthService : IAuthService
{
    private readonly ManagementDb _managementDb;
    private readonly JwtTokenBuilder _jwtTokenBuilder;

    public AuthService(ManagementDb managementDb, JwtTokenBuilder jwtTokenBuilder)
    {
        _managementDb = managementDb;
        _jwtTokenBuilder = jwtTokenBuilder;
    }

    public async Task<OperationResult<JwtToken>> SignIn(SignInDto signInDto, CancellationToken cancellationToken)
    {
        try
        {
            var identity = await _managementDb.GetIdentity(signInDto.Phone, cancellationToken);

            if (identity.identity.IsSigned)
                if (!signInDto.Pin.Verify(identity.identity.Pin)) throw new Exception("invalid_data_provided!!");

            var token = _jwtTokenBuilder
                .AddId(identity.identity.Id.ToString())
                .AddIdentity(identity.identity.Phone)
                .AddTag((int)identity.type)
                .Build();

            return OperationResult<JwtToken>.Valid(token);
        }
        catch (Exception e)
        {
            return OperationResult<JwtToken>.UnValid(messages: new string[] { e.Message });
        }
    }

    public async Task<OperationResult> ChangePin(ChangePinDto changePinDto, CancellationToken cancellationToken)
    {
        var developer = await _managementDb.Developers.SingleOrDefaultAsync(x => x.Phone == changePinDto.Phone, cancellationToken);

        if (developer is not null)
            return await ChangeDevPin(changePinDto, developer, cancellationToken);

        var manager = await _managementDb.Managers.SingleOrDefaultAsync(x => x.Phone == changePinDto.Phone && x.Pin == changePinDto.Pin.Hash(), cancellationToken);

        if (manager is not null)
            return await ChangeManagerPin(changePinDto, developer, manager, cancellationToken);

        return OperationResult.UnValid(messages: new string[] { "invalid_data_provided!!" });
    }

    public async Task<OperationResult> ResetPin(string phone, CancellationToken cancellationToken)
    {
        try
        {
            var identity = await _managementDb.GetIdentity(phone, cancellationToken);
            string @default = Hasher.GenerateDefaultPassword();

            if (identity.type == DeveloperType.Pm)
            {
                var manager = await _managementDb.GetManager(phone, cancellationToken);
                manager.IsSigned = false;
                manager.Pin = @default;
                _managementDb.Managers.Update(manager);
            }
            else
            {
                var developer = await _managementDb.GetDeveloper(phone, cancellationToken);
                developer.IsSigned = false;
                developer.Pin = @default;
                _managementDb.Developers.Update(developer);
            }
            await _managementDb.SaveChangesAsync(cancellationToken);
            return OperationResult.Valid();
        }
        catch (Exception e)
        {
            return OperationResult<JwtToken>.UnValid(messages: new string[] { e.Message });
        }
    }

    private async Task<OperationResult> ChangeManagerPin(ChangePinDto changePinDto, Developer developer, Manager manager, CancellationToken cancellationToken)
    {
        if (developer.IsSigned)
            if (!changePinDto.Pin.Verify(manager.Pin)) throw new Exception("invalid_data_provided!!");

        manager.Pin = changePinDto.NewPin.Hash();
        manager.IsSigned = true;
        _managementDb.Managers.Update(manager);
        await _managementDb.SaveChangesAsync(cancellationToken);
        return OperationResult.Valid();
    }

    private async Task<OperationResult> ChangeDevPin(ChangePinDto changePinDto, Developer developer, CancellationToken cancellationToken)
    {
        if (developer.IsSigned)
            if (!changePinDto.Pin.Verify(developer.Pin)) throw new Exception("invalid_data_provided!!");

        developer.Pin = changePinDto.NewPin.Hash();
        developer.IsSigned = true;

        _managementDb.Developers.Update(developer);
        await _managementDb.SaveChangesAsync(cancellationToken);
        return OperationResult.Valid();
    }
}

public static class Ext
{
    public static async Task<(Identity identity, DeveloperType type)> GetIdentity(this ManagementDb managementDb, string phone, CancellationToken cancellationToken)
    {
        var developer = await managementDb.GetDeveloper(phone, cancellationToken);
        if (developer is not null) return (developer, developer.Type);
        var manager = await managementDb.GetManager(phone, cancellationToken);
        if (manager is not null) return (manager, DeveloperType.Pm);

        throw new Exception("invalid_data_provided!!");
    }

    public static async Task<Manager> GetManager(this ManagementDb managementDb, string phone, CancellationToken cancellationToken)
        => await managementDb.Managers.SingleOrDefaultAsync(x => x.Phone == phone, cancellationToken);

    public static async Task<Developer> GetDeveloper(this ManagementDb managementDb, string phone, CancellationToken cancellationToken) =>
        await managementDb.Developers.SingleOrDefaultAsync(x => x.Phone == phone, cancellationToken);
}