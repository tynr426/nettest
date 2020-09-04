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

namespace Core.BLL
{
    class AccountBusiness : CoreBusiness
    {
        #region override Property

        /// <summary>
        /// 常量数据表名
        /// </summary>
        public const string _TableName = _TablePrefix + "Account";

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
                return new AccountEntity();
            }
        }
        #endregion

        #region 插入管理员信息 +override int Insert(System.Xml.XmlDocument xml)
        /// <summary>
        /// 插入管理员信息
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
            AccountEntity ent = new AccountEntity();
            ent.SetValues(datajson);

            if (String.IsNullOrEmpty(ent.UserName) || String.IsNullOrEmpty(ent.Password))
            {
                return -1;
            }

            ent.Password = ECF.Security.Encrypt.MD532(ent.Password);

            //检查信息是否存在
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("UserName", ent.UserName);
            AccountEntity entAccount = DbAccess.GetEntity<AccountEntity>(TableName, dic);
            if (entAccount != null)
            {
                return -1;
            }
            ent.AddTime = DateTime.Now;
            ent.UpdateTime = DateTime.Now;
            val = DbAccess.ExecuteInsert(TableName, ent);
            return val;
        }

        #endregion
        #region 更新管理员
        /// <summary>
        /// 更新管理员
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
                AccountEntity ent = new AccountEntity();
                ent.SetValues(datajson);

                //检查信息是否存在
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Id", ent.Id);
                AccountEntity entAccount = DbAccess.GetEntity<AccountEntity>(TableName, dic);
                if (entAccount == null || entAccount.Id == null || entAccount.Id < 1)
                {
                    return -1;
                }
                entAccount.UpdateTime= DateTime.Now;
                entAccount.UserName = ent.UserName;
                entAccount.Password = ECF.Security.Encrypt.MD532(ent.Password);
                entAccount.NickName = ent.NickName;
                entAccount.Remark = ent.Remark;
                val = DbAccess.ExecuteUpdate(TableName, entAccount, new string[] { "Id" });
                return val;
            }
            catch (Exception ex)
            {
                new Exceptions(ex.Message, ex);
            }
            return 0;
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
        public PagingResult GetList(PagingQuery pagingQuery)
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
                            condition += string.Format(" and UserName like '%{0}%'", c.Value);
                            break;
                        case "status":
                            condition += c.ToWhereString("Status");
                            break;
                    }
                }
            }

            string sql = string.Format("select count(0) from {0} where {1} ;select Id,UserName,NickName,Errors,LoginIP,LoginTime,AddTime,Status from {0}  where {1} order by AddTime desc limit {2},{3};", TableName,
                                        condition, pagingQuery.PageSize * (pagingQuery.PageIndex - 1), pagingQuery.PageSize);
            DataSet ds = DbService.ExecuteDataset(sql);
            if (ds == null || ds.Tables.Count != 2) return result;

            result.TotalCount = Utils.ToInt(ds.Tables[0].Rows[0][0]);
            result.Data = ds.Tables[1];
            return result;
        }
        #endregion
    }
}
