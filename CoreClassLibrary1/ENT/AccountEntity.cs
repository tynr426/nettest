using System;

namespace Core.ENT
{
    /// <summary>
    /// FullName： 
    /// </summary>
    [Serializable]
    public class AccountEntity : Sdk.BaseEntity
    {


        private int? _Id;
        /// <summary>
        /// 序号
        /// </summary>
        public int? Id
        {
            get { return _Id; }
            set { _Id = value; }
        }
        private String _UserName;
        /// <summary>
        /// 用户名
        /// </summary>
        public String UserName
        {
            get { return _UserName; }
            set { _UserName = value; }
        }
        private String _Password;
        /// <summary>
        /// 密码
        /// </summary>
        public String Password
        {
            get { return _Password; }
            set { _Password = value; }
        }
        private String _NickName;
        /// <summary>
        /// 昵称
        /// </summary>
        public String NickName
        {
            get { return _NickName; }
            set { _NickName = value; }
        }
        private int? _Errors;
        /// <summary>
        /// 错误次数
        /// </summary>
        public int? Errors
        {
            get { return _Errors; }
            set { _Errors = value; }
        }
        private DateTime? _ErrorTime;
        /// <summary>
        /// 错误时间
        /// </summary>
        public DateTime? ErrorTime
        {
            get { return _ErrorTime; }
            set { _ErrorTime = value; }
        }
        private String _LoginIp;
        /// <summary>
        /// 登录ip
        /// </summary>
        public String LoginIp
        {
            get { return _LoginIp; }
            set { _LoginIp = value; }
        }
        private int? _Status;
        /// <summary>
        /// 状态
        /// </summary>
        public int? Status
        {
            get { return _Status; }
            set { _Status = value; }
        }
        private DateTime? _LoginTime;
        /// <summary>
        /// 登录时间
        /// </summary>
        public DateTime? LoginTime
        {
            get { return _LoginTime; }
            set { _LoginTime = value; }
        }
        private DateTime? _AddTime;
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? AddTime
        {
            get { return _AddTime; }
            set { _AddTime = value; }
        }
        private DateTime? _UpdateTime;
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdateTime
        {
            get { return _UpdateTime; }
            set { _UpdateTime = value; }
        }
        private String _Remark;
        /// <summary>
        /// 备注
        /// </summary>
        public String Remark
        {
            get { return _Remark; }
            set { _Remark = value; }
        }
        public override string EntityFullName => "Core.Entity.AccountEntity";
    }
}