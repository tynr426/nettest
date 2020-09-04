using System;

namespace Core.ENT
{
    /// <summary>
    /// FullName： 
    /// </summary>
    [Serializable]
    public class ProductRelationEntity : Sdk.BaseEntity
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
        private int? _UpProductId;
        /// <summary>
        /// 上游商品Id
        /// </summary>
        public int? UpProductId
        {
            get { return _UpProductId; }
            set { _UpProductId = value; }
        }
        private int? _UpGoodsId;
        /// <summary>
        /// 上游货品Id
        /// </summary>
        public int? UpGoodsId
        {
            get { return _UpGoodsId; }
            set { _UpGoodsId = value; }
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
        private int? _DownProductId;
        /// <summary>
        /// 下游商品id
        /// </summary>
        public int? DownProductId
        {
            get { return _DownProductId; }
            set { _DownProductId = value; }
        }
        private int? _DownGoodsId;
        /// <summary>
        /// 下游货品Id
        /// </summary>
        public int? DownGoodsId
        {
            get { return _DownGoodsId; }
            set { _DownGoodsId = value; }
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
        private DateTime? _Addtime;
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? Addtime
        {
            get { return _Addtime; }
            set { _Addtime = value; }
        }
        private DateTime? _Updatetime;
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? Updatetime
        {
            get { return _Updatetime; }
            set { _Updatetime = value; }
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
        public override string EntityFullName => "Core.Entity.ProductRelationEntity";
    }
}