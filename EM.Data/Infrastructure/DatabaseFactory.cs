using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Topuc22Top.Model.Entities;

namespace Topuc22Top.Data.Infrastructure
{
    public class DatabaseFactory : Disposable, IDatabaseFactory
    {
        private TopucDB dataContext;
        public TopucDB Get()
        {
            return dataContext ?? (dataContext = new TopucDB());
        }
        protected override void DisposeCore()
        {
            if (dataContext != null)
                dataContext.Dispose();
        }
    }
}
