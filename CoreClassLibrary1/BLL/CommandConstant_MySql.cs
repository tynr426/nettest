using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.BLL
{
    /// <summary>
    /// 
    /// </summary>
    public static class CommandConstant
    {
        public const string ConnectorAuth = @"SELECT a.Id,a.Name,a.SellerId,a.Logo,a.Mobile,a.`Status`,a.GroupId,
b.Domain,b.ImageDomain,b.DBId,ifnull(c.UpConnectorId,0) AS UpConnectorId
FROM ict_connector AS a
JOIN ict_group AS b ON(a.GroupId=b.Id)
LEFT JOIN ict_relation AS c ON(a.Id=c.DownConnectorId AND IsDefault=1)
WHERE sellerId={0} and DBId={1}";

        public const string GetConnectorById = @"SELECT a.Id,a.Name,a.SellerId,a.Logo,a.Mobile,a.`Status`,a.GroupId,
b.Domain,b.ImageDomain,b.DBId
FROM ict_connector AS a
JOIN ict_group AS b ON(a.GroupId=b.Id)
WHERE a.Id={0}";
        /// <summary>
        /// 获得上游信息
        /// </summary>
        public const string GetUpConnectorBySellerId = @"SELECT d.Id,d.Name,d.SellerId,d.Logo,d.Mobile,d.`Status`,d.GroupId,
b.Domain,b.ImageDomain,b.DBId,c.UpBuyerId,a.Id as ThirdConnectorId
FROM ict_connector AS a
JOIN ict_group AS b ON(a.GroupId=b.Id) 
JOIN ict_relation AS c ON(a.Id=c.DownConnectorId AND IsDefault=1)
JOIN ict_connector AS d on(c.UpConnectorId=d.Id)
WHERE a.sellerId={0} and DBId={1}";
        /// <summary>
        /// 获得下游信息
        /// </summary>
        public const string GetDownConnectorBySellerId = @"SELECT d.Id,d.Name,d.SellerId,d.Logo,d.Mobile,d.`Status`,d.GroupId,
b.Domain,b.ImageDomain,b.DBId,a.Id as ThirdConnectorId
FROM ict_connector AS a
JOIN ict_group AS b ON(a.GroupId=b.Id) 
JOIN ict_relation AS c ON(a.Id=c.UpConnectorId AND IsDefault=1)
JOIN ict_connector AS d on(c.DownConnectorId=d.Id)
WHERE a.sellerId={0} and DBId={1} and c.UpBuyerId={2}";

    }
}
