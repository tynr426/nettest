using System;

namespace Core.ENT
{
    /// <summary>
    /// FullName： 
    /// </summary>
    [Serializable]
    public class AuthSiteEntity : Sdk.BaseEntity
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
        private int? _ConnectorId;
        /// <summary>
        /// 连接者id
        /// </summary>
        public int? ConnectorId
        {
            get { return _ConnectorId; }
            set { _ConnectorId = value; }
        }
        private string _PageIds;
        /// <summary>
        /// 装修页面id
        /// </summary>
        public string PageIds
        {
            get { return _PageIds; }
            set { _PageIds = value; }
        }

        private int? _PageType;
        /// <summary>
        /// 页面类型;pc或h5
        /// </summary>
        public int? PageType
        {
            get { return _PageType; }
            set { _PageType = value; }
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
        public override string EntityFullName => "Core.Entity.AuthSiteEntity";
    }
}