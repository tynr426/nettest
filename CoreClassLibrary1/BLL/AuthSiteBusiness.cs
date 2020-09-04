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
using System.Web.Script.Serialization;
using System.Xml.Linq;

namespace Core.BLL
{
    class AuthSiteBusiness : CoreBusiness
    {
        #region override Property

        /// <summary>
        /// 常量数据表名
        /// </summary>
        public const string _TableName = _TablePrefix + "auth_site";

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
                return new AuthSiteEntity();
            }
        }
        #endregion

        #region 插入授权页面信息 
        /// <summary>
        /// 插入授权页面信息
        /// </summary>
        /// <param name="xml">XML格式的页面数据.</param>
        /// <returns>大于0成功，否则失败</returns>
        /// <remarks>
        /// <list type="bullet">
        /// </list>
        /// </remarks>
        public int Insert(string sitePageIds, int connectorId, int pageType)
        {
            ClearCache();
            int val = 0;

            //检查信息是否存在
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("ConnectorId", connectorId);
            dic.Add("Status", 1);
            dic.Add("PageType", pageType);
            AuthSiteEntity entConnector = DbAccess.GetEntity<AuthSiteEntity>(TableName, dic);
            if (entConnector != null && entConnector.Id > 0)
            {
                entConnector.UpdateTime = DateTime.Now;
                entConnector.PageIds = sitePageIds;
                val = DbAccess.ExecuteUpdate(TableName, entConnector, new string[] { "Id" });

            }
            else
            {
                AuthSiteEntity ent = new AuthSiteEntity();
                ent.Status = 1;
                ent.PageType = pageType;
                ent.ConnectorId = connectorId;
                ent.PageIds = sitePageIds;
                ent.AddTime = DateTime.Now;
                ent.UpdateTime = DateTime.Now;
                val = DbAccess.ExecuteInsert(TableName, ent);
            }

            return val;
        }

        #endregion


        #region 通过授权id获取网站库信息
        /// <summary>
        /// 通过授权id获取网站库信息
        /// </summary>
        /// <param name="id">授权ID</param>
        /// <returns>大于0成功，否则失败</returns>
        /// <remarks>
        /// <list type="bullet">
        /// <item></item>
        /// </list>
        /// </remarks>
        public ArrayList GetAuthSiteById(int connectorId, int pageType)
        {
            string str = string.Format("select * from {0} where DownConnectorId={1} limit 1", RelationBusiness._TableName, connectorId);
            DataTable relation = DbAccess.GetDataTable(str);
            if (relation == null || relation.Rows.Count == 0) return null;
            int upConnectorId = Utils.ToInt(relation.Rows[0]["UpConnectorId"]);
            ConnectorEntity upConnector = (ConnectorEntity)new ConnectorBusiness().GetEntity("Id=" + upConnectorId);
            DataTable dt = DBTable("ConnectorId=" + upConnectorId + " and PageType=" + pageType);
            string selectIds = "";
            if (dt != null && dt.Rows.Count > 0)
            {
                selectIds = Utils.ToString(dt.Rows[0]["PageIds"]);
            }
            //快马接口获取数据
            GroupEntity group = (GroupEntity)new GroupBusiness().GetEntity("Id=" + upConnector.GroupId);
            if (group == null)
            {
                return null;
            }
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("FKId", upConnector.SellerId.ToString());
            dic.Add("Ids", selectIds);
            string param = dic.ToJson();
            string msg = "";
            string datajson = ApiRequest.GetRemoteContent(group.Domain + "/Route.axd", "theme.import.webpagesbyids", dic, out msg);
            var jss = new JavaScriptSerializer();
            var dict = jss.Deserialize<Dictionary<string, object>>(datajson);
            if (!Utils.ToBool(dict["Success"]))
            {
                return null;
            }
            ArrayList content = dict["Content"] as ArrayList;
            if (content.Count <= 0)
            {
                return null;
            }
            foreach (Dictionary<string, object> item in content)
            {
                item.Add("ConnectorId", upConnectorId);
                item.Add("Supplier", upConnector.Name);
            }
            //DataTable webpage = new DataTable();
            //webpage.Columns.Add("ConnectorId");
            //if (webpage == null || webpage.Rows.Count == 0) return webpage;
            //foreach (DataRow dr in webpage.Rows)
            //{
            //    dr["ConnectorId"] = upConnectorId;
            //}
            return content;
        }
        #endregion
        #region 获取授权页面装修信息
        /// <summary>
        ///获取授权页面装修信息
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item></item>
        /// </list>
        /// </remarks>
        public ArrayList GetAuthSitePageList(int siteType, int connectorId)
        {

            string sql = string.Format("select * from {0}  where ConnectorId={1} and PageType={2} order by Id desc;", TableName,
                                         connectorId, siteType);
            DataTable dt = DbAccess.GetDataTable(sql);
            //if (dt == null || dt.Rows.Count == 0) return null;
            //快马接口获取装修列表
            ConnectorEntity connector = (ConnectorEntity)new ConnectorBusiness().GetEntity("Id=" + connectorId);
            if (connector == null)
            {
                return null;
            }
            GroupEntity group = (GroupEntity)new GroupBusiness().GetEntity("Id=" + connector.GroupId);
            if (group == null)
            {
                return null;
            }
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("FKId", connector.SellerId.ToString());
            dic.Add("SiteType", siteType.ToString());
            string msg = "";
            string datajson = ApiRequest.GetRemoteContent(group.Domain + "/Route.axd", "theme.import.webpageslist", dic, out msg);
            string selectIds = "";
            if (dt != null && dt.Rows.Count > 0)
            {
                selectIds = Utils.ToString(dt.Rows[0]["PageIds"]);
            }
            selectIds = "," + selectIds + ",";
            var jss = new JavaScriptSerializer();
            var dict = jss.Deserialize<Dictionary<string, object>>(datajson);
            if (!Utils.ToBool(dict["Success"]))
            {
                return null;
            }
            ArrayList content = dict["Content"] as ArrayList;
            if (content.Count <= 0)
            {
                return null;
            }
            foreach (Dictionary<string, object> item in content)
            {
                item.Add("ConnectorId", connectorId);
                if (!string.IsNullOrEmpty(selectIds))
                {
                    if (selectIds.Contains("," + Utils.ToString(item["Id"]) + ","))
                    {
                        item.Add("Checked", true);
                    }
                    else
                    {
                        item.Add("Checked", false);
                    }
                }
            }
            return content;
        }
        #endregion

        #region 导入授权页面装修
        public int ImportPage(int connectorId, int upConnectorId, int pageType, int pageId)
        {
            int val = 0;
            ConnectorEntity upConnector = (ConnectorEntity)new ConnectorBusiness().GetEntity("Id=" + upConnectorId);
            if (upConnector == null)
            {
                return val;
            }
            GroupEntity upGroup = (GroupEntity)new GroupBusiness().GetEntity("Id=" + upConnector.GroupId);
            if (upGroup == null)
            {
                return val;
            }
            Dictionary<string, string> dic1 = new Dictionary<string, string>();
            dic1.Add("FKId", upConnector.SellerId.ToString());
            string msg = "";
            string datajson = ApiRequest.GetRemoteContent(upGroup.Domain + "/Route.axd", "theme.pagewidget.list", dic1, out msg);
            if (string.IsNullOrEmpty(datajson))
            {
                return val;
            }
            var jss = new JavaScriptSerializer();
            var dict = jss.Deserialize<Dictionary<string, object>>(datajson);
            if (!Utils.ToBool(dict["Success"]))
            {
                return val;
            }
            ArrayList content = dict["Content"] as ArrayList;
            XElement el = new XElement("Styles");
            foreach (Dictionary<string,object> item in content) {
                XElement itemxml= new XElement("Style");
                itemxml.Add(
item.Select(kv => new XElement(kv.Key, kv.Value)));
                el.Add(itemxml);
            }

            //string json = content.ToJson();
            ConnectorEntity connector = (ConnectorEntity)new ConnectorBusiness().GetEntity("Id=" + connectorId);
            if (connector == null)
            {
                return val;
            }
            GroupEntity group = (GroupEntity)new GroupBusiness().GetEntity("Id=" + connector.GroupId);
            if (group == null)
            {
                return val;
            }
            Dictionary<string, string> dic2 = new Dictionary<string, string>();
            dic2.Add("WebPages", el.ToString());
            dic2.Add("ProprietorId", connector.SellerId.ToString());
            //string result = ApiRequest.GetRemoteContent(group.Domain + "/Route.axd", "theme.webpage.add", dic2, out msg);
            //Dictionary<string, string> dic2 = new Dictionary<string, string>();
            //dic2.Add("FKId", connector.SellerId.ToString());
            //dic2.Add("content", json.ToString());
            //string result = ApiRequest.GetRemoteContent(group.Domain + "/Route.axd", "theme.import.importpagedata", dic2, out msg);
            //if (string.IsNullOrEmpty(datajson))
            //{
            //    return val;
            //}
            //var jss2 = new JavaScriptSerializer();
            //var dict2 = jss.Deserialize<Dictionary<string, object>>(result);
            //if (!Utils.ToBool(dict2["Success"]))
            //{
            //    return val;
            //}
            //val = Utils.ToInt(dict2["Content"]);
            return val;
        }
        #endregion

    }
}
