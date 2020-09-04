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
    class CloneResultBusiness : CoreBusiness
    {
        #region override Property

        /// <summary>
        /// 常量数据表名
        /// </summary>
        public const string _TableName = _TablePrefix + "clone_result";

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
                return new CloneResultEntity();
            }
        }
        #endregion

        public int Insert(Sdk.AccessToken accessToken)
        {
            CloneResultEntity cloneResultEntity = new CloneResultEntity()
            {
                UpConnectorId = accessToken.UpConnectorId,
                DownConnectorId = accessToken.Id,
                DownSellerId = accessToken.SellerId,
                UpSellerId=0,
                Status = 0
            };
            return DbAccess.ExecuteInsert(TableName, cloneResultEntity);
        }
        public DataTable GetCloneData(Sdk.AccessToken accessToken)
        {
           return DbAccess.GetDataTable(TableName, "DownSellerId,CloneResult,Status", $"UpConnectorId={accessToken.UpConnectorId} and DownConnectorId={accessToken.Id} and Status=1","Id desc",1);

        }
    }
}
