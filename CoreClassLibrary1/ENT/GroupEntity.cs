using System;

namespace Core.ENT
{
    /// <summary>
    /// FullName： 
    /// </summary>
    [Serializable]
    public class GroupEntity : Sdk.BaseEntity
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
        private String _Domain;
        /// <summary>
        /// 主域名
        /// </summary>
        public String Domain
        {
            get { return _Domain; }
            set { _Domain = value; }
        }
        private String _ImageDomain;
        /// <summary>
        /// 图片域名
        /// </summary>
        public String ImageDomain
        {
            get { return _ImageDomain; }
            set { _ImageDomain = value; }
        }
        private int? _DBId;
        /// <summary>
        /// 数据库id
        /// </summary>
        public int? DBId
        {
            get { return _DBId; }
            set { _DBId = value; }
        }
        private int? _Weight;
        /// <summary>
        /// 权重
        /// </summary>
        public int? Weight
        {
            get { return _Weight; }
            set { _Weight = value; }
        }
        private String _Province;
        /// <summary>
        /// 省
        /// </summary>
        public String Province
        {
            get { return _Province; }
            set { _Province = value; }
        }
        private String _City;
        /// <summary>
        /// 市
        /// </summary>
        public String City
        {
            get { return _City; }
            set { _City = value; }
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
        private String _Remark;
        /// <summary>
        /// 备注
        /// </summary>
        public String Remark
        {
            get { return _Remark; }
            set { _Remark = value; }
        }
        public override string EntityFullName => "Core.Entity.GroupEntity";
    }
}