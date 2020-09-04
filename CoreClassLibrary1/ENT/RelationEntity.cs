using System;

namespace Core.ENT
{
    /// <summary>
    /// FullName： 
    /// </summary>
    [Serializable]
    public class RelationEntity : Sdk.BaseEntity
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
        private int? _UpConnectorId;
        /// <summary>
        /// 上游连接者Id
        /// </summary>
        public int? UpConnectorId
        {
            get { return _UpConnectorId; }
            set { _UpConnectorId = value; }
        }
        private int? _UpBuyerId;
        /// <summary>
        /// 上游买家id
        /// </summary>
        public int? UpBuyerId
        {
            get { return _UpBuyerId; }
            set { _UpBuyerId = value; }
        }
        private int? _DownConnectorId;
        /// <summary>
        /// 下游连接者Id
        /// </summary>
        public int? DownConnectorId
        {
            get { return _DownConnectorId; }
            set { _DownConnectorId = value; }
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
        private int? _IsDefault;
        /// <summary>
        /// 状态
        /// </summary>
        public int? IsDefault
        {
            get { return _IsDefault; }
            set { _IsDefault = value; }
        }

        private int? _IsOpen;
        /// <summary>
        /// 开放克隆
        /// </summary>
        public int? IsOpen
        {
            get { return _IsOpen; }
            set { _IsOpen = value; }
        }
        private int? _InvitedFrom;
        /// <summary>
        /// 发起邀请方
        /// </summary>
        public int? InvitedFrom
        {
            get { return _InvitedFrom; }
            set { _InvitedFrom = value; }
        }
        public override string EntityFullName => "Core.Entity.RelationEntity";
    }
}