using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EM.Model.Entities;

namespace EM.Data
{
    /// <summary>
    /// ObjectContextHelper classes. 
    /// </summary>
    public class ObjectContextHelper
    {
        #region Private  Object Context Properties
        private static ObjectContextManager<EMDB> TopUObjectContextManager
        {
            get;
            set;
        }

        #endregion

        #region Private Object Context Initial Methods

        private static void InstantiateTopUObjectContextManger()
        {
            TopUObjectContextManager = new AspNetObjectContextManager<EMDB>();
        }

        #endregion

        #region Public Object Properties
        /// <summary>
        /// Gets an object context
        /// </summary>
        public static EMDB TopUObjectContext
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
