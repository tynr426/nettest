using Core.ENT;
using ECF;
using ECF.Security;
using Sdk;
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
    class RelationBusiness : CoreBusiness
    {
        #region override Property

        /// <summary>
        /// 常量数据表名
        /// </summary>
        public const string _TableName = _TablePrefix + "Relation";

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
                return new RelationEntity();
            }
        }
        #endregion

        #region 插入授权关系信息
        /// <summary>
        /// 插入授权关系信息
        /// </summary>
        /// <returns>大于0成功，否则失败</returns>
        /// <remarks>
        /// <list type="bullet">
        /// </list>
        /// </remarks>
        public int InsertUpConnectorRelation(string mobile,int downConnectorId)
        {
            int val = 0;
            //快马接口查询
            int upBuyerId = 0;
            //检查信息是否存在
            Dictionary<string, object> dic1 = new Dictionary<string, object>();
            dic1.Add("Mobile", mobile);
            ConnectorEntity entConnector = DbAccess.GetEntity<ConnectorEntity>(ConnectorBusiness._TableName,dic1);
            if (entConnector == null)
            {
                return -2;
            }
            if (entConnector.Id == downConnectorId)
            {
                return -1;
            }
            Dictionary<string, object> dic2 = new Dictionary<string, object>();
            //dic2.Add("UpConnectorId", entConnector.Id);
            //dic2.Add("UpBuyerId", upBuyerId);
            dic2.Add("DownConnectorId", downConnectorId);

            RelationEntity entRelation = DbAccess.GetEntity<RelationEntity>(TableName, dic2);
            if (entRelation != null&& entRelation.UpConnectorId!=0)
            {
                return -1;
            }
            RelationEntity ent = new RelationEntity();
            ent.UpConnectorId = entConnector.Id;
            ent.UpBuyerId = upBuyerId;
            ent.DownConnectorId = downConnectorId;
            ent.Status = 0;//0待处理
            ent.AddTime = DateTime.Now;
            ent.UpdateTime = DateTime.Now;
            ent.InvitedFrom = 0;
            val = DbAccess.ExecuteInsert(TableName, ent);
            return val;
        }
        public int InsertDownConnectorRelation(int downConnectorId, int upConnectorId)
        {
            int val = 0;
            //快马接口查询
            int upBuyerId = 0;
            //检查信息是否存在
            ConnectorEntity downConnector = (ConnectorEntity)new ConnectorBusiness().GetEntity("Id=" + downConnectorId);
            if (downConnector == null|| Utils.ToInt(downConnector.Id)==0)
            {
                return -2;
            }
            RelationEntity entRelation = (RelationEntity)new RelationBusiness().GetEntity("DownConnectorId=" + downConnectorId);
            if (entRelation != null&&Utils.ToInt(entRelation.Id)!=0)
            {
                return -1;
            }
            RelationEntity ent = new RelationEntity();
            ent.UpConnectorId = upConnectorId;
            ent.UpBuyerId = upBuyerId;
            ent.DownConnectorId = downConnector.Id;
            ent.Status = 0;//0待处理
            ent.AddTime = DateTime.Now;
            ent.UpdateTime = DateTime.Now;
            ent.InvitedFrom = 1;
            val = DbAccess.ExecuteInsert(TableName, ent);
            return val;
        }
        #endregion

        #region 审核合作商 
        /// <summary>
        /// 审核合作商
        /// </summary>
        /// <returns>
        /// System.Int32，大于0成功，否则失败
        /// </returns>
        /// <remarks>
		/// <list type="bullet">
		/// <item></item>
		/// </list>
        /// </remarks>
        public IResultResponse CheckDownConnector(int connectorId,int id,int status)
        {
            try
            {
                int val = 0;
                ConnectorEntity connector = (ConnectorEntity)new ConnectorBusiness().GetEntity("Id=" + connectorId);
                if (connector == null || Utils.ToInt(connector.Id) == 0)
                {
                    return ResultResponse.ExceptionResult("不存在该上游");
                }
                GroupEntity group = (GroupEntity)new GroupBusiness().GetEntity("Id=" + connector.GroupId);
                if (group == null || Utils.ToInt(group.Id) == 0)
                {
                    return ResultResponse.ExceptionResult("不存在该上游机组");
                }
                RelationEntity entRelation = (RelationEntity)new RelationBusiness().GetEntity("Id=" + id);
                if (entRelation == null || Utils.ToInt(entRelation.Id) == 0)
                {
                    return ResultResponse.ExceptionResult("不存在合作关系");
                }
                ConnectorEntity downConnector = (ConnectorEntity)new ConnectorBusiness().GetEntity("Id=" + entRelation.DownConnectorId);
                if (downConnector == null || Utils.ToInt(downConnector.Id) == 0)
                {
                    return ResultResponse.ExceptionResult("不存在该下游");
                }
                if (status==1&& entRelation.UpBuyerId==0)
                {
                    //快马上游插入下游客户账户
                    long ts = Utils.ToUnixTime(DateTime.Now.AddMinutes(1));
                    Dictionary<string, string> dic = new Dictionary<string, string>() {

                {"fk_id", connector.SellerId.ToString()},
                {"expire",ts.ToString() },
                {"mobile",downConnector.Mobile}
            };
                    string json = dic.ToJson();
                    string token = ECF.Security.AES.Encode(json);
                    IResultResponse result= ApiRequest.GetResponse(group.Domain, "account.add.downconnector", new Dictionary<string, string>() {
                        {"exchange_token",HttpUtility.UrlEncode(token) },
                    });
                    if (result.Success)
                    {
                        Dictionary<string, object> content = result.Content as Dictionary<string, object>;
                        int storeId = content.ToInt("StoreId", 0);
                        entRelation.IsDefault = 1;
                        entRelation.UpBuyerId = storeId;
                    }
                    else {
                        return ResultResponse.ExceptionResult("上游零售商关联失败,请稍后在试");
                    }
                }
                entRelation.Status = status;
                entRelation.UpdateTime = DateTime.Now;
                val = DbAccess.ExecuteUpdate(TableName, entRelation, new string[] { "Id" });
                if (val > 0)
                {
                    return ResultResponse.GetSuccessResult(1);
                }
                return ResultResponse.ExceptionResult("接受邀请失败");
            }
            catch (Exception ex)
            {
                return ResultResponse.ExceptionResult(ex);
            }
        }

        public IResultResponse CheckUpConnector(int downConnectorId, int id, int status)
        {
            try
            {
                ClearCache();
                int val = 0;
                ConnectorEntity downConnector = (ConnectorEntity)new ConnectorBusiness().GetEntity("Id=" + downConnectorId);
                if (downConnector == null || Utils.ToInt(downConnector.Id) == 0)
                {
                    return ResultResponse.ExceptionResult("不存在该下游");
                }
                RelationEntity entRelation = (RelationEntity)new RelationBusiness().GetEntity("Id=" + id+ " and DownConnectorId="+ downConnectorId);
                if (entRelation == null || Utils.ToInt(entRelation.Id) == 0)
                {
                    return ResultResponse.ExceptionResult("不存在合作关系");
                }
                ConnectorEntity connector = (ConnectorEntity)new ConnectorBusiness().GetEntity("Id=" + entRelation.UpConnectorId);
                if (connector == null || Utils.ToInt(connector.Id) == 0)
                {
                    return ResultResponse.ExceptionResult("不存在该上游");
                }
                GroupEntity group = (GroupEntity)new GroupBusiness().GetEntity("Id=" + connector.GroupId);
                if (group == null || Utils.ToInt(group.Id) == 0)
                {
                    return ResultResponse.ExceptionResult("不存在该上游机组");
                }
                if (status == 1 && entRelation.UpBuyerId == 0)
                {
                    //快马上游插入下游客户账户
                    long ts = Utils.ToUnixTime(DateTime.Now.AddMinutes(1));
                    Dictionary<string, string> dic = new Dictionary<string, string>() {

                {"fk_id", connector.SellerId.ToString()},
                {"expire",ts.ToString() },
                {"mobile",downConnector.Mobile}
            };
                    string json = dic.ToJson();
                    string token = ECF.Security.AES.Encode(json);
                    IResultResponse result = ApiRequest.GetResponse(group.Domain, "account.add.downconnector", new Dictionary<string, string>() {
                        {"exchange_token",HttpUtility.UrlEncode(token) },
                    });
                    if (result.Success)
                    {
                        Dictionary<string, object> content = result.Content as Dictionary<string, object>;
                        int storeId = content.ToInt("StoreId", 0);
                        entRelation.IsDefault = 1;
                        entRelation.UpBuyerId = storeId;
                    }
                    else
                    {
                        return ResultResponse.ExceptionResult("上游零售商关联失败,请稍后在试");
                    }
                }
                entRelation.IsDefault = 1;
                entRelation.Status = status;
                entRelation.UpdateTime = DateTime.Now;
                val = DbAccess.ExecuteUpdate(TableName, entRelation, new string[] { "Id" });
                if (val>0) {
                    return ResultResponse.GetSuccessResult(1);
                }
                return ResultResponse.ExceptionResult("接受邀请失败");
            }
            catch (Exception ex)
            {
                return ResultResponse.ExceptionResult(ex);
            }
        }
        #endregion

        #region 删除授权关系记录
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
        public int Delete(int id,int connectorId)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("DownConnectorId", connectorId);
            dic.Add("Id", id);
            RelationEntity entRelation = DbAccess.GetEntity<RelationEntity>(TableName, dic);
            if (entRelation == null)
            {
                return -1;
            }
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
        #region 通过下游卖家mobile获取信息
        /// <summary>
        /// 通过下游卖家mobile获取信息
        /// </summary>
        /// <param name="mobile">卖家mobile</param>
        /// <returns>大于0成功，否则失败</returns>
        /// <remarks>
        /// <list type="bullet">
        /// <item></item>
        /// </list>
        /// </remarks>
        public IResultResponse CheckDownConnector(string mobile,int upConnectorId)
        {
            //检查信息是否存在
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("Mobile", mobile);
            List<ConnectorEntity> entConnectors = DbAccess.GetEntitys<ConnectorEntity>(ConnectorBusiness._TableName, dic,0);
            if (entConnectors == null || entConnectors.Count==0)
            {
                return ResultResponse.ExceptionResult("您尚未成为连接器合作商");
            }
            IResultResponse resultResponse = null;
            foreach (ConnectorEntity entConnector in entConnectors)
            {
                Dictionary<string, object> dic2 = new Dictionary<string, object>();
                dic2.Add("DownConnectorId", entConnector.Id);
                RelationEntity relate = DbAccess.GetEntity<RelationEntity>(TableName, dic2);
                if (relate != null && relate.Id > 0)
                {
                    if (relate.UpConnectorId == upConnectorId)
                    {
                        resultResponse = ResultResponse.ExceptionResult("您已成为该合作商的下游合作商");
                    }
                    else
                    {
                        resultResponse = ResultResponse.ExceptionResult("您已拥有上游合作商，不能再添加");
                    }
                    continue;
                }
                GroupEntity group = (GroupEntity)new GroupBusiness().GetEntity("Id=" + entConnector.GroupId);
                Dictionary<string, object> result = new Dictionary<string, object>();
                result.Add("Id", entConnector.Id);
                result.Add("Domain", group.Domain);
                resultResponse= ResultResponse.GetSuccessResult(result.ToJson());
                break;
            }
            return resultResponse;
        }
        #endregion

        #region 发送短信验证码
        public IResultResponse GetValidateCode(int connectorId,string codeToken, string verifyCode) {
            ConnectorEntity connector = (ConnectorEntity)new ConnectorBusiness().GetEntity("Id=" + connectorId);
            if (connector == null || connector.Id == null || connector.Id < 1)
            {
                return ResultResponse.ExceptionResult("您尚未成为连接器合作商");
            }
            GroupEntity group = (GroupEntity)new GroupBusiness().GetEntity("Id=" + connector.GroupId);
            string mobile = DES.Encode(string.Format("{0}|{1}|{2}|{3}|{4}|{5}", connector.Mobile, connector.Mobile, connector.Mobile, connector.Mobile, connector.Mobile, connector.Mobile));
            IResultResponse res = ApiRequest.GetResponse(group.Domain, "vast.account.supplier.validatecode.get", new Dictionary<string, string>() {

                    {"mobile",mobile },
                    {"VerifyCode",verifyCode },
                    {"CodeToken",codeToken },
                    {"ProprietorId",connector.SellerId.ToString() },
                });
            if (res.Success) {
                return ResultResponse.GetSuccessResult(1);
            }
            return ResultResponse.ExceptionResult(res.Message);
        }
        #endregion

        #region 验证短信验证码，并添加邀请记录
        public IResultResponse ValidateCode(int connectorId,int upConnectorId,string validateCode)
        {
            ConnectorEntity connector = (ConnectorEntity)new ConnectorBusiness().GetEntity("Id=" + connectorId);
            if (connector == null || connector.Id == null || connector.Id < 1)
            {
                return ResultResponse.ExceptionResult("您尚未成为连接器合作商");
            }
            string mobile = DES.Encode(string.Format("{0}|{1}|{2}|{3}|{4}|{5}", connector.Mobile, connector.Mobile, connector.Mobile, connector.Mobile, connector.Mobile, connector.Mobile));

            GroupEntity group = (GroupEntity)new GroupBusiness().GetEntity("Id=" + connector.GroupId);
            IResultResponse res = ApiRequest.GetResponse(group.Domain, "vast.account.supplier.validatecode.validate", new Dictionary<string, string>() {

                    {"Mobile",mobile },
                    {"ValidateCode",validateCode },
                    {"ProprietorId",connector.SellerId.ToString() },
                });
            if (res.Success)
            {
                ConnectorEntity upConnector = (ConnectorEntity)new ConnectorBusiness().GetEntity("Id=" + upConnectorId);
                RelationEntity ent = new RelationEntity();
                ent.UpConnectorId = upConnectorId;
                ent.UpBuyerId = 0;
                ent.DownConnectorId = connectorId;
                ent.Status = 0;//0待处理
                ent.AddTime = DateTime.Now;
                ent.UpdateTime = DateTime.Now;
                ent.InvitedFrom = 0;
                int val = DbAccess.ExecuteInsert(TableName, ent);
                if (val>0) {
                    return ResultResponse.GetSuccessResult("您已成功申请成为该商家的下游合作商，请等待审核");
                }
            }
            return ResultResponse.ExceptionResult(res.Message);
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

        #region 修改全站克隆权限
        public IResultResponse ChangeIsOpen(int connectorId, int id, int status)
        {
            try
            {
                int val = 0;
                ConnectorEntity connector = (ConnectorEntity)new ConnectorBusiness().GetEntity("Id=" + connectorId);
                if (connector == null || Utils.ToInt(connector.Id) == 0)
                {
                    return ResultResponse.ExceptionResult("不存在该上游");
                }
                RelationEntity entRelation = (RelationEntity)new RelationBusiness().GetEntity("Id=" + id);
                if (entRelation == null || Utils.ToInt(entRelation.Id) == 0)
                {
                    return ResultResponse.ExceptionResult("不存在合作关系");
                }
                entRelation.IsOpen = status;
                val = DbAccess.ExecuteUpdate(TableName, entRelation, new string[] { "Id" });
                if (val > 0)
                {
                    return ResultResponse.GetSuccessResult(1);
                }
                else {
                    return ResultResponse.ExceptionResult("修改权限失败");
                }
            }
            catch (Exception ex)
            {
                return ResultResponse.ExceptionResult(ex);
            }
        }
        #endregion
    }
}
