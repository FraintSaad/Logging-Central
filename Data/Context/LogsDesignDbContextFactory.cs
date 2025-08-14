using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Data.Context
{
    internal class LogsDesignDbContextFactory : IDesignTimeDbContextFactory<LogsDbContext>
    {
        public LogsDbContext CreateDbContext(string[] args)
        {
            throw new NotImplementedException();
        }
    }
}
