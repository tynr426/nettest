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
    class AuthBrandBusiness : CoreBusiness
    {
        #region override Property

        /// <summary>
        /// 常量数据表名
        /// </summary>
        public const string _TableName = _TablePrefix + "auth_brand";

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
                return new AuthBrandEntity();
            }
        }
        #endregion

        #region 插入授权品牌信息 
        /// <summary>
        /// 插入授权品牌信息
        /// </summary>
        /// <returns>大于0成功，否则失败</returns>
        /// <remarks>
        /// <list type="bullet">
        /// </list>
        /// </remarks>
        public int Insert(int connectorId, string ids)
        {
            ClearCache();
            int val = 0;

            //检查信息是否存在
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("ConnectorId", connectorId);
            dic.Add("Status", 1);
            AuthBrandEntity entConnector = DbAccess.GetEntity<AuthBrandEntity>(TableName, dic);
            if (entConnector != null&& entConnector.Id>0)
            {
                entConnector.UpdateTime = DateTime.Now;
                entConnector.BrandIds = ids;
                val = DbAccess.ExecuteUpdate(TableName, entConnector, new string[] { "Id" });

            }
            else {
                AuthBrandEntity ent = new AuthBrandEntity();
                ent.Status = 1;
                ent.ConnectorId = connectorId;
                ent.BrandIds = ids;
                ent.AddTime = DateTime.Now;
                ent.UpdateTime = DateTime.Now;
                val = DbAccess.ExecuteInsert(TableName, ent);
            }

            return val;
        }

        #endregion


        #region 通过授权id获取授权品牌信息
        /// <summary>
        /// 通过授权id获取授权品牌信息
        /// </summary>
        /// <param name="id">授权ID</param>
        /// <returns>大于0成功，否则失败</returns>
        /// <remarks>
        /// <list type="bullet">
        /// <item></item>
        /// </list>
        /// </remarks>
        public DataTable GetAuthBrandById(int id)
        {
            string sql = string.Format("Id={0}", id);
            DataTable dt = DBTable(sql);
            return dt;
        }
        #endregion

        #region 通过条件获取授权品牌信息
        /// <summary>
        /// 通过条件获取授权品牌信息
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item></item>
        /// </list>
        /// </remarks>
        public ArrayList GetAuthBrandList(int connectorId, string firstSpell,string keyword,  bool showAll = true)
        {
            DataTable dt = DBTable("ConnectorId="+ connectorId);
            string selectIds = "";
            if (dt != null && dt.Rows.Count > 0)
            {
                selectIds = Utils.ToString(dt.Rows[0]["BrandIds"]);
            }

            //快马接口获取
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
            if (!showAll)
            {
                if (!string.IsNullOrWhiteSpace(selectIds))
                {
                    string condition = "<Id Oper=\"in\">" + selectIds + "</Id>";
                    dic.Add("Condition", HttpUtility.UrlEncode(condition));
                }
                else
                {
                    return null;
                }
            }
            if (keyword!="") {
                string condition = "<Name Oper=\"like\">" + keyword + "</Name>";
                dic.Add("Condition", HttpUtility.UrlEncode(condition));
            }
            dic.Add("FKId", connector.SellerId.ToString());
            dic.Add("FKFlag", "2");
            dic.Add("ProprietorId", connector.SellerId.ToString());
            dic.Add("TypeId","0" );
            dic.Add("FirstSpell", firstSpell);

            string msg = "";
            string datajson = ApiRequest.GetRemoteContent(group.Domain + "/Route.axd", "vast.mall.product.brands", dic, out msg);

            selectIds = "," + selectIds + ",";
            var jss = new JavaScriptSerializer();
            var dict = jss.Deserialize<Dictionary<string, object>>(datajson);
            if (!Utils.ToBool(dict["Success"])) {
                return null;
            }
            ArrayList content = dict["Content"] as ArrayList;
            if (content==null||content.Count <= 0)
            {
                return null;
            }
            foreach (Dictionary<string, object> item in content)
            {
                ArrayList brands = item["brands"] as ArrayList;
                foreach (Dictionary<string, object> brand in brands) {
                    brand.Add("ConnectorId", connectorId);
                    if (!string.IsNullOrEmpty(selectIds))
                    {
                        if (selectIds.Contains("," + Utils.ToString(brand["Id"]) + ","))
                        {
                            brand.Add("Checked", true);
                        }
                        else
                        {
                            brand.Add("Checked", false);
                        }
                    }
                }

            }
            return content;

        }
        #endregion
    }
}
