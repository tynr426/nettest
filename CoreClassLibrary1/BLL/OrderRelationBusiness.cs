using Core.ENT;
using ECF;
using ECF.Data.Query;
using Sdk;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace Core.BLL
{
    class OrderRelationBusiness : CoreBusiness
    {
        #region override Property

        /// <summary>
        /// 常量数据表名
        /// </summary>
        public const string _TableName = _TablePrefix + "Order_Relation";

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
        /// <summary>
        /// 插入订单关联信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="oddNumber"></param>
        /// <param name="ConnectorEntity"></param>
        /// <param name="orderDic"></param>
        /// <returns></returns>
        public int Insert(int orderId, string oddNumber, ConnectorRelation ConnectorEntity,Dictionary<string,object> orderDic)
        {
            int upOrderId = orderDic.ToInt("OrderId", 0);
            string upOddNumber = orderDic.ToValue("OddNumber");
            if (upOrderId == 0) throw new Exception("代下单失败");
            OrderRelationEntity orderRelationEntity = new OrderRelationEntity()
            {
                UpConnectorId = ConnectorEntity.Id,
                UpOrderId = upOrderId,
                UpOddNumber = upOddNumber,
                DownConnectorId = ConnectorEntity.ThirdConnectorId,
                DownOrderId = orderId,
                DownOddNumber = oddNumber
            };
            return DbAccess.ExecuteUpsert(TableName, orderRelationEntity, new string[] { "UpConnectorId", "UpOrderId", "DownConnectorId", "DownOrderId" });
        }

        #region GetRelationMappingEntity
        /// <summary>
        /// 获得下游
        /// </summary>
        /// <param name="sellerId"></param>
        /// <param name="dbId"></param>
        /// <returns></returns>
        public RelationMappingEntity GetDownOrder(int sellerId, int dbId,int orderId, int upBuyerId)
        {
            RelationMappingEntity relationMappingEntity = new RelationMappingEntity();
            ConnectorRelation connectorEntity = new ConnectorBusiness().GetDownConnector(sellerId,upBuyerId, dbId);
            if (connectorEntity.Id == null)
            {
                return relationMappingEntity;
            }

            relationMappingEntity.ConnectorMapping.Add(sellerId, Utils.ToInt(connectorEntity.SellerId));

            relationMappingEntity.ConnectorEntity = connectorEntity;

            DataTable data = DBTable($"UpConnectorId={connectorEntity.ThirdConnectorId} and DownConnectorId={connectorEntity.Id} and UpOrderId={orderId}");

            if (data == null || data.Rows.Count == 0)
            {
                return relationMappingEntity;
            }
           

            foreach (DataRow dr in data.Rows)
            {
                OrderRelationEntity orderRelationEntity = new OrderRelationEntity();
                orderRelationEntity.SetValues(dr);

                relationMappingEntity.OrderRelationEntity = orderRelationEntity;

                int upOrderId = Utils.ToInt(orderRelationEntity.UpOrderId);
                int downOrderId = Utils.ToInt(orderRelationEntity.DownOrderId);
                string upOddNumber = orderRelationEntity.UpOddNumber;
                string downOddNumber = orderRelationEntity.DownOddNumber;

                if (!relationMappingEntity.OrderMapping.ContainsKey(upOrderId))
                {
                    relationMappingEntity.OrderMapping.Add(upOrderId, downOrderId);
                }
                if (!relationMappingEntity.OrderOddNumberMapping.ContainsKey(upOddNumber))
                {
                    relationMappingEntity.OrderOddNumberMapping.Add(upOddNumber, downOddNumber);
                }

                new DispatchRelationBusiness().GetOrderDispatch(relationMappingEntity);
            }


            return relationMappingEntity;
        }
        /// <summary>
        /// 获得上游
        /// </summary>
        /// <param name="sellerId"></param>
        /// <param name="dbId"></param>
        /// <returns></returns>
        public RelationMappingEntity GetUpOrder(int sellerId, int dbId, int orderId)
        {
            RelationMappingEntity relationMappingEntity = new RelationMappingEntity();
            ConnectorRelation connectorEntity = new ConnectorBusiness().GetUpConnector(sellerId, dbId);
            if (connectorEntity.Id == null)
            {
                return relationMappingEntity;
            }

            relationMappingEntity.ConnectorMapping.Add(sellerId, Utils.ToInt(connectorEntity.SellerId));

            relationMappingEntity.ConnectorEntity = connectorEntity;

            DataTable data = DBTable($"UpConnectorId={connectorEntity.Id} and DownConnectorId={connectorEntity.ThirdConnectorId} and DownOrderId={orderId}");

            if (data == null || data.Rows.Count == 0)
            {
                return relationMappingEntity;
            }

            foreach (DataRow dr in data.Rows)
            {
                OrderRelationEntity orderRelationEntity = new OrderRelationEntity();
                orderRelationEntity.SetValues(dr);

                relationMappingEntity.OrderRelationEntity = orderRelationEntity;

                int upOrderId = Utils.ToInt(orderRelationEntity.UpOrderId);
                int downOrderId = Utils.ToInt(orderRelationEntity.DownOrderId);
                string upOddNumber = orderRelationEntity.UpOddNumber;
                string downOddNumber = orderRelationEntity.DownOddNumber;

                if (!relationMappingEntity.OrderMapping.ContainsKey(downOrderId))
                {
                    relationMappingEntity.OrderMapping.Add(downOrderId, upOrderId);
                }
                if (!relationMappingEntity.OrderOddNumberMapping.ContainsKey(downOddNumber))
                {
                    relationMappingEntity.OrderOddNumberMapping.Add(downOddNumber, upOddNumber);
                }
                new DispatchRelationBusiness().GetOrderDispatch(relationMappingEntity);
            }
            return relationMappingEntity;
        }
        #endregion
        /// <summary>
        /// 检查订单
        /// </summary>
        /// <param name="sellerId"></param>
        /// <param name="dbId"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public bool CheckOrder(int sellerId, int dbId, int orderId)
        {
            RelationMappingEntity relationMappingEntity = GetUpOrder(sellerId, dbId, orderId);
            return relationMappingEntity.OrderMapping.ContainsKey(orderId);
        }

    }
}
