using System;

namespace Core.ENT
{
    /// <summary>
    /// FullName： 
    /// </summary>
    [Serializable]
    public class DispatchRelationEntity : Sdk.BaseEntity
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
        public int? OrderRelationId { get; set; }
        public int? UpDispatchId { get; set; }
        
        public int? DownDispatchId { get; set; }
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
        public override string EntityFullName => "Core.Entity.DispatchRelationEntity";
    }
}