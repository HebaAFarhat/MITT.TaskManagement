using Microsoft.EntityFrameworkCore;
using MITT.EmployeeDb.Models;
using System.Linq.Expressions;

namespace MITT.Services.Helpers;

public class ManagementService<R> where R : BaseEntity
{
    public readonly DbContext dbContext;

    protected ManagementService(DbContext dataContext) => dbContext = dataContext;

    public async Task Add(R request, CancellationToken cancellationToken)
    {
        await dbContext.Set<R>().AddAsync(request, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Update(R request, CancellationToken cancellationToken)
    {
        dbContext.Set<R>().Update(request);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<R>> List(Expression<Func<R, bool>> predicate = null, CancellationToken cancellationToken = default) =>
        predicate != null
            ? await dbContext.Set<R>().Where(predicate).ToListAsync(cancellationToken)
            : await dbContext.Set<R>().ToListAsync(cancellationToken);

    public async Task<R> Get(Expression<Func<R, bool>> predicate = null, CancellationToken cancellationToken = default)
        => await dbContext.Set<R>().FirstOrDefaultAsync(predicate, cancellationToken);

    public async Task<R> Get(Guid id, CancellationToken cancellationToken = default)
        => await dbContext.Set<R>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task Save(CancellationToken cancellationToken = default) => await dbContext.SaveChangesAsync(cancellationToken);
}