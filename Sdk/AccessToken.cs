using ECF;
using ECF.Caching;
using ECF.Security;
using ECF.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sdk
{
    /// <summary>
    /// 
    /// </summary>
    public class AccessToken
    {
        #region 属性
        /// <summary>
        /// 连接器id
        /// </summary>
        public int Id { get; internal set; }
        /// <summary>
        /// 卖家id
        /// </summary>
        public int SellerId { get; internal set; }
        /// <summary>
        /// 帐号名称
        /// </summary>
        public string Name { get; internal set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; internal set; }
        /// <summary>
        /// 域名
        /// </summary>
        public string Domain { get; internal set; }

        public int GroupId { get; internal set; }
        /// <summary>
        /// 上游
        /// </summary>
        public int UpConnectorId { get; set; }
        /// <summary>
        /// 数据库Id
        /// </summary>
        public int DBId { get; internal set; }

       
        /// <summary>
        /// 有效期时间戳
        /// </summary>
        public long Expired { get; internal set; }
        #endregion
        /// <summary>
        /// 
        /// </summary>
        private AccessToken()
        {

        }
        #region 生成Token
        /// <summary>
        /// 生成Token
        /// </summary>
        /// <param name="id">连接器id</param>
        /// <param name="sellerId">卖家id</param>
        /// <param name="name">名称</param>
        /// <param name="mobile">手机号</param>
        /// <param name="domain">卖家域名</param>
        /// <param name="groupId">卖家图片域名</param>
        /// <param name="dbId">卖家dbid</param>
        public AccessToken(int id, int sellerId, string name, string mobile, string domain,int groupId,int dbId,int upConnectorId)
        {
            Id = id;
            SellerId = sellerId;
            Name = name;
            Mobile = mobile;
            Domain = domain;
            GroupId = groupId;
            DBId = dbId;
            UpConnectorId = upConnectorId;

        }
        #endregion

        #region 解析Token +static Token ParseToken(string token)
        /// <summary>
        /// 解析Token
        /// </summary>
        /// <param name="token"></param>
        public static AccessToken ParseToken(string token)
        {
            AccessToken tokenEntity = new AccessToken();
            if (!string.IsNullOrWhiteSpace(token))
            {
                try
                {
                    string tokenStr = AES.Decode(token);
                    string[] arr = tokenStr.Split('_');
                    if (arr.Length >= 9)
                    {
                        Type t = tokenEntity.GetType();
                        PropertyInfo[] pis = t.GetProperties();
                        for (int i = 0; i < pis.Length; i++)
                        {
                            pis[i].SetValue(tokenEntity, ReflectionUtil.ChangeType(arr[i], pis[i]), null);
                        }
                    }
                }
                catch
                {
                    tokenEntity = null;
                }
            }
            return tokenEntity;
        }
        #endregion

        #region 转换为Token字符串 +string ToTokenString()
        /// <summary>
        /// 转换为Token字符串
        /// </summary>
        /// <returns></returns>
        public string ToTokenString()
        {
            try
            {
                this.Expired = Utils.ToUnixTime(DateTime.Now.AddMinutes(7*24*60));
                Type t = this.GetType();
                PropertyInfo[] pis = t.GetProperties();
                string tokenStr = string.Empty;
                foreach (PropertyInfo pi in pis)
                {
                    tokenStr += pi.GetValue(this, null) + "_";
                }
                tokenStr = tokenStr.Remove(tokenStr.Length - 1);

                return AES.Encode(tokenStr);
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }
            return "";
        }
        #endregion

        #region 加密成md5 string GetMd5Token(string token)
        /// <summary>
        /// 加密的token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string GetMd5Token(string token)
        {
            return Encrypt.MD532("rwxkj:" + token).ToLower();
        }
        #endregion

        #region 校验token是否被篡改 
        /// <summary>
        /// 校验token是否被篡改
        /// </summary>
        /// <param name="token"></param>
        /// <param name="md5"></param>
        /// <returns></returns>
        public static bool ValidateToken(string token, string md5)
        {
            string md5token = Encrypt.MD532("rwxkj:" + token).ToLower();
            if (md5 != md5token) return false;
            return true;
        }
        #endregion

        #region 产生tokenkey
        /// <summary>
        /// 产生tokenkey
        /// </summary>
        /// <returns></returns>
        public string GenerateKey()
        {
            return string.Format(this.DBId + "_" + this.SellerId + "_"+this.Id);
        }
        #endregion
    }
}
