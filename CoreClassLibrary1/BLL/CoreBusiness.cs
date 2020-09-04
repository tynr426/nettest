using Sdk.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.BLL
{
    /// <summary>
    /// 供应商中心基础业务逻辑.
    /// </summary>
    public abstract class CoreBusiness : BaseBusiness
    {
        #region override

        /// <summary>
        /// 常量化数据表前缀
        /// </summary>
        internal const string _TablePrefix = "ict_";

        /// <summary>
        /// 当前项目的数据库链接关键字
        /// </summary>
        public override string DBKey
        {
            get { return "connector"; }
        }

        /// <summary>
        /// 当前项目的数据表前缀
        /// </summary>
        public override string TablePrefix
        {
            get { return _TablePrefix; }
        }

        /// <summary>
        /// 数据库映射关键字
        /// </summary>
        public override string MapKey
        {
            get { return "Config"; }
        }

        /// <summary>
        /// 设置模块名称
        /// </summary>
        public override string ModuleName
        {
            get { return "供应商中心 》 "; }
        }

        /// <summary>
        /// 业务逻辑缓存关键字
        /// </summary>
        public override string CacheKey
        {
            get { return "Connector" + DBKey + TablePrefix + TableName; }
        }
        #endregion


    }
}
