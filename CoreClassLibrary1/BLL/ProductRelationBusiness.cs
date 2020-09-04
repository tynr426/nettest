using Core.ENT;
using ECF;
using ECF.Json;
using Sdk;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml;

namespace Core.BLL
{
    class ProductRelationBusiness : CoreBusiness
    {
        #region override Property

        /// <summary>
        /// 常量数据表名
        /// </summary>
        public const string _TableName = _TablePrefix + "Product_Relation";

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
                return new ProductRelationEntity();
            }
        }
        #endregion

        #region 插入授权关系信息 +override int Insert(System.Xml.XmlDocument xml)
        /// <summary>
        /// 插入授权关系信息
        /// </summary>
        /// <param name="xml">XML格式的品牌数据.</param>
        /// <returns>大于0成功，否则失败</returns>
        /// <remarks>
        /// <list type="bullet">
        /// </list>
        /// </remarks>
        public override int Insert(System.Xml.XmlDocument xml)
        {
            ClearCache();
            int val = 0;
            ProductRelationEntity ent = new ProductRelationEntity();
            ent.SetValues(xml);

            //检查信息是否存在
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("UpConnectorId", ent.UpConnectorId);
            dic.Add("UpProductId", ent.UpProductId);
            dic.Add("UpGoodsId", ent.UpGoodsId);
            dic.Add("DownConnectorId", ent.DownConnectorId);
            dic.Add("DownProductId", ent.DownProductId);
            dic.Add("DownGoodsId", ent.DownGoodsId);
            ProductRelationEntity entAccount = DbAccess.GetEntity<ProductRelationEntity>(TableName, dic);
            if (entAccount != null)
            {
                return -1;
            }
            val = DbAccess.ExecuteInsert(TableName, ent);
            return val;
        }

        #endregion

        #region 批量导入商品
        public IResultResponse BatchImportProduct(int connectorId, int upConnectorId, string productIds)
        {
            if (string.IsNullOrWhiteSpace(productIds))
            {
                return ResultResponse.ExceptionResult("请选择上货商品");
            }
            string[] arrIds = productIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                foreach (string id in arrIds)
                {
                    int productId = Utils.ToInt(id);
                    if (productId > 0)
                    {
                        ImportProduct(connectorId, upConnectorId, productId);
                    }
                }
                return ResultResponse.GetSuccessResult("导入商品完毕");
            }
            catch (Exception ex)
            {
                return ResultResponse.ExceptionResult("导入商品失败:" + ex.Message);
            }

        }
        #endregion

        #region 导入商品
        public IResultResponse ImportProduct(int connectorId, int upConnectorId, int productId)
        {
            int val = 0;
            ConnectorEntity upConnector = (ConnectorEntity)new ConnectorBusiness().GetEntity("Id=" + upConnectorId);
            if (upConnector == null || upConnector.Id == null)
            {
                return ResultResponse.ExceptionResult("上游不存在");
            }
            GroupEntity upGroup = (GroupEntity)new GroupBusiness().GetEntity("Id=" + upConnector.GroupId);
            if (upGroup == null || upGroup.Id == null)
            {
                return ResultResponse.ExceptionResult("上游不存在");
            }
            ConnectorEntity connector = (ConnectorEntity)new ConnectorBusiness().GetEntity("Id=" + connectorId);
            if (connector == null || connector.Id == null)
            {
                return ResultResponse.ExceptionResult("下游不存在");
            }
            GroupEntity group = (GroupEntity)new GroupBusiness().GetEntity("Id=" + connector.GroupId);
            if (group == null || group.Id == null)
            {
                return ResultResponse.ExceptionResult("下游不存在");
            }
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("FKId", upConnector.SellerId.ToString());
            dic.Add("FKFlag", "2");
            dic.Add("ProductId", productId.ToString());
            string msg = "";
            string datajson = ApiRequest.GetRemoteContent(upGroup.Domain + "/Route.axd", "mall.product.pullinfo", dic, out msg);
            if (string.IsNullOrEmpty(datajson))
            {
                return ResultResponse.ExceptionResult("拉取数据为空");
            }
            var jss = new JavaScriptSerializer();
            var dict = jss.Deserialize<Dictionary<string, object>>(datajson);
            if (!Utils.ToBool(dict["Success"]))
            {
                return ResultResponse.ExceptionResult(Utils.ToString(dict["Message"])); ;
            }
            #region 处理数据
            Dictionary<string, object> content = dict["Content"] as Dictionary<string, object>;
            Dictionary<string, object> importData = new Dictionary<string, object>();
            List<string> picList = new List<string>();
            if (content.ContainsKey("Product"))
            {
                Dictionary<string, object> product = content["Product"] as Dictionary<string, object>;
                importData = product;
                int id = Utils.ToInt(product["ProductId"]);
                importData.Remove("ProductId");
                importData.Add("ThirdId", id);
                importData["BrandId"] = 0;
                importData["CategoryId"] = 0;
                importData["TypeId"] = 0;
                importData.Add("Marketable", true);
                importData.Remove("FootIntro");
                importData.Remove("HeadIntro");
                importData.Remove("ShowstandIntro");
                importData.Remove("DefaultUnitId");
                importData.Add("DefaultSaleUnitId", 0);
                importData.Remove("BrandName");
                importData.Remove("CustomCateId");
            }
            ArrayList goods = new ArrayList();
            if (content.ContainsKey("Goods"))
            {
                goods = content["Goods"] as ArrayList;
                foreach (Dictionary<string, object> item in goods)
                {
                    int id = Utils.ToInt(item["GoodsId"]);
                    item.Remove("GoodsId");
                    item.Remove("ProductId");
                    item.Add("ThirdId", id);
                    string name = item["SpecValueText"].ToString();
                    item.Add("Name", name);
                    item.Remove("SpecValueText");
                    if (item.ContainsKey("SpecValue"))
                    {
                        ArrayList specValue = item["SpecValue"] as ArrayList;
                        foreach (Dictionary<string, object> specItem in specValue)
                        {
                            specItem["Id"] = 0;
                            string pic = specItem.ContainsKey("ImagePath") ? specItem["ImagePath"].ToString() : "";
                            if (pic.IndexOf("?") > -1)
                            {
                                pic = pic.Substring(0, pic.IndexOf("?"));
                            }
                            if (pic != "")
                            {
                                picList.Add(pic);
                                specItem["ImagePath"] = "/UserFiles/Supplier" + connector.SellerId + "/" + DateTime.Now.Date.ToString("yyyMMdd") + "/Images/" + Path.GetFileName(pic); ;
                            }
                            specItem.Remove("SpecId");
                        }
                    }
                    if (item.ContainsKey("Pics"))
                    {
                        ArrayList pics = item["Pics"] as ArrayList;
                        foreach (Dictionary<string, object> picItem in pics)
                        {
                            string pic = picItem.ContainsKey("VirtualPath") ? picItem["VirtualPath"].ToString() : "";
                            if (pic.IndexOf("?") > -1)
                            {
                                pic = pic.Substring(0, pic.IndexOf("?"));
                            }
                            if (pic != "")
                            {

                                picList.Add(pic);
                                picItem["ImagePath"] = "/UserFiles/Supplier" + connector.SellerId + "/" + DateTime.Now.Date.ToString("yyyMMdd") + "/Images/" + Path.GetFileName(pic);
                            }
                        }
                    }
                }
            }
            importData.Add("Goods", goods);
            ArrayList auxiliaryUnit = new ArrayList();
            if (content.ContainsKey("AuxiliaryUnits"))
            {
                auxiliaryUnit = content["AuxiliaryUnits"] as ArrayList;
                foreach (Dictionary<string, object> unitItem in auxiliaryUnit)
                {
                    unitItem["Id"] = 0;
                }
            }
            importData.Add("AuxiliaryUnit", auxiliaryUnit);
            importData.Add("ViceCatalogIds", content["ViceCatalogIds"]);
            ArrayList picsList = new ArrayList();
            if (content.ContainsKey("Pics"))
            {
                picsList = content["Pics"] as ArrayList;
                foreach (Dictionary<string, object> picItem in picsList)
                {
                    picItem.Remove("Id");
                    picItem.Remove("VirtualPath");
                    string picContent = picItem["Content"].ToString();
                    if (picContent.IndexOf("?") > -1)
                    {
                        picContent = picContent.Substring(0, picContent.IndexOf("?"));
                    }
                    if (picContent != "")
                    {
                        picList.Add(picContent);
                        picItem["Content"] = "/UserFiles/Supplier" + connector.SellerId + "/" + DateTime.Now.Date.ToString("yyyMMdd") + "/Images/" + Path.GetFileName(picContent);
                    }
                }
            }
            importData.Add("Pics", picsList);

            if (content.ContainsKey("Video"))
            {
                Dictionary<string, object> video = content["Video"] as Dictionary<string, object>;
                video.Remove("Id");
                video.Remove("GoodsId");
                video.Remove("FileType");
                video.Remove("Reorder");
                video.Remove("HasVideo");
                importData.Add("Video", video);
            }


            ArrayList specification = new ArrayList();
            if (content.ContainsKey("Specification"))
            {
                specification = content["Specification"] as ArrayList;
                foreach (Dictionary<string, object> specItem in specification)
                {
                    specItem["Id"] = 0;
                }
            }
            importData.Add("Specification", specification);
            importData.Add("Extends", content["Extends"]);
            importData.Add("Params", content["Params"]);
            importData.Add("SEO", content["SEO"]);
            #endregion
            #region 导入图片数据
            StringBuilder picsb = new StringBuilder();
            for (int i = 0; i < picList.Count; i++)
            {
                picsb.Append(string.Format("<Image><Path>{0}</Path><Url>{1}{0}</Url></Image>", picList[i], upGroup.ImageDomain));
            }
            Task.Run(() =>
            {
                IResultResponse pic = ApiRequest.GetResponse(group.Domain, "material.upload.saveremoteimage", new Dictionary<string, string>() {

                    {"Module","Product" },
                    {"FKId",connector.SellerId.ToString() },
                    {"FKFlag","2" },
                    {"Images",(picsb.ToString()) },
                });
            });
            #endregion

            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            list.Add(importData);

            string data = JsonMapper.ToJson(list);
            IResultResponse result = ApiRequest.GetResponse(group.Domain, "mall.product.push", new Dictionary<string, string>() {
                        {"ProprietorId",connector.SellerId.ToString() },
                        {"FKId",connector.SellerId.ToString() },
                        {"FKFlag","2" },
                        {"Products",HttpUtility.UrlEncode(data) },
                    });
            if (result.Success)
            {
                DataTable dt = result.Content as DataTable;
                if (dt == null || dt.Rows.Count == 0) return ResultResponse.ExceptionResult("转换异常错误");
                StringBuilder sb = new StringBuilder();
                string error = string.Empty;
                foreach (DataRow dr in dt.Rows)
                {
                    var success = Utils.ToString(dr["Success"]) == "True";
                    if (!success)
                    {
                        error = (Utils.ToString(dt.Rows[0]["Message"]));
                        continue;
                    }

                    Dictionary<string, object> output = dt.Rows[0]["Content"] as Dictionary<string, object>;
                    ProductRelationEntity ent = new ProductRelationEntity();
                    ent.UpConnectorId = upConnectorId;
                    ent.UpProductId = productId;
                    ent.UpGoodsId = Utils.ToInt(output["ThirdId"]);
                    ent.DownConnectorId = connectorId;
                    ent.DownProductId = Utils.ToInt(output["Id"]);
                    ent.DownGoodsId = Utils.ToInt(output["Id"]);
                    sb.AppendLine(DbAccess.Provider.InsertCommandText(ent, TableName));
                }
                if (sb.Length > 0)
                {
                    val = DbService.ExecuteNonQuery(sb.ToString());
                }
                else
                {
                    return ResultResponse.ExceptionResult(error);
                }
                return ResultResponse.GetSuccessResult(val);
            }
            else
            {
                return result;
            }

        }
        #endregion

        #region 标记删除授权关系 +override int LogicDelete(int id)
        /// <summary>
        /// 获取授权关系信息
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

        #region 删除授权关系记录 +override int Delete(int id)
        /// <summary>
        /// 删除授权关系记录
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

        #region 通过上游卖家id获取授权关系信息
        /// <summary>
        /// 通过上游卖家id获取授权关系信息
        /// </summary>
        /// <param name="id">卖家ID</param>
        /// <returns>大于0成功，否则失败</returns>
        /// <remarks>
        /// <list type="bullet">
        /// <item></item>
        /// </list>
        /// </remarks>
        public DataTable GetRelationByUpConnectorId(int upConnectorId)
        {
            string sql = string.Format("UpConnectorId={0}", upConnectorId);
            DataTable dt = DBTable(sql);
            return dt;
        }
        #endregion
        #region 通过下游卖家id获取授权关系信息
        /// <summary>
        /// 通过下游卖家id获取授权关系信息
        /// </summary>
        /// <param name="id">卖家ID</param>
        /// <returns>大于0成功，否则失败</returns>
        /// <remarks>
        /// <list type="bullet">
        /// <item></item>
        /// </list>
        /// </remarks>
        public DataTable GetRelationByDownConnectorId(int downConnectorId)
        {
            string sql = string.Format("DownConnectorId={0}", downConnectorId);
            DataTable dt = DBTable(sql);
            return dt;
        }
        #endregion
        #region 通过授权id获取授权关系信息
        /// <summary>
        /// 通过授权id获取授权关系信息
        /// </summary>
        /// <param name="id">授权ID</param>
        /// <returns>大于0成功，否则失败</returns>
        /// <remarks>
        /// <list type="bullet">
        /// <item></item>
        /// </list>
        /// </remarks>
        public DataTable GetRelationById(int id)
        {
            string sql = string.Format("Id={0}", id);
            DataTable dt = DBTable(sql);
            return dt;
        }
        #endregion
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="downId"></param>
        /// <param name="upId"></param>
        /// <returns></returns>
        public int DeleteData(int downId, int upId)
        {
            return DbAccess.ExecuteDelete(TableName, $"UpConnectorId={upId} and DownConnectorId={downId}");
        }
        /// <summary>
        /// 是否有克隆权限
        /// </summary>
        /// <param name="downId"></param>
        /// <param name="upId"></param>
        /// <returns></returns>
        public bool CloneAuthority(int downId, int upId)
        {
            return !DbAccess.Exists(TableName, $"UpConnectorId={upId} and DownConnectorId={downId}");
        }
        #region GetRelationMappingEntity
        /// <summary>
        /// 获得关联映射
        /// </summary>
        /// <param name="sellerId"></param>
        /// <param name="dbId"></param>
        /// <returns></returns>
        public RelationMappingEntity GetUpMappingEntity(int sellerId, int dbId)
        {
            RelationMappingEntity relationMappingEntity = new RelationMappingEntity();
            ConnectorRelation connectorEntity = new ConnectorBusiness().GetUpConnector(sellerId, dbId);
            if (connectorEntity.Id == null)
            {
                return relationMappingEntity;
            }

            relationMappingEntity.ConnectorMapping.Add(sellerId, Utils.ToInt(connectorEntity.SellerId));

            relationMappingEntity.ConnectorEntity = connectorEntity;

            DataTable data = DBTable("UpConnectorId=" + connectorEntity.Id + " and DownConnectorId=" + connectorEntity.ThirdConnectorId);

            if (data == null || data.Rows.Count == 0)
            {
                return relationMappingEntity;
            }

            foreach (DataRow dr in data.Rows)
            {
                int upProductId = Utils.ToInt(dr["UpProductId"]);
                int downProductId = Utils.ToInt(dr["DownProductId"]);
                int upGoodsId = Utils.ToInt(dr["UpGoodsId"]);
                int downGoodsId = Utils.ToInt(dr["DownGoodsId"]);
                if (!relationMappingEntity.ProductMapping.ContainsKey(downProductId))
                {
                    relationMappingEntity.ProductMapping.Add(downProductId, upProductId);
                }
                if (!relationMappingEntity.GoodsMapping.ContainsKey(downGoodsId))
                {
                    relationMappingEntity.GoodsMapping.Add(downGoodsId, upGoodsId);
                }
            }
            return relationMappingEntity;
        }
        /// <summary>
        /// 获得关联映射
        /// </summary>
        /// <param name="sellerId"></param>
        /// <param name="dbId"></param>
        /// <returns></returns>
        public RelationMappingEntity GetDownMappingEntity(int sellerId, int upBuyerId, int dbId)
        {
            RelationMappingEntity relationMappingEntity = new RelationMappingEntity();
            ConnectorRelation connectorEntity = new ConnectorBusiness().GetDownConnector(sellerId, upBuyerId, dbId);
            if (connectorEntity.Id == null)
            {
                return relationMappingEntity;
            }

            relationMappingEntity.ConnectorMapping.Add(sellerId, Utils.ToInt(connectorEntity.SellerId));

            relationMappingEntity.ConnectorEntity = connectorEntity;

            DataTable data = DBTable("UpConnectorId=" + connectorEntity.ThirdConnectorId + " and DownConnectorId=" + connectorEntity.Id);

            if (data == null || data.Rows.Count == 0)
            {
                return relationMappingEntity;
            }

            foreach (DataRow dr in data.Rows)
            {
                int upProductId = Utils.ToInt(dr["UpProductId"]);
                int downProductId = Utils.ToInt(dr["DownProductId"]);
                int upGoodsId = Utils.ToInt(dr["UpGoodsId"]);
                int downGoodsId = Utils.ToInt(dr["DownGoodsId"]);
                if (!relationMappingEntity.ProductMapping.ContainsKey(upProductId))
                {
                    relationMappingEntity.ProductMapping.Add(upProductId, downProductId);
                }
                if (!relationMappingEntity.GoodsMapping.ContainsKey(upGoodsId))
                {
                    relationMappingEntity.GoodsMapping.Add(upGoodsId, downGoodsId);
                }
            }
            return relationMappingEntity;
        }
        #endregion

        #region CheckProduct 代下单货品检查是否为已上货
        /// <summary>
        /// 代下单货品检查是否为已上货
        /// </summary>
        /// <param name="sellerId"></param>
        /// <param name="dbId"></param>
        /// <param name="productInfo"></param>
        /// <returns></returns>
        public bool CheckProduct(int sellerId, int dbId, XmlDocument xml)
        {
            bool check = true;
            RelationMappingEntity relationMappingEntity = GetUpMappingEntity(sellerId, dbId);
            if (relationMappingEntity.GoodsMapping.Count == 0) return false;

            string productInfo = xml.SelectSingleNode("ProductInfo").InnerText;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<root>" + productInfo + "</root>");
            foreach (XmlNode good in doc.DocumentElement.ChildNodes)
            {
                int goodsId = Utils.ToInt(good.SelectSingleNode("GoodsId").InnerText);
                int productId = Utils.ToInt(good.SelectSingleNode("ProductId").InnerText);
                if (relationMappingEntity.ProductMapping.ContainsKey(productId) && relationMappingEntity.GoodsMapping.ContainsKey(goodsId))
                {
                    continue;
                }
                else
                {
                    check = false;
                    break;
                }
            }
            return check;
        }
        #endregion

    }
}
