using Core.ENT;
using ECF;
using ECF.Data.Query;
using Sdk;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Core.BLL
{
    class GroupBusiness : CoreBusiness
    {
        #region override Property

        /// <summary>
        /// 常量数据表名
        /// </summary>
        public const string _TableName = _TablePrefix + "Group";

        /// <summary>
        /// 获取数据库表
        /// </summary>
        public override string TableName
        {
            get
            {
                return _TableName;
            }
        }

        /// <summary>
        /// 后期通过继承类接口而实现的数据实体接口
        /// </summary>
        public override ECF.IEntity Entity
        {
            get
            {
                return new GroupEntity();
            }
        }
        #endregion

        #region 插入机组信息 +override int Insert(System.Xml.XmlDocument xml)
        /// <summary>
        /// 插入机组信息
        /// </summary>
        /// <param name="xml">XML格式的品牌数据.</param>
        /// <returns>大于0成功，否则失败</returns>
        /// <remarks>
        /// <list type="bullet">
        /// </list>
        /// </remarks>
        public int Insert(Dictionary<string, object> datajson)
        {
            ClearCache();
            int val = 0;
            GroupEntity ent = new GroupEntity();
            ent.SetValues(datajson);

            //检查信息是否存在
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("Name", ent.Name);
            dic.Add("Domain", ent.Domain);
            dic.Add("DBId", ent.DBId);
            GroupEntity entAccount = DbAccess.GetEntity<GroupEntity>(TableName, dic);
            if (entAccount != null)
            {
                return -1;
            }
            ent.AddTime = DateTime.Now;
            val = DbAccess.ExecuteInsert(TableName, ent);
            return val;
        }

        #endregion

        #region 标记删除机组 +override int LogicDelete(int id)
        /// <summary>
        /// 获取机组信息
        /// </summary>
        /// <param name="id">授权id.</param>
        /// <returns>list</returns>
        /// <remarks>
        /// <list type="bullet">
        /// </list>
        /// </remarks>
        public override int LogicDelete(int id)
        {
            //检查是否有商品在使用
            int count = Utils.ToInt(DbAccess.GetValue(TableName, "count(0)", "Id=" + id));
            if (count > 0)
            {
                return -1;
            }

            return base.LogicDelete(id);
        }

        #endregion

        #region 删除机组记录 +override int Delete(int id)
        /// <summary>
        /// 删除机组记录
        /// </summary>
        /// <param name="id">授权ID</param>
        /// <returns>大于0成功，否则失败</returns>
        /// <remarks>
        /// <list type="bullet">
        /// <item></item>
        /// </list>
        /// </remarks>
        public override int Delete(int id)
        {
            return base.Delete(id);
        }
        #endregion

        #region 通过ID获取机组信息
        /// <summary>
        /// 通过卖家id获取机组信息
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>大于0成功，否则失败</returns>
        /// <remarks>
        /// <list type="bullet">
        /// <item></item>
        /// </list>
        /// </remarks>
        public DataTable GetGroupById(int id)
        {
            string sql = string.Format("Id={0}", id);
            DataTable dt = DBTable(sql);
            return dt;
        }
        #endregion

        #region 通过条件获取机组信息
        /// <summary>
        /// 通过条件获取机组信息
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item></item>
        /// </list>
        /// </remarks>
        public PagingResult GetGroupList(PagingQuery pagingQuery)
        {
            PagingResult result = new PagingResult();
            result.PageIndex = pagingQuery.PageIndex;
            result.PageSize = pagingQuery.PageSize;
            string condition = "1=1";
            //遍历所有子节点
            foreach (Condition c in pagingQuery.Condition)
            {
                if (!string.IsNullOrEmpty(c.Value))
                {
                    switch (c.Name.ToLower())
                    {
                        case "keyword":
                            condition += string.Format(" and Name like '%{0}%'", c.Value);
                            break;
                        case "name":
                            condition += c.ToWhereString("Name");
                            break;
                        case "domain":
                            condition += c.ToWhereString("Domain");
                            break;
                        case "dbid":
                            condition += c.ToWhereString("DBId");
                            break;
                        case "status":
                            condition += c.ToWhereString("Status");
                            break;
                    }
                }
            }

            string sql = string.Format("select count(0) from {0} where {1} ;select * from {0}  where {1} order by AddTime desc limit {2},{3};", TableName,
                                        condition, pagingQuery.PageSize * (pagingQuery.PageIndex - 1), pagingQuery.PageSize);
            DataSet ds = DbService.ExecuteDataset(sql);
            if (ds == null || ds.Tables.Count != 2) return result;

            result.TotalCount = Utils.ToInt(ds.Tables[0].Rows[0][0]);
            result.Data = ds.Tables[1];
            return result;
        }
        #endregion

        #region 更新机组
        /// <summary>
        /// 更新机组
        /// </summary>
        /// <param name="xmlDoc">xmlDoc格式的机组数据.</param>
        /// <param name="id">待更新的Id.</param>
        /// <returns>
        /// System.Int32，大于0成功，否则失败
        /// </returns>
        /// <remarks>
		/// <list type="bullet">
		/// <item></item>
		/// </list>
        /// </remarks>
        public int Update(Dictionary<string, object> datajson)
        {
            try
            {
                ClearCache();
                int val = 0;
                GroupEntity ent = new GroupEntity();
                ent.SetValues(datajson);

                //检查信息是否存在
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Id", ent.Id);
                GroupEntity entGroup = DbAccess.GetEntity<GroupEntity>(TableName, dic);
                if (entGroup == null || entGroup.Id == null || entGroup.Id < 1)
                {
                    return -1;
                }
                entGroup.Name = ent.Name;
                entGroup.Domain = ent.Domain;
                entGroup.ImageDomain = ent.ImageDomain;
                val = DbAccess.ExecuteUpdate(TableName, entGroup, new string[] { "Id" });
                return val;
            }
            catch (Exception ex)
            {
                new Exceptions(ex.Message, ex);
            }
            return 0;
        }
        #endregion
    }
}
