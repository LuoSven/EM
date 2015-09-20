using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Topuc22Top.Model.Entities;

namespace Topuc22Top.Data
{
    /// <summary>
    /// ObjectContextHelper classes. 
    /// </summary>
    public class ObjectContextHelper
    {
        #region Private  Object Context Properties
        private static ObjectContextManager<TopucDB> TopUObjectContextManager
        {
            get;
            set;
        }

        #endregion

        #region Private Object Context Initial Methods

        private static void InstantiateTopUObjectContextManger()
        {
            TopUObjectContextManager = new AspNetObjectContextManager<TopucDB>();
        }

        #endregion

        #region Public Object Properties
        /// <summary>
        /// Gets an object context
        /// </summary>
        public static TopucDB TopUObjectContext
        {
            get
            {
                if (TopUObjectContextManager == null)
                    InstantiateTopUObjectContextManger();
                return TopUObjectContextManager.DbContext;
            }
        }
        #endregion
    }
}
