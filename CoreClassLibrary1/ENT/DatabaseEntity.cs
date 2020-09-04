using System;

namespace Core.ENT
{
    /// <summary>
    /// FullName： 
    /// </summary>
    [Serializable]
    public class DatabaseEntity : Sdk.BaseEntity
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
        private String _Server;
        /// <summary>
        /// 服务器地址
        /// </summary>
        public String Server
        {
            get { return _Server; }
            set { _Server = value; }
        }
        private String _User;
        /// <summary>
        /// 用户名
        /// </summary>
        public String User
        {
            get { return _User; }
            set { _User = value; }
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
        private int? _Port;
        /// <summary>
        /// 端口
        /// </summary>
        public int? Port
        {
            get { return _Port; }
            set { _Port = value; }
        }
        private String _DBName;
        /// <summary>
        /// 数据库名称
        /// </summary>
        public String DBName
        {
            get { return _DBName; }
            set { _DBName = value; }
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
        private int? _Status;
        /// <summary>
        /// 状态
        /// </summary>
        public int? Status
        {
            get { return _Status; }
            set { _Status = value; }
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
        public override string EntityFullName => "Core.Entity.DatabaseEntity";
    }
}