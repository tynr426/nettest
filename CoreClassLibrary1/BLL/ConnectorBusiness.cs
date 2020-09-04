using Core.ENT;
using ECF;
using ECF.Data;
using ECF.Data.Query;
using Sdk;
using Sdk.Socket;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace Core.BLL
{
    class ConnectorBusiness : CoreBusiness
    {
        #region override Property

        /// <summary>
        /// 常量数据表名
        /// </summary>
        public const string _TableName = _TablePrefix + "Connector";

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
                return new ConnectorEntity();
            }
        }
        #endregion

        #region 插入合作商信息
        /// <summary>
        /// 插入合作商信息
        /// </summary>
        /// <returns>大于0成功，否则失败</returns>
        /// <remarks>
        /// <list type="bullet">
        /// </list>
        /// </remarks>
        public int Insert(Dictionary<string, object> datajson)
        {
            ClearCache();
            int val = 0;
            ConnectorEntity ent = new ConnectorEntity();
            ent.SetValues(datajson);

            //检查信息是否存在
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("GroupId", ent.GroupId);
            dic.Add("SellerId", ent.SellerId);
            dic.Add("Name", ent.Name);
            ConnectorEntity entConnector = DbAccess.GetEntity<ConnectorEntity>(TableName, dic);
            if (entConnector != null && Utils.ToInt(entConnector.Id) != 0)
            {
                return -1;
            }
            ent.Status = 1;
            ent.AddTime = DateTime.Now;
            ent.UpdateTime = DateTime.Now;
            val = DbAccess.ExecuteInsert(TableName, ent);
            return val;
        }

        #endregion

        #region 标记删除合作商 +override int LogicDelete(int id)
        /// <summary>
        /// 获取合作商信息
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

        #region 删除合作商记录 +override int Delete(int id)
        /// <summary>
        /// 删除合作商记录
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
        #region 通过卖家id获取合作商信息
        /// <summary>
        /// 通过卖家id获取合作商信息
        /// </summary>
        /// <param name="id">卖家ID</param>
        /// <returns>大于0成功，否则失败</returns>
        /// <remarks>
        /// <list type="bullet">
        /// <item></item>
        /// </list>
        /// </remarks>
        public DataTable GetConnectorBySellerId(int sellerId, int groupId)
        {
            string sql = string.Format("GroupId={0} and SellerId={1}", groupId, sellerId);
            DataTable dt = DBTable(sql);
            return dt;
        }
        #endregion

        #region 通过条件获取合作商信息
        /// <summary>
        /// 通过条件获取合作商信息
        /// </summary>
        /// <param name="id">授权ID</param>
        /// <returns>大于0成功，否则失败</returns>
        /// <remarks>
        /// <list type="bullet">
        /// <item></item>
        /// </list>
        /// </remarks>
        public PagingResult GetMyConnectorList(PagingQuery pagingQuery, int connectorId,int status=0)
        {
            PagingResult result = new PagingResult();
            result.PageIndex = pagingQuery.PageIndex;
            result.PageSize = pagingQuery.PageSize;
            string condition = "1=1 and a.Status=1";
            string field = string.Empty;
            if (status == 0)
            {
                //我的下游合作商
                condition += string.Format(" and a.UpConnectorId={0}", connectorId);
                field = "a.DownConnectorId=b.Id";
            }
            else {
                //我的上游合作商
                condition += string.Format(" and a.DownConnectorId={0}", connectorId);
                field = "a.UpConnectorId=b.Id";
            }

            //遍历所有子节点
            foreach (Condition c in pagingQuery.Condition)
            {
                if (!string.IsNullOrEmpty(c.Value))
                {
                    switch (c.Name.ToLower())
                    {

                        case "keyword":
                            condition += string.Format(" and (b.Name like '%{0}%' or b.Mobile like '%{0}%')", c.Value);
                            break;
                        case "status":
                            condition += c.ToWhereString("a.Status");
                            break;
                    }
                }
            }

            string sql = string.Format("select count(0) from {0} a inner join {1} b on {2} where {3} ;select b.Name,b.SellerId,b.Logo,b.Mobile,b.GroupId,a.Id,a.UpConnectorId,a.UpBuyerId,a.DownConnectorId,a.AddTime,a.Status,a.IsOpen,a.InvitedFrom from {0} a inner join {1} b on {2}  where {3} order by b.AddTime desc limit {4},{5};",
                                        RelationBusiness._TableName, TableName, field, condition, pagingQuery.PageSize * (pagingQuery.PageIndex - 1), pagingQuery.PageSize);
            DataSet ds = DbService.ExecuteDataset(sql);
            if (ds == null || ds.Tables.Count != 2) return result;
            result.TotalCount = Utils.ToInt(ds.Tables[0].Rows[0][0]);
            result.Data = ds.Tables[1];
            return result;
        }

        #endregion
        #region 通过条件获取合作商信息
        /// <summary>
        /// 通过条件获取合作商信息
        /// </summary>
        /// <param name="id">授权ID</param>
        /// <returns>大于0成功，否则失败</returns>
        /// <remarks>
        /// <list type="bullet">
        /// <item></item>
        /// </list>
        /// </remarks>
        public PagingResult GetNewConnectorList(PagingQuery pagingQuery, int connectorId)
        {
            PagingResult result = new PagingResult();
            result.PageIndex = pagingQuery.PageIndex;
            result.PageSize = pagingQuery.PageSize;
            string condition = "1=1 and a.Status <> 1";
            string field = "";
            //遍历所有子节点
            foreach (Condition c in pagingQuery.Condition)
            {
                if (!string.IsNullOrEmpty(c.Value))
                {
                    switch (c.Name.ToLower())
                    {
                        case "keyword":
                            condition += string.Format(" and b.Name like '%{0}%' or b.Mobile like '%{0}%'", c.Value);
                            break;
                        case "name":
                            condition += c.ToWhereString("b.Name");
                            break;
                        case "mobile":
                            condition += c.ToWhereString("b.Mobile");
                            break;
                        case "status":
                            condition += c.ToWhereString("a.Status");
                            break;
                        case "invitation":
                            if (c.Value == "invit")
                            {
                                //我邀请的(下游)
                                condition += string.Format(" and a.UpConnectorId={0}", connectorId);
                                field = "a.DownConnectorId=b.Id";
                            }
                            else
                            {
                                //我申请的(上游)
                                condition += string.Format(" and a.DownConnectorId={0}", connectorId);
                                field = "a.UpConnectorId=b.Id";
                            }
                            break;
                    }
                }
            }

            string sql = string.Format("select count(0) from {0} a inner join {1} b on {2} where {3} ;select b.Name,b.SellerId,b.Logo,b.Mobile,b.GroupId,a.Id,a.UpConnectorId,a.UpBuyerId,a.DownConnectorId,a.AddTime,a.Status,a.IsOpen,a.InvitedFrom from {0} a inner join {1} b on {2}  where {3} order by b.AddTime desc limit {4},{5};",
                                        RelationBusiness._TableName, TableName, field, condition, pagingQuery.PageSize * (pagingQuery.PageIndex - 1), pagingQuery.PageSize);
            DataSet ds = DbService.ExecuteDataset(sql);
            if (ds == null || ds.Tables.Count != 2) return result;
            result.TotalCount = Utils.ToInt(ds.Tables[0].Rows[0][0]);
            result.Data = ds.Tables[1];
            return result;
        }
        #endregion

        #region 更新合作商 
        /// <summary>
        /// 更新合作商
        /// </summary>
        /// <param name="xmlDoc">xmlDoc格式的合作商数据.</param>
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
                ConnectorEntity ent = new ConnectorEntity();
                ent.SetValues(datajson);

                //检查信息是否存在
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Id", ent.Id);
                ConnectorEntity entConnector = DbAccess.GetEntity<ConnectorEntity>(TableName, dic);
                if (entConnector == null || entConnector.Id == null || entConnector.Id < 1)
                {
                    return -1;
                }
                entConnector.Name = ent.Name;
                entConnector.SellerId = ent.SellerId;
                entConnector.Logo = ent.Logo;
                entConnector.Mobile = ent.Mobile;
                entConnector.GroupId = ent.GroupId;
                entConnector.Remark = ent.Remark;
                entConnector.Status = ent.Status;
                entConnector.UpdateTime = DateTime.Now;
                val = DbAccess.ExecuteUpdate(TableName, entConnector, new string[] { "Id" });
                return val;
            }
            catch (Exception ex)
            {
                new Exceptions(ex.Message, ex);
            }
            return 0;
        }
        #endregion

        #region 通过条件获取连接器客户信息
        /// <summary>
        /// 通过条件获取连接器客户信息
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item></item>
        /// </list>
        /// </remarks>
        public PagingResult GetConnectorList(PagingQuery pagingQuery)
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
                        case "mobile":
                            condition += c.ToWhereString("Mobile");
                            break;
                        case "keyword":
                            condition += string.Format(" and (Name like '%{0}%' or Mobile  like '%{0}%')", c.Value);
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

        /// <summary>
        /// 交换token
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public IResultResponse ExchangeToken(int fk_id,int db_id)
        {
            DataTable dt = DbAccess.GetDataTable(string.Format(CommandConstant.ConnectorAuth, fk_id,db_id));

            if (dt != null && dt.Rows.Count > 0)
            {
                ConnectorEntity entity = new ConnectorEntity();
                entity.SetValues(dt.Rows[0]);
                AccessToken accessToken = new AccessToken(Utils.ToInt(entity.Id),
                    Utils.ToInt(entity.SellerId),
                    entity.Name,
                    entity.Mobile,
                    entity.Domain,
                    Utils.ToInt(entity.GroupId),
                    Utils.ToInt(entity.DBId),
                    Utils.ToInt(entity.UpConnectorId)
                    );
                Dictionary<string, object> authDic = dt.Rows[0].ToDictionary();
                authDic.Add("Token", accessToken.ToTokenString());
                authDic.Add("Qr", ECF.Security.AES.Encode(entity.Id.ToString()));
                authDic.Add("Expire", Utils.ToUnixTime(DateTime.Now.AddDays(1)));
                return ResultResponse.GetSuccessResult(authDic);
            }
            else
            {
                return ResultResponse.ExceptionResult("", null, 201);
            }
        }
        /// <summary>
        /// 获得上游信息
        /// </summary>
        /// <param name="fk_id"></param>
        /// <param name="db_id"></param>
        /// <returns></returns>
        public ConnectorRelation GetUpConnector(int fk_id, int db_id)
        {
            DataTable dt = DbAccess.GetDataTable(string.Format(CommandConstant.GetUpConnectorBySellerId, fk_id, db_id));
            ConnectorRelation entity = new ConnectorRelation();
            if (dt != null && dt.Rows.Count > 0)
            {
               
                entity.SetValues(dt.Rows[0]);
                
            }
            return entity;
        }
        /// <summary>
        /// 获得下游信息
        /// </summary>
        /// <param name="fk_id"></param>
        /// <param name="db_id"></param>
        /// <returns></returns>
        public ConnectorRelation GetDownConnector(int fk_id, int upBuyerId, int db_id)
        {
            DataTable dt = DbAccess.GetDataTable(string.Format(CommandConstant.GetDownConnectorBySellerId, fk_id, db_id, upBuyerId));
            ConnectorRelation entity = new ConnectorRelation();
            if (dt != null && dt.Rows.Count > 0)
            {

                entity.SetValues(dt.Rows[0]);

            }
            return entity;
        }
        /// <summary>
        /// 入驻
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public IResultResponse Join(Dictionary<string, object> dic)
        {
            if (dic == null)
            {
                return ResultResponse.ExceptionResult("对像为空");
            }
            ConnectorEntity entity = new ConnectorEntity();
            entity.SetValues(dic);
            entity.Status = 1;
            entity.AddTime = DateTime.Now;
            GroupEntity groupEntity = new GroupEntity();
            groupEntity.SetValues(dic);
            groupEntity.Status = 1;
            groupEntity.AddTime = DateTime.Now;
            IResultResponse resultResponse = null;
            using (IDTService dbHandler = DbAccess.DtService)
            {
                try
                {
                    dbHandler.BeginTransaction();
                    //保存主表
                    int id = Utils.ToInt(dbHandler.ExecuteScalar(DbProvider.InsertCommandText(groupEntity, GroupBusiness._TableName)));

                    if (id == 0) throw new Exception("操作失败");

                    entity.GroupId = id;

                    dbHandler.ExecuteScalar(DbProvider.InsertCommandText(entity, TableName));

                    dbHandler.CommitTransaction();
                    resultResponse = ResultResponse.GetSuccessResult(1);
                }
                catch (Exception ex)
                {
                    dbHandler.RollbackTransaction();
                    resultResponse = ResultResponse.ExceptionResult(ex.Message);
                }
            }
            return resultResponse;
        }
        /// <summary>
        /// 获得连接器信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Dictionary<string,object> GetConnectorById(int id)
        {
             DataTable dt = DbAccess.GetDataTable(string.Format(CommandConstant.GetConnectorById, id));

            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0].ToDictionary();
            }
            return new Dictionary<string, object>();
        }


    }
}
