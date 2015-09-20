using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMTop.Common.Json
{
    /// <summary>
    ///Json 的摘要说明
    /// </summary>
    public class ReturnObj
    {
        public ReturnObj()
        {
            _Result = true;//默认成功
            _Msg = string.Empty;
        }

        private bool _Result;
        /// <summary>
        /// 结果
        /// </summary>
        public bool Result
        {
            get { return _Result; }
            set { _Result = value; }
        }

        private int _Flag;
        /// <summary>
        /// 标志
        /// </summary>
        public int Flag
        {
            get { return _Flag; }
            set { _Flag = value; }
        }

        private string _Msg;
        /// <summary>
        /// 消息
        /// </summary>
        public string Msg
        {
            get { return _Msg; }
            set { _Msg = value; }
        }
    }
}