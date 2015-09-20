using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EM.Model.Entities;

namespace EM.Data.Infrastructure
{
    public class DatabaseFactory : Disposable, IDatabaseFactory
    {
        private EMDB dataContext;
        public EMDB Get()
        {
            return dataContext ?? (dataContext = new EMDB());
        }
        protected override void DisposeCore()
        {
            if (dataContext != null)
                dataContext.Dispose();
        }
    }
}
