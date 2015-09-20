using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Diagnostics;
using System.Data.Entity;

namespace EM.Data
{
    public sealed class AspNetObjectContextManager<T> : ObjectContextManager<T> where T : DbContext, new()
    {
        private object _lockObject;

        /// <summary>
        /// Returns a shared ObjectContext instance.
        /// </summary>
        public override T DbContext
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    T context = Activator.CreateInstance(typeof(T)) as T;
                    return context;
                }

                string ocKey = "lnocm_" + HttpContext.Current.GetHashCode().ToString("x");

                lock (_lockObject)
                {
                    if (!HttpContext.Current.Items.Contains(ocKey))
                    {
                        T context = Activator.CreateInstance(typeof(T)) as T;
                        HttpContext.Current.Items.Add(ocKey, context);
                        //Debug.WriteLine("AspNetObjectContextManager: Created new ObjectContext");
                    }
                }
                return HttpContext.Current.Items[ocKey] as T;
            }
        }

        public AspNetObjectContextManager()
        {
            //暂时改为通用，可供windows service程序调用
            //if (HttpContext.Current == null)
            //    throw new InvalidOperationException("An AspNetObjectContextManager can only be used in a HTTP context.");
            _lockObject = new object();
        }

    }
}
