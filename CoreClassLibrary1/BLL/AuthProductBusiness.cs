using Core.ENT;
using ECF;
using ECF.Data.Query;
using Sdk;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace Core.BLL
{
    class AuthProductBusiness : CoreBusiness
    {
        #region override Property

        /// <summary>
        /// 常量数据表名
        /// </summary>
        public const string _TableName = _TablePrefix + "auth_product";

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
                return new AuthProductEntity();
            }
        }
        #endregion

        #region 插入授权商品信息
        /// <summary>
        /// 插入授权商品信息
        /// </summary>
        /// <returns>大于0成功，否则失败</returns>
        /// <remarks>
        /// <list type="bullet">
        /// </list>
        /// </remarks>
        public int Insert(string productIds,int connectorId)
        {
            ClearCache();
            int val = 0;
            StringBuilder sb = new StringBuilder();
            if (string.IsNullOrEmpty(productIds)) return -1;

            //检查信息是否存在
            string sql = string.Format("select count(0) from {0} where ConnectorId={1} and ProductId in ({2})",TableName, connectorId, productIds);
            DataTable dt= DbAccess.GetDataTable(sql);
            if (Utils.ToInt(dt.Rows[0][0]) != 0) {
                return -1;
            }
            string[] ids = productIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string productId in ids) {
                int id = Utils.ToInt(productId);
                if (id > 0) {
                    AuthProductEntity ent = new AuthProductEntity();
                    ent.ConnectorId = connectorId;
                    ent.ProductId = id;
                    ent.Status = 1;
                    ent.AddTime = DateTime.Now;
                    ent.UpdateTime = DateTime.Now;
                    sb.AppendLine(DbAccess.Provider.InsertCommandText(ent, TableName));
                }
            }
            val = DbService.ExecuteNonQuery(sb.ToString());
            return val;
        }

        #endregion


        #region 删除授权商品记录 +override int Delete(int id)
        /// <summary>
        /// 删除授权商品记录
        /// </summary>
        /// <param name="id">授权ID</param>
        /// <returns>大于0成功，否则失败</returns>
        /// <remarks>
        /// <list type="bullet">
        /// <item></item>
        /// </list>
        /// </remarks>
        public int Delete(string authProductIds, int connectorId)
        {
            ClearCache();
            int val = 0;
            StringBuilder sb = new StringBuilder();
            if (string.IsNullOrEmpty(authProductIds)) return -1;

            //检查信息是否存在
            string sql = string.Format("select count(0) from {0} where ConnectorId={1} and Id in ({2})", TableName, connectorId, authProductIds);
            DataTable dt = DbAccess.GetDataTable(sql);
            if (dt == null || Utils.ToInt(dt.Rows[0][0]) == 0)
            {
                return -1;
            }
            string[] ids = authProductIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string authId in ids)
            {
                int id = Utils.ToInt(authId);
                if (id > 0)
                {
                    AuthProductEntity ent = (AuthProductEntity)new AuthProductBusiness().GetEntity("Id="+ id+ " and ConnectorId="+ connectorId);
                    if (ent != null && Utils.ToInt(ent.Id) > 0)
                    {
                        sb.AppendLine(DbAccess.Provider.DeleteCommandText(ent, TableName));
                    }
                }
            }
            val = DbService.ExecuteNonQuery(sb.ToString());
            return val;
        }
        #endregion

        #region 通过连接者id获取授权商品信息
        /// <summary>
        /// 通过连接者id获取授权商品信息
        /// </summary>
        /// <param name="id">连接者ID</param>
        /// <returns>大于0成功，否则失败</returns>
        /// <remarks>
        /// <list type="bullet">
        /// <item></item>
        /// </list>
        /// </remarks>
        public DataTable GetAuthProductByConnectorId(int connectorId)
        {
            string sql = string.Format("ConnectorId={0}", connectorId);
            DataTable dt = DBTable(sql);
            return dt;
        }
        #endregion



        #region 通过条件获取授权商品信息
        /// <summary>
        /// 通过条件获取授权商品信息
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item></item>
        /// </list>
        /// </remarks>
        public PagingResult GetAuthProductList(PagingQuery pagingQuery,int connectorId,bool auth=true)
        {
            PagingResult result = new PagingResult()
            {
                PageIndex = pagingQuery.PageIndex,
                PageSize = pagingQuery.PageSize
            };
            result.PageIndex = pagingQuery.PageIndex;
            result.PageSize = pagingQuery.PageSize;
            result.Data = new DataTable();
            ConnectorEntity connector = (ConnectorEntity)new ConnectorBusiness().GetEntity("Id=" + connectorId);
            if (connector == null)
            {
                return result;
            }
            GroupEntity group = (GroupEntity)new GroupBusiness().GetEntity("Id=" + connector.GroupId);
            if (group == null)
            {
                return result;
            }
            AuthBrandEntity brands = (AuthBrandEntity)new AuthBrandBusiness().GetEntity("ConnectorId=" + connectorId);
            AuthCategoryEntity categorys = (AuthCategoryEntity)new AuthCategoryBusiness().GetEntity("ConnectorId=" + connectorId);
            DataTable dt = DBTable("ConnectorId=" + connectorId);
            string ids;
            if (dt == null || dt.Rows.Count == 0) ids = "";
            ids= string.Join(",", dt.AsEnumerable().Select(dr => Utils.ToString(dr["ProductId"])));


            string condition = "";            
            if (auth)
            {
                condition += "<ProductId Oper=\"notin\">" + ids + "</ProductId><droitbrandids>" + brands.BrandIds+ "</droitbrandids><droitcategoryids>" + categorys.CategoryIds+ "</droitcategoryids>";
            }
            else {
                if (string.IsNullOrWhiteSpace(ids)) {
                    return result;
                }
                //未授权商品
                condition += "<ProductId>" + ids + "</ProductId>";
            }
            Dictionary<string, string> dic = new Dictionary<string, string>();
            //遍历所有子节点
            foreach (Condition c in pagingQuery.Condition)
            {
                if (!string.IsNullOrEmpty(c.Value))
                {
                    switch (c.Name.ToLower())
                    {

                        case "keywords":
                            dic.Add("Keywords", c.Value);
                            break;
                        case "brandid":
                            dic.Add("brandId", c.Value);
                            break;
                        case "categoryid":
                            dic.Add("cid", c.Value);
                            break;
                        case "productcode":
                            condition += "<ProductCode>" + c.Value + "</ProductCode>";
                            break;
                        case "productname":
                            condition += "<ProductName>" + c.Value + "</ProductName>";
                            break;
                        case "Bbarcode":
                            condition += "<BarCode>" + c.Value + "</BarCode>";
                            break;
                    }
                }
            }
            dic.Add("FKId", connector.SellerId.ToString());
            dic.Add("ProprietorId", connector.SellerId.ToString());
            dic.Add("Proprietor", "2");
            //dic.Add("UserId", connector.SellerId.ToString());
            dic.Add("FKFlag", "2");
            dic.Add("Condition", HttpUtility.UrlEncode(condition));
            dic.Add("PageIndex", pagingQuery.PageIndex.ToString());
            dic.Add("PageSize", pagingQuery.PageSize.ToString());
            string msg = "";
            string datajson = ApiRequest.GetRemoteContent(group.Domain + "/Route.axd", "vast.mall.product.page", dic, out msg);
            var jss = new JavaScriptSerializer();
            var dict = jss.Deserialize<Dictionary<string, object>>(datajson);
            if (!Utils.ToBool(dict["Success"]))
            {
                return null;
            }
            Dictionary<string, object> content = dict["Content"] as Dictionary<string, object>;
            if (content==null||content.Count <= 0)
            {
                return result;
            }
            result.TotalCount = Utils.ToInt(content["TotalCount"]);
            if (content["Data"] != null)
            {
                result.Data = content["Data"];
            }
            return result;
        }
        #endregion
        #region 获取上游卖家商品列表
        public PagingResult UpConnectorAuthList(PagingQuery pagingQuery, int connectorId) {
            PagingResult productList = new PagingResult() {
                PageIndex = pagingQuery.PageIndex,
                PageSize = pagingQuery.PageSize
            };
            string str = string.Format("select * from {0} where DownConnectorId={1} and Status=1 limit 1", RelationBusiness._TableName, connectorId);
            DataTable relation = DbAccess.GetDataTable(str);
            if (relation == null || relation.Rows.Count == 0) return productList;
            int upConnectorId = Utils.ToInt(relation.Rows[0]["UpConnectorId"]);
            ConnectorEntity upConnector = (ConnectorEntity)new ConnectorBusiness().GetEntity("Id=" + upConnectorId);
            GroupEntity upGroup = (GroupEntity)new GroupBusiness().GetEntity("Id=" + upConnector.GroupId);
            productList = GetAuthProductList(pagingQuery, upConnectorId,true);
            if (productList.Data!=null) {
                string sql = string.Format("select * from {0} where UpConnectorId={1} and DownConnectorId={2} and Status=1", ProductRelationBusiness._TableName, upConnectorId, connectorId);
                DataTable dt = DbAccess.GetDataTable(sql);
                string selectIds;
                if (dt == null || dt.Rows.Count <= 0)
                {
                    selectIds = "";
                }
                else {
                    selectIds = string.Join(",", dt.AsEnumerable().Select(dr => Utils.ToString(dr["UpProductId"])));
                    selectIds = "," + selectIds + ",";
                }

                    foreach (Dictionary<string, object> item in (ArrayList)productList.Data)
                    {
                    item.Add("ConnectorId", upConnectorId);
                    string preview = upGroup.Domain + "/Supplier/Product/Product.aspx?p="+ upConnector .VirtualDir+ "&pid=" + Utils.ToString(item["ProductId"]) + "&sid=" + upConnector.SellerId;
                    item.Add("Preview", preview);
                    if (!string.IsNullOrEmpty(selectIds))
                    {
                        if (selectIds.Contains("," + Utils.ToString(item["ProductId"]) + ","))
                        {
                            item.Add("Imported", true);
                        }
                        else
                        {
                            item.Add("Imported", false);
                        }
                    }
                    else {
                        item.Add("Imported", false);
                    }
                    }


            }
            return productList;
        }
        #endregion
        public int ImportProduct(string ids, int connectorId, int upConnectorId) {
            int val = 0;
            if (string.IsNullOrEmpty(ids))
            {
                return val;
            }
            string[] arrIds = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            return val;
        }
    }
}
