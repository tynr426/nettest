using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdk
{
    /// <summary>
    /// FullName： <see cref="Vast.Sdk.Constant"/>
    /// Summary ： 系统常量 
    /// Version： 1.0.0.0 
    /// DateTime： 2012/5/17 20:11 
    /// CopyRight (c) by shaipe
    /// </summary>
    public class Constant
    {
        private static string _staticVersion = "";
        /// <summary>
        /// 系统静态文件版本
        /// </summary>
        public static string StaticVersion
        {
            get
            {
                if (string.IsNullOrEmpty(_staticVersion))
                {
                    object _version = ConfigurationManager.AppSettings["StaticVersion"];
                    if (_version == null)
                    {
                        _staticVersion = "3.1.9.19491";
                    }
                    else
                    {
                        _staticVersion = _version.ToString();
                    }
                }

                return _staticVersion;
            }
        }

        public const string CurrentVersion = "3.2.1";

        #region 系统常量

        /// <summary>
        /// 软件代号
        /// </summary>
        public const string SoftwareMark = "Vast";

        /// <summary>
        /// Mandatory password changes 强制密码修改时间,默认为90天必须修改一次密码
        /// </summary>
        public const int MPCDays = 90;

        /// <summary>
        /// 数据库链接配置缓存名前缀
        /// </summary>
        public const string DBKeyCachePrefix = "DBKey";
        /// <summary>
        /// 系统缓存前缀
        /// </summary>
        public const string CachePrefix = "Cache";
        /// <summary>
        /// 系统设置常量
        /// </summary>
        public const string SETTING_CACH_NAME = "Setting_Cache";

        /// <summary>
        /// 系统许可授权常量
        /// </summary>
        public const string License_Cache_Name = "License_Cache";

        /// <summary>
        /// 邮件服务器的配置信息
        /// </summary>
        public const string MailServerConfig = "Mail_Server_Config";

        /// <summary>
        /// 第三方集成登录Session名称
        /// </summary>
        public const string IntegratedInfo = "IntegratedInfo";


        /// <summary>
        /// 通用配置，未分组的配置
        /// </summary>
        public const string GeneralSetting = "GENERAL_SETTING";

        /// <summary>
        /// 系统配置前缀
        /// </summary>
        public const string ConfigPrefix = "CONST_SETTING_";

        /// <summary>
        /// 系统配置部分
        /// </summary>
        public const string SystemSetting = "Site";

        /// <summary>
        /// 第三方登录类型,txj 添加
        /// </summary>
        public const string UserLoginType = "UserLoginType";

        /// <summary>
        /// 凭据Key
        /// </summary>
        public const string TokenKey = "PLUS";

        /// <summary>
        /// 登录票据有效分钟数（默认十天）
        /// </summary>
        public const int LoginTicket_Minutes = 1 * 24 * 60 * 30;

        #endregion

    }
}
