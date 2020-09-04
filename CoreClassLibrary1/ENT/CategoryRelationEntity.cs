using System;

namespace Core.ENT
{
    /// <summary>
    /// FullName： 
    /// </summary>
    [Serializable]
    public class CategoryRelationEntity : Sdk.BaseEntity
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
        private int? _Upconnectorid;
        /// <summary>
        /// 上游连接者id
        /// </summary>
        public int? Upconnectorid
        {
            get { return _Upconnectorid; }
            set { _Upconnectorid = value; }
        }
        private int? _Upcategoryid;
        /// <summary>
        /// 上游货品Id
        /// </summary>
        public int? Upcategoryid
        {
            get { return _Upcategoryid; }
            set { _Upcategoryid = value; }
        }
        private int? _Downconnectorid;
        /// <summary>
        /// 下游连接者Id
        /// </summary>
        public int? Downconnectorid
        {
            get { return _Downconnectorid; }
            set { _Downconnectorid = value; }
        }
        private int? _Downcategoryid;
        /// <summary>
        /// 下游货品Id
        /// </summary>
        public int? Downcategoryid
        {
            get { return _Downcategoryid; }
            set { _Downcategoryid = value; }
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
        public override string EntityFullName => "Core.Entity.CategoryRelationEntity";
    }
}