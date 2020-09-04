using ECF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdk
{
    /// <summary>
    /// FullName： <see cref="Vast.Sdk.Exceptions"/>
    /// 系统统一的异常处理
    /// Author:  XP
    /// Created: 2011/11/14
    /// </summary>
    public class Exceptions : ECFException
    {
        private string _LogPath = "\\SysLogs\\Wizard_" + DateTime.Now.ToString("yy-MM-dd") + "_Log.html";

        public override string Module
        {
            get
            {
                return "Vast";
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="DbException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public Exceptions(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Exceptions"/> class.
        /// </summary>
        /// <param name="ex">The ex.</param>
        public Exceptions(Exception ex)
            : base(ex)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Exceptions"/> class.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="module">The module.</param>
        public Exceptions(Exception ex, string module)
            : base(ex, module)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        public Exceptions(string message, Exception ex)
            : base(message, ex)
        {

        }

        public Exceptions(string message, string module)
            : base(message, module)
        {
        }

        public Exceptions(string message, Exception ex, string module)
            : base(message, ex, module)
        {
        }
    }


    /// <summary>
    /// 用户级异常处理
    /// Author：hfs
    /// Created：2017/06/27
    /// </summary>
    [Serializable]
    public class MyException : ApplicationException
    {
        public MyException() { }
        public MyException(string message) : base(message) { }
        public MyException(string message, Exception inner) : base(message, inner) { }
        public MyException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
