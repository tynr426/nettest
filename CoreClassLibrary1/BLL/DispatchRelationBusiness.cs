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
    class DispatchRelationBusiness : CoreBusiness
    {
        #region override Property

        /// <summary>
        /// 常量数据表名
        /// </summary>
        public const string _TableName = _TablePrefix + "Dispatch_Relation";

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

        public int Insert(int disPatchId, OrderRelationEntity orderRelationEntity, Dictionary<string, object> orderDic)
        {
            int downDisPatchId = orderDic.ToInt("DispatchId", 0);
            if (downDisPatchId == 0) throw new Exception("代下单失败");
            DispatchRelationEntity dispatchRelationEntity = new DispatchRelationEntity()
            {
                UpDispatchId=disPatchId,
                OrderRelationId= orderRelationEntity.Id,
                DownDispatchId= downDisPatchId
            };
            return DbAccess.ExecuteUpsert(TableName, dispatchRelationEntity, new string[] { "UpDispatchId", "OrderRelationId", "DownDispatchId"});
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderRelationEntity"></param>
        /// <returns></returns>
        public void GetOrderDispatch(RelationMappingEntity relationMappingEntity)
        {
            DataTable data= DBTable($"OrderRelationId={relationMappingEntity.OrderRelationEntity.Id}");
            if (data == null || data.Rows.Count == 0) return;

            foreach (DataRow dr in data.Rows)
            {
               

                int upDispatchId = Utils.ToInt(dr["UpDispatchId"]);
                int downDispatchId = Utils.ToInt(dr["DownDispatchId"]);

                if (!relationMappingEntity.DispatchMapping.ContainsKey(upDispatchId))
                {
                    relationMappingEntity.DispatchMapping.Add(upDispatchId, downDispatchId);
                }
            }

           
        }
    }
}
