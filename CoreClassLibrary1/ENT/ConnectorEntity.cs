using ECF;
using System;

namespace Core.ENT
{
    /// <summary>
    /// FullName： 
    /// </summary>
    [Serializable]
    [EntityClass(ValidateField = true)]
    public class ConnectorEntity : Sdk.BaseEntity
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
        private String _Name;
        /// <summary>
        /// 名称
        /// </summary>
        public String Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        private int? _SellerId;
        /// <summary>
        /// 卖家id
        /// </summary>
        public int? SellerId
        {
            get { return _SellerId; }
            set { _SellerId = value; }
        }
        private String _Logo;
        /// <summary>
        /// logo
        /// </summary>
        public String Logo
        {
            get { return _Logo; }
            set { _Logo = value; }
        }

        private String _VirtualDir;
        /// <summary>
        /// VirtualDir
        /// </summary>
        public String VirtualDir
        {
            get { return _VirtualDir; }
            set { _VirtualDir = value; }
        }
        private String _Mobile;
        /// <summary>
        /// 手机号
        /// </summary>
        public String Mobile
        {
            get { return _Mobile; }
            set { _Mobile = value; }
        }
        private int? _GroupId;
        /// <summary>
        /// 机组id
        /// </summary>
        public int? GroupId
        {
            get { return _GroupId; }
            set { _GroupId = value; }
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
        /// <summary>
        /// 上游
        /// </summary>
        [EntityProperty(true)]
        public int? UpConnectorId
        {
            get;set;
        }

        /// <summary>
        /// 域名
        /// </summary>
        [EntityProperty(true)]
        public String Domain
        {
            get;set;
        }
        /// <summary>
        /// dbid
        /// </summary>
        [EntityProperty(true)]
        public int? DBId
        {
            get; set;
        }
        public override string EntityFullName => "Core.Entity.ConnectorEntity";
    }
    public class ConnectorRelation : ConnectorEntity
    {
        public int? UpBuyerId
        {
            get; set;
        }
        public int? ThirdConnectorId
        {
            get;set;
        }
    }
}