using DataAccess.Concrete;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Business;

public class LogService
{
    private readonly ESLogDal eSLogDal;

    public LogService(ESLogDal eSLogDal)
    {
        this.eSLogDal = eSLogDal;
    }

    public async Task<bool> SaveAsync(LogModel model)
    {
        var isCreated = await eSLogDal.SaveAsync(model);

        return isCreated != null;
    }
    public async Task<ImmutableList<LogModel>> GetAllLogAsync()
    {
        return await eSLogDal.GetAllAsync();
    }
}
