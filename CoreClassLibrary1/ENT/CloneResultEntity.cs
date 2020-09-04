using System;

namespace Core.ENT
{
    /// <summary>
    /// FullName： 
    /// </summary>
    [Serializable]
    public class CloneResultEntity : Sdk.BaseEntity
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
        /// <summary>
        /// 上游连接器id
        /// </summary>
        public int? UpConnectorId { get; set; }
        /// <summary>
        /// 下游连接器id
        /// </summary>
        public int? DownConnectorId { get; set; }
        /// <summary>
        /// 上游卖家id
        /// </summary>
        public int? UpSellerId { get; set; }
        /// <summary>
        /// 下游卖家id
        /// </summary>
        public int? DownSellerId { get; set; }
        /// <summary>
        /// 克隆结果
        /// </summary>
        public int? CloneResult { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int? Status { get; set; }
        public override string EntityFullName => "Core.Entity.CloneResultEntity";
    }
}