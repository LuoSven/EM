using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EM.Model.Entities;

namespace EM.Data.Infrastructure
{
    public interface IDatabaseFactory : IDisposable
    {
        EMDB Get();
    }
}
