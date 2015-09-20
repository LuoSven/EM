using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Topuc22Top.Model.Entities;

namespace Topuc22Top.Data.Infrastructure
{
    public interface IDatabaseFactory : IDisposable
    {
        TopucDB Get();
    }
}
