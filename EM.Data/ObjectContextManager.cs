using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace EM.Data
{
    public abstract class ObjectContextManager<T> where T : DbContext, new()
    {
        /// <summary>
        /// Returns a reference to an ObjectContext instance.
        /// </summary>
        public abstract T DbContext
        {
            get;
        }
    }
}
