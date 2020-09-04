using System;

namespace Core.ENT
{
    /// <summary>
    /// FullName： 
    /// </summary>
    [Serializable]
    public class OrderRelationEntity : Sdk.BaseEntity
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
        /// 上游连接者id
        /// </summary>
        public int? UpConnectorId
        {
            get { return _UpConnectorId; }
            set { _UpConnectorId = value; }
        }
        private int? _UpOrderId;
        /// <summary>
        /// 上游商品Id
        /// </summary>
        public int? UpOrderId
        {
            get { return _UpOrderId; }
            set { _UpOrderId = value; }
        }
        public string UpOddNumber { get; set; }
        private int? _DownConnectorId;
        /// <summary>
        /// 下游连接者Id
        /// </summary>
        public int? DownConnectorId
        {
            get { return _DownConnectorId; }
            set { _DownConnectorId = value; }
        }
        private int? _DownOrderId;
        /// <summary>
        /// 下游商品id
        /// </summary>
        public int? DownOrderId
        {
            get { return _DownOrderId; }
            set { _DownOrderId = value; }
        }
        public string DownOddNumber { get; set; }
       
        private DateTime? _Addtime;
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? Addtime
        {
            get { return _Addtime; }
            set { _Addtime = value; }
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
        public override string EntityFullName => "Core.Entity.OrderRelationEntity";
    }
}