using ECF;
using ECF.Caching;
using ECF.Data;
using ECF.Data.Query;
using ECF.Optimize;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Xml;

namespace Sdk.Business
{
    /// <summary>
    /// FullName： <see cref="Sdk.BaseBusiness"/>
    /// 结构化的业务逻辑处理类
    /// Author:  XP
    /// Created: 2011/11/15
    /// </summary>

    public abstract class BaseBusiness : IBusiness
    {
        /// <summary>
        /// 记录此类中已使用的缓存key
        /// By XP-PC 2012/10/22 17:00
        /// </summary>
        public static SecList<string> CacheKeyList = new SecList<string>();

        public static object ListLock = new object();

        #region Monitor

        /// <summary>
        /// 业务逻辑处理监控器队列.
        /// </summary>
        public List<ECF.Optimize.IMonitor> Monitors
        {
            get { return _DbAccess.Monitors; }
        }

        /// <summary>
        /// 重置数据处理监控器对象.
        /// </summary>
        public void ResetMonitor()
        {
            _DbAccess.Monitors.Clear();
        }

        /// <summary>
        /// 数据处理性能监控详情.
        /// </summary>
        public string MonitorDetail
        {
            get
            {
                List<string> lst = new List<string>();

                if (Monitors.Count > 0)
                {
                    long totalTime = 0L;
                    SortedDictionary<string, IMonitor> sortDic = new SortedDictionary<string, IMonitor>();
                    int i = 0;
                    // 对监控器进行耗时倒序处理
                    for (int m = 0; m < Monitors.Count; m++)
                    {
                        IMonitor monitor = Monitors[m];
                        if (monitor.Detail.Count > 0)
                            sortDic.Add(monitor.TotalTimeElapsed + "-" + i, monitor);

                        totalTime = totalTime + monitor.TotalTimeElapsed;
                        i++;
                    }

                    foreach (KeyValuePair<string, IMonitor> pair in sortDic.Reverse())
                    {
                        IMonitor monitor = pair.Value;
                        lst.Add(monitor.Detail.ToJson());
                    }

                    return "{\"Name\":\"数据库访问监控\",\"TotalElapsed\":" + totalTime + ",\"Count\":" + Monitors.Count + ",\"Detail\":[" + string.Join(",", lst) + "]}";
                }

                return "";
            }
        }
        #endregion

        #region 构造方法

        /// <summary>
        /// 构造实例化.
        /// </summary>
        public BaseBusiness()
        {
            _DbAccess = WebCache.Get(Constant.DBKeyCachePrefix + DBKey) as IDBAccess;
            if (_DbAccess == null)
            {
                _DbAccess = DBFactory.GetDBAccess(DBKey);
                if (_DbAccess == null)
                {
                    throw new Exceptions("数据库连接错误,请检查数据库连接!");
                }
                WebCache.Max(Constant.DBKeyCachePrefix + DBKey, _DbAccess);
            }
            else
            {
                if (_DbAccess.DtService == null)
                {
                    _DbAccess = DBFactory.GetDBAccess(DBKey);
                    WebCache.Max(Constant.DBKeyCachePrefix + DBKey, _DbAccess);
                }
            }

            // Map = new Mapping(DBKey, MapKey);
        }

        #endregion

        #region 结构化属性
        /// <summary>
        /// 数据表名
        /// </summary>
        /// <value></value>
        public abstract string TableName { get; }

        /// <summary>
        /// 数据库链接关键字
        /// </summary>
        public abstract string DBKey { get; }

        /// <summary>
        /// 数据库映射关键字
        /// </summary>
        public abstract string MapKey { get; }

        /// <summary>
        /// 缓存关键字
        /// </summary>
        public abstract string CacheKey { get; }

        /// <summary>
        /// 模块名称
        /// </summary>
        public abstract string ModuleName { get; }

        /// <summary>
        /// 获取当前实例化业务逻辑要处理的实体
        /// </summary>
        public abstract IEntity Entity { get; }

        /// <summary>
        /// 数据表前缀
        /// </summary>
        public abstract string TablePrefix { get; }

        #endregion

        #region 字段
        IDBAccess _DbAccess;
        /// <summary>
        /// 数据库访问
        /// </summary>
        public IDBAccess DbAccess
        {
            get
            {
                if (_DbAccess == null)
                {
                    _DbAccess = DBFactory.GetDBAccess(DBKey);
                    if (_DbAccess == null)
                    {
                        throw new Exceptions("数据库连接错误,请检查数据库连接!");
                    }
                    WebCache.Max(Constant.DBKeyCachePrefix + DBKey, _DbAccess);
                }
                else
                {
                    if (_DbAccess.DtService == null)
                    {
                        _DbAccess = DBFactory.GetDBAccess(DBKey);
                        WebCache.Max(Constant.DBKeyCachePrefix + DBKey, _DbAccess);
                    }
                }
                return _DbAccess;
            }
        }


        /// <summary>
        /// 数据库访问服务
        /// </summary>
        public IDBService DbService
        {
            get { return DbAccess.DbService; }
        }


        /// <summary>
        /// 数据库提供者.
        /// </summary>
        public IDBProvider DbProvider
        {
            get { return DbAccess.Provider; }
        }


        #endregion

        #region Insert 向数据库中插入数据

        /// <summary>
        /// 向数据表中插入数据.
        /// </summary>
        /// <param name="xmlDoc">XmlDocument文档.</param>
        /// <returns></returns>
        public virtual int Insert(System.Xml.XmlDocument xmlDoc)
        {
            ClearCache();
            if (ECF.Utils.IsNullOrEmpty(xmlDoc))
                return 0;
            else
            {
                IEntity ent = this.Entity;
                ent.SetValues(xmlDoc);
                return DbAccess.ExecuteInsert(TableName, ent);
            }
        }

        /// <summary>
        /// 向数据表中插入数据.
        /// </summary>
        /// <param name="json">Json格式的数据.</param>
        /// <returns></returns>
        public virtual int Insert(string json)
        {
            ClearCache();
            if (ECF.Utils.IsNullOrEmpty(json))
                return 0;
            else
            {
                IEntity ent = this.Entity;
                ent.SetValues(json);
                return DbAccess.ExecuteInsert(TableName, ent);
            }
        }

        #endregion

        #region Delete 删除数据表中的数据.

        /// <summary>
        /// 删除数据表中的数据,不记录操作日志.
        /// </summary>
        /// <param name="id">唯一标识的id.</param>
        /// <returns></returns>
        public virtual int Delete(int id)
        {
            ClearCache();
            if (id > 0)
            {
                return DbAccess.ExecuteDelete(TableName, id);
            }
            else
                return 0;
        }

        /// <summary>
        /// 删除数据表中的数据.
        /// </summary>
        /// <param name="id">唯一标识的id.</param>
        /// <param name="paras">记录日志的参数组.</param>
        /// <returns></returns>
        public virtual int Delete(int id, object[] paras)
        {
            ClearCache();
            if (id > 0)
            {
                //if (paras != null)
                //{
                //    DataEngine.AddLogs(DataEngine.OpeType.Delete, paras);
                //}
                return DbAccess.ExecuteDelete(TableName, id);
            }
            else
                return 0;
        }

        /// <summary>
        /// 删除数据表中的数据.
        /// </summary>
        /// <param name="condition">条件.</param>
        /// <param name="parameters">数据参数</param>
        /// <returns></returns>
        public virtual int Delete(string condition, System.Data.Common.DbParameter[] parameters = null)
        {
            if (ECF.Utils.IsNullOrEmpty(condition))
                return 0;
            else
            {
                ClearCache();
                return DbAccess.ExecuteDelete(TableName, condition, parameters);
            }
        }

        /// <summary>
        /// 批量进行数据物理删除
        /// </summary>
        /// <param name="ids">需要删除的id集.</param>
        /// <param name="paras">记录删除日志参数.</param>
        /// <returns>返回影响删除日志的条数</returns>
        public virtual int Delete(string[] ids, object[] paras)
        {
            if (ids.Length > 0)
            {
                ClearCache();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < ids.Length; i++)
                {
                    int id = ECF.Utils.ToInt(ids[i]);
                    if (id > 0)
                        sb.Append("DELETE FROM " + TableName + " WHERE Id=" + id + ";");
                }
                //if (paras != null)
                //{
                //    DataEngine.AddLogs(DataEngine.OpeType.BatDelete, paras);
                //}
                return DbService.ExecuteNonQuery(sb.ToString());
            }
            return 0;
        }
        #endregion

        #region Update 更新数据库记录

        /// <summary>
        /// 更新数据库记录,支持Json格式的数据.
        /// </summary>
        /// <param name="json">Json格式的数据.</param>
        /// <param name="id">数据表中唯一标识.</param>
        /// <returns></returns>
        public virtual int Update(string json, int id)
        {
            ClearCache();
            if (ECF.Utils.IsNullOrEmpty(json) || id < 1)
                return 0;
            else
            {
                IEntity ent = this.Entity;
                ent.SetValues(json);
                return DbAccess.ExecuteUpdate(TableName, ent, "id".Split(','));
            }
        }

        /// <summary>
        /// 更新数据库记录,支持Xmldocument.
        /// </summary>
        /// <param name="xmlDoc">XmlDocument文档.</param>
        /// <param name="id">数据表中唯一标识.</param>
        /// <returns></returns>
        public virtual int Update(System.Xml.XmlDocument xmlDoc, int id)
        {
            ClearCache();
            if (ECF.Utils.IsNullOrEmpty(xmlDoc) || id < 1)
                return 0;
            else
            {
                IEntity ent = this.Entity;
                ent.SetValues(xmlDoc);
                return DbAccess.ExecuteUpdate(TableName, ent, "id".Split(','));
            }
        }

        /// <summary>
        /// 更新数据库记录,支持Json格式的数据.
        /// </summary>
        /// <param name="json">Json格式的数据.</param>
        /// <param name="condition">更新条件.</param>
        /// <returns></returns>
        public virtual int Update(string json, string[] fields)
        {
            ClearCache();
            if (ECF.Utils.IsNullOrEmpty(json) || fields.Length < 1)
                return 0;
            else
            {
                //this.Entity
                IEntity ent = this.Entity;
                ent.SetValues(json);
                return DbAccess.ExecuteUpdate(TableName, ent, fields);
            }
        }

        /// <summary>
        /// 更新数据库记录,支持Xmldocument.
        /// </summary>
        /// <param name="xmlDoc">XmlDocument文档.</param>
        /// <param name="condition">更新条件.</param>
        /// <returns></returns>
        public virtual int Update(System.Xml.XmlDocument xmlDoc, string[] fields)
        {
            ClearCache();
            if (ECF.Utils.IsNullOrEmpty(xmlDoc) || fields.Length < 1)
                return 0;
            else
            {
                IEntity ent = this.Entity;
                ent.SetValues(xmlDoc);
                return DbAccess.ExecuteUpdate(TableName, ent, fields);
            }
        }

        #endregion

        #region DetailXml 获取记录行的详细信息
        /// <summary>
        /// 获取记录行的详细信息.
        /// </summary>
        /// <param name="id">自动编号的id.</param>
        /// <returns>
        /// 返回Xml模式的数据
        /// </returns>
        public virtual string DetailXml(int id)
        {
            if (id > 0)
                return DBHelper.Table2Xml(DBTable("Id=" + id)).ToString();
            else
                return "";
        }

        /// <summary>
        /// 获取记录行的详细信息.
        /// </summary>
        /// <param name="condition">获取详细的条件.</param>
        /// <returns>
        /// 返回Xml模式的数据
        /// </returns>
        public virtual string DetailXml(string condition, DbParameter[] parameters = null)
        {
            if (ECF.Utils.IsNullOrEmpty(condition))
                return "";
            else
                return DBHelper.Table2Xml(DBTable(condition, parameters)).ToString();


        }

        /// <summary>
        /// 获取记录行的详细信息.
        /// </summary>
        /// <param name="condition">获取详细的条件.</param>
        /// <returns>
        /// 返回Json模式的数据
        /// </returns>
        public string DetailXml(ConditionParameter[] conditions)
        {
            if (conditions != null && conditions.Length > 0)
            {
                return DBHelper.Table2Xml(DBTable(conditions));
            }
            return string.Empty;
        }
        #endregion

        #region DetailJson 获取记录行的详细信息

        /// <summary>
        /// 获取记录行的详细信息.
        /// </summary>
        /// <param name="id">自动编号的id.</param>
        /// <returns>
        /// 返回Json模式的数据
        /// </returns>
        public virtual string DetailJson(int id)
        {
            if (id > 0)
                return DBHelper.TableToJson(DBTable("Id=" + id)).ToString();
            else
                return "";
        }

        /// <summary>
        /// 获取记录行的详细信息.
        /// </summary>
        /// <param name="condition">获取详细的条件.</param>
        /// <returns>
        /// 返回Json模式的数据
        /// </returns>
        public virtual string DetailJson(string condition, System.Data.Common.DbParameter[] parameters = null)
        {
            if (!ECF.Utils.IsNullOrEmpty(condition))
            {
                if (parameters != null && parameters.Length > 0)
                {
                    return DBHelper.TableToJson(DBTable(0, condition, null, parameters));
                }
                else
                {
                    return DBHelper.TableToJson(DBTable(condition)).ToString();
                }
            }
            else
                return "";
        }

        /// <summary>
        /// 获取记录行的详细信息.
        /// </summary>
        /// <param name="condition">获取详细的条件.</param>
        /// <returns>
        /// 返回Json模式的数据
        /// </returns>
        public string DetailJson(ConditionParameter[] conditions)
        {
            if (conditions != null && conditions.Length > 0)
            {
                return DBHelper.TableToJson(DBTable(conditions));
            }
            return string.Empty;
        }

        #endregion

        #region GetEntity 获取已赋值的实体

        /// <summary>
        /// 获取已附值的实体.
        /// </summary>
        /// <param name="id">数据表标识.</param>
        /// <returns></returns>
        public virtual ECF.IEntity GetEntity(int id)
        {
            if (id > 0)
            {
                string sql = DbProvider.QueryCommandText(1, TableName, "Id=" + id, null);
                DataTable dt = DbAccess.GetDataTable(sql);
                if (dt!= null && dt.Rows.Count > 0)
                    return (IEntity)EntityUtil.SetEntityData(this.Entity, dt);
            }
            return null;
        }


        /// <summary>
        /// 根据带参数的条件获取实体.
        /// </summary>
        /// <param name="condition">带参数名的条件字符串.</param>
        /// <param name="parameters">执行Sql的数据库参数.</param>
        /// <returns></returns>
        public virtual ECF.IEntity GetEntity(string condition, DbParameter[] parameters = null)
        {
            if (!ECF.Utils.IsNullOrEmpty(condition))
            {
                DataTable dt = DBTable(1, condition, null, parameters);
                if (dt.Rows.Count > 0)
                    return (IEntity)EntityUtil.SetEntityData(this.Entity, dt);
            }
            return this.Entity;
        }

        /// <summary>
        /// 根据带参数的条件获取实体.
        /// </summary>
        /// <param name="tableName">表名.</param>
        /// <param name="condition">带参数名的条件字符串.</param>
        /// <param name="parameters">执行Sql的数据库参数.</param>
        /// <param name="orderby">排序条件.</param>
        /// <param name="fields">查询返回字段.</param>
        /// <returns></returns>
        public virtual ECF.IEntity GetEntity(string tableName, string condition, DbParameter[] parameters = null, string orderby = null, string fields = "*")
        {
            if (!ECF.Utils.IsNullOrEmpty(tableName))
            {
                string sql = DbProvider.QueryCommandText(1, fields, tableName, condition, orderby);
                DataTable dt = DbAccess.GetDataTable(sql, parameters);
                if (dt.Rows.Count > 0)
                    return (IEntity)EntityUtil.SetEntityData(this.Entity, dt);
            }
            return this.Entity;
        }

        /// <summary>
        /// 根据条件组获取实体.
        /// </summary>
        /// <param name="conditions">条件参数组.</param>
        /// <returns></returns>
        public virtual ECF.IEntity GetEntity(ConditionParameter[] conditions)
        {
            if (conditions.Length > 0)
            {
                DataTable dt = DBTable(conditions);
                if (dt.Rows.Count > 0)
                    return (IEntity)EntityUtil.SetEntityData(this.Entity, dt);
            }
            return this.Entity;
        }

        public virtual ECF.IEntity GetEntity(Dictionary<string, object> conditions)
        {
            if (conditions != null && conditions.Count > 0)
            {
                DataTable dt = DBTable(conditions);
                if (dt.Rows.Count > 0)
                    return (IEntity)EntityUtil.SetEntityData(this.Entity, dt);
            }
            return this.Entity;
        }

        #endregion

        #region GetDataRow 获取数据行
        /// <summary>
        /// 根据Id获取数据行.
        /// </summary>
        /// <param name="id">数据表标识.</param>
        /// <returns></returns>
        public virtual DataRow GetDataRow(int id)
        {
            if (id > 0)
            {
                DataTable dt = DBTable("Id=" + id);
                if (dt.Rows.Count > 0)
                    return dt.Rows[0];
            }
            return null;
        }

        /// <summary>
        /// 根据Id获取数据行.
        /// </summary>
        /// <param name="id">查询条件.</param>
        /// <returns></returns>
        public virtual DataRow GetDataRow(string condition, DbParameter[] parameters = null)
        {
            DataTable dt = DBTable(1, condition, null, parameters);
            if (dt.Rows.Count > 0)
                return dt.Rows[0];

            return null;

        }

        /// <summary>
        /// 根据Id获取数据行.
        /// </summary>
        /// <param name="condParameter">条件参数.</param>
        /// <returns></returns>
        public DataRow GetDataRow(ConditionParameter[] condParameter)
        {
            DataTable dt = DBTable(condParameter);
            if (dt.Rows.Count > 0)
                return dt.Rows[0];

            return null;
        }

        #endregion

        #region DBTable 获取表里的所有数据

        /// <summary>
        /// 根据条件参数组获取数据表.
        /// </summary>
        /// <param name="conditions">条件参数组.</param>
        /// <param name="orderBy">显示顺序.</param>
        /// <param name="rows">需要取数据的行数.</param>
        /// <returns></returns>
        public virtual System.Data.DataTable DBTable(ConditionParameter[] conditions, string orderBy = "", int rows = 0)
        {

            DbParameter[] parameters = null;
            string sql = DbProvider.QueryCommandText(rows, TableName, "*", conditions, orderBy, out parameters);
            return DbAccess.GetDataTable(sql, parameters);
        }

        /// <summary>
        /// 根据条件获取数据表.
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="orderBy">The order by.</param>
        /// <returns></returns>
        public virtual System.Data.DataTable DBTable(int rows, string condition, string orderBy)
        {
            DbParameter[] parameters = null;
            return DBTable(rows, condition, orderBy, parameters);
        }

        public virtual System.Data.DataTable DBTable(int rows, string condition, string orderBy, Dictionary<string, object> roles)
        {
            DbParameter[] parameters = null;
            if (roles != null && roles.Count > 0)
            {
                condition += " AND " + DbProvider.GetCondition(roles, out parameters, "");
            }
            return DBTable(rows, condition, orderBy, parameters);

        }

        /// <summary>
        /// 根据条件获取数据表.
        /// </summary>
        /// <param name="rows">行数.</param>
        /// <param name="condition">条件.</param>
        /// <param name="orderBy">排序.</param>
        /// <param name="parameters">数据参数.</param>
        /// <returns></returns>
        public virtual System.Data.DataTable DBTable(int rows, string condition, string orderBy, DbParameter[] parameters = null)
        {
            string sql = DbProvider.QueryCommandText(rows, TableName, condition, orderBy);
            if (parameters != null && parameters.Length > 0)
                return DbAccess.GetDataTable(sql, parameters);
            else
                return DbAccess.GetDataTable(sql);
        }

        /// <summary>
        /// 根据条件获取数据表.
        /// </summary>
        /// <param name="condition">条件.</param>
        /// <param name="parameters">数据参数.</param>
        /// <returns></returns>
        public virtual DataTable DBTable(string condition, DbParameter[] parameters = null)
        {
            return DBTable(0, condition, null, parameters);
        }

        /// <summary>
        /// 根据条件获取数据表.
        /// </summary>
        /// <param name="fields">查询返回字段.</param>
        /// <param name="condition">条件.</param>
        /// <param name="parameters">数据参数.</param>
        /// <returns></returns>
        public virtual DataTable DBTable(string fields, string condition, DbParameter[] parameters)
        {
            string sql = DbProvider.QueryCommandText(0, fields, TableName, condition, null);
            return DbAccess.DbService.ExecuteDataTable(sql, parameters);
        }

        /// <summary>
        /// 查询返回数据表
        /// </summary>
        /// <param name="condition">查询条件.</param>
        /// <param name="fields">查询返回字段.</param>
        /// <param name="ident">if set to <c>true</c> [ident].</param>
        /// <returns>
        /// System.Data.DataTable
        /// </returns>
        public virtual System.Data.DataTable DBTable(string condition, string fields = null, bool ident = false)
        {
            string sql = DbProvider.QueryCommandText(0, fields, TableName, condition, null);
            return DbAccess.DbService.ExecuteDataTable(sql, ident);
        }

        /// <summary>
        /// 获取表里的所有数据
        /// </summary>
        /// <param name="fields">The fields.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="orderby">The orderby.</param>
        /// <returns>
        /// System.Data.DataTable
        /// </returns>
        public virtual System.Data.DataTable DBTable(string fields, string condition, string orderby)
        {
            string sql = DbProvider.QueryCommandText(0, fields, TableName, condition, orderby);
            return DbAccess.DbService.ExecuteDataTable(sql);
        }

        /// <summary>
        /// 根据条件获取数据表.
        /// </summary>
        /// <param name="condition">条件.</param>
        /// <returns></returns>
        public virtual DataTable DBTable(string condition)
        {
            DbParameter[] parameters = null;
            return DBTable(0, condition, "", parameters);
        }

        /// <summary>
        /// 获取表里的所有数据
        /// </summary>
        /// <returns>
        /// System.Data.DataTable
        /// </returns>
        public virtual System.Data.DataTable DBTable()
        {
            return DBTable(0, null, null);
        }


        /// <summary>
        /// Databases the table.
        /// </summary>
        /// <param name="conditions">The conditions.</param>
        /// <param name="orderby">The orderby.</param>
        /// <param name="rows">The rows.</param>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
        public virtual System.Data.DataTable DBTable(Dictionary<string, object> conditions, string orderby = "", int rows = 0, string[] fields = null)
        {
            string fds = (fields != null ? string.Join(",", fields) : string.Empty);

            return DbAccess.GetDataTable(TableName, conditions, orderby, rows, fds);
        }

        #endregion

        #region LogicDelete 数据逻辑删除
        /// <summary>
        /// 数据逻辑删除，将表中的Status（状态）值修改为-1
        /// </summary>
        /// <param name="id">待更新的id.</param>
        /// <returns></returns>
        public virtual int LogicDelete(int id)
        {
            return LogicDelete(id, null);
        }
        /// <summary>
        /// 数据逻辑删除，将表中的Status（状态）值修改为-1
        /// </summary>
        /// <param name="id">唯一标识的id.</param>
        /// <param name="paras">The paras.</param>
        /// <returns>
        /// System.Int32
        /// </returns>
        public virtual int LogicDelete(int id, object[] paras)
        {
            ClearCache();
            if (id > 0)
            {
                string sql = "UPDATE " + TableName + " SET Status=-1 WHERE Id=" + id;
                
                return DbAccess.DbService.ExecuteNonQuery(sql);
            }
            return 0;
        }

        public int LogicDelete(string condition, DbParameter[] paras = null)
        {
            ClearCache();
            string sql = "UPDATE " + TableName + " SET Status=-1 WHERE " + condition;
            
            return DbAccess.DbService.ExecuteNonQuery(sql);
        }

        #endregion

        #region ClearCache 缓存处理

        /// <summary>
        /// 清除缓存
        /// </summary>
        public virtual void ClearCache()
        {
            try
            {
                // 删除缓存关键字对应的Key
                WebCache.Remove(CacheKey);

                //清除带这个key为前缀的所有缓存
                WebCache.Clear(CacheKey);


                for (int i = 0; i < CacheKeyList.Count; i++)
                {
                    string key = CacheKeyList[i];
                    if (key.IndexOf(CacheKey) > -1)
                    {
                        WebCache.Remove(key);
                        //lock(listLock){
                        // CacheKeyList.Remove(key);
                        //}
                    }
                }

                // 清除所有以缓存关键字开头的Key
                WebCache.Clear(CacheKey);
            }
            catch (Exception ex)
            {
                new Exceptions(ex);
            }
        }



        #region SetCache 设置缓存
        /// <summary>
        /// 设置缓存.
        /// </summary>
        /// <param name="ocache">需要缓存的对像.</param>
        public void SetCache(object ocache)
        {
            WebCache.Max(CacheKey, ocache);
        }

        /// <summary>
        /// 设置缓存.
        /// </summary>
        /// <param name="key">缓存关键字.</param>
        /// <param name="ocache">需要缓存的对像.</param>
        public void SetCache(string key, object ocache)
        {
            WebCache.Max(key, ocache);
        }
        #endregion

        #region GetCache 获取缓存的对象
        /// <summary>
        /// 获取缓存的对象.
        /// </summary>
        /// <param name="key">缓存关键字.</param>
        /// <returns></returns>
        public object GetCache(string key)
        {
            return WebCache.Get(key);
        }

        #endregion

        #endregion

        #region LogicDelete 批量进行数据物理删除
        /// <summary>
        /// 数据逻辑删除，将表中的Status（状态）值修改为-1
        /// </summary>
        /// <param name="condition">逻辑删除的条件.</param>
        /// <returns>
        /// System.Int32
        /// </returns>
        public virtual int LogicDelete(string condition)
        {
            ClearCache();
            if (ECF.Utils.IsNullOrEmpty(condition))
                return 0;

            string sql = "UPDATE " + TableName + " SET Status=-1 WHERE " + condition;

            return DbAccess.DbService.ExecuteNonQuery(sql);
        }

        /// <summary>
        /// 批量进行数据逻辑删除
        /// </summary>
        /// <param name="ids">需要删除的id集.</param>
        /// <param name="paras">记录删除日志参数.</param>
        /// <returns>返回影响删除日志的条数</returns>
        public virtual int LogicDelete(string[] ids, object[] paras)
        {
            ClearCache();
            if (ids.Length > 0)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < ids.Length; i++)
                {
                    int id = ECF.Utils.ToInt(ids[i]);
                    if (id > 0)
                        sb.Append("UPDATE " + TableName + " SET Status=-1 WHERE Id=" + id + ";");
                }
                
                return DbService.ExecuteNonQuery(sb.ToString());
            }
            return 0;
        }
        #endregion

        #region StatusUpdate 数据库中状态更新
        /// <summary>
        /// 数据库中状态更新
        /// </summary>
        /// <param name="status">要更新的状态值.</param>
        /// <param name="ids">待更新的id.</param>
        /// <param name="paras">记录日志的参数集.</param>
        /// <returns></returns>
        public virtual int StatusUpdate(int status, int id, object[] paras)
        {
            ClearCache();
            if (id > 0)
            {
                string sql = "UPDATE " + TableName + " SET Status=" + status + " WHERE Id=" + id;
                
                return DbAccess.DbService.ExecuteNonQuery(sql);
            }
            return 0;
        }

        /// <summary>
        /// 数据库中状态更新
        /// </summary>
        /// <param name="status">要更新的状态</param>
        /// <param name="condition">条件组合，用于限定StoreId、Id等</param>
        /// <param name="paras">记录日志的参数集</param>
        /// <returns></returns>
        public virtual int StatusUpdate(int status, string codition, object[] paras, DbParameter[] param = null)
        {
            ClearCache();
            if (!string.IsNullOrEmpty(codition))
            {
                string sql = "UPDATE " + TableName + " SET Status=" + status + " WHERE " + codition;
                
                return DbAccess.DbService.ExecuteNonQuery(sql, param);
            }
            return 0;
        }
        /// <summary>
        /// 数据库中状态更新
        /// </summary>
        /// <param name="status">要更新的状态值.</param>
        /// <param name="ids">待更新的id集.</param>
        /// <param name="paras">记录日志的参数集.</param>
        /// <returns></returns>
        public virtual int StatusUpdate(int status, string[] ids, object[] paras)
        {
            ClearCache();
            if (ids.Length > 0)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < ids.Length; i++)
                {
                    int id = ECF.Utils.ToInt(ids[i]);
                    if (id > 0)
                        sb.Append("UPDATE " + TableName + " SET Status=" + status + " WHERE Id=" + id + ";");
                }
                
                return DbService.ExecuteNonQuery(sb.ToString());
            }
            return 0;
        }

        /// <summary>
        /// 数据库中状态更新(还原)
        /// </summary>
        /// <param name="status">要更新的状态值.</param>
        /// <param name="ids">待更新的id.</param>
        /// <param name="paras">记录日志的参数集.</param>
        /// <returns></returns>
        public virtual int StatusUpdate(int status, int id, object[] paras, bool revert)
        {
            ClearCache();
            if (id > 0)
            {
                string sql = "UPDATE " + TableName + " SET Status=" + status + " WHERE Id=" + id;
                
                return DbAccess.DbService.ExecuteNonQuery(sql);
            }
            return 0;
        }
        /// <summary>
        /// 数据库中状态更新(还原)
        /// </summary>
        /// <param name="status">要更新的状态值.</param>
        /// <param name="ids">待更新的id集.</param>
        /// <param name="paras">记录日志的参数集.</param>
        /// <returns></returns>
        public virtual int StatusUpdate(int status, string[] ids, object[] paras, bool revert)
        {
            ClearCache();
            if (ids.Length > 0)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < ids.Length; i++)
                {
                    int id = ECF.Utils.ToInt(ids[i]);
                    if (id > 0)
                        sb.Append("UPDATE " + TableName + " SET Status=" + status + " WHERE Id=" + id + ";");
                }
                
                return DbService.ExecuteNonQuery(sb.ToString());
            }
            return 0;
        }
        #endregion

        #region Adder 对指定表，指定字段执行加一计数，返回计数后的值

        /// <summary>
        /// 对指定表，指定字段执行加一计数，返回计数后的值。条件由ConditionExpress指定。
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="condition">The condition.</param>
        /// <returns>返回计数后的值。</returns>
        public int Adder(string fieldName, string condition)
        {
            try
            {
                string upSql = "UPDATE " + TableName + " SET " + fieldName + "=( isnull(" + fieldName + ", 0)+1) ";
                string sql = "select " + fieldName + " from  " + TableName;
                if (!String.IsNullOrEmpty(condition))
                {
                    upSql += " where " + condition;
                    sql += " where " + condition;

                    DataTable dt = DbAccess.GetDataTable(upSql + ";" + sql);
                    if (dt.Rows.Count > 0)
                        return Convert.ToInt32(dt.Rows[0][0]);

                }
                return 1;
            }
            catch
            {
                return 1;
            }
        }

        /// <summary>
        /// 对指定表，指定字段执行减一计数，返回计数后的值。条件由ConditionExpress指定.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="condition">The condition.</param>
        /// <returns></returns>
        public int Subtract(string fieldName, string condition)
        {
            try
            {
                string upSql = "UPDATE " + TableName + " SET " + fieldName + "=( isnull(" + fieldName + ", 0)-1) ";
                string sql = "select " + fieldName + " from  " + TableName;
                if (!String.IsNullOrEmpty(condition))
                {
                    upSql += " where " + condition;
                    sql += " where " + condition;

                    DataTable dt = DbAccess.GetDataTable(upSql + ";" + sql);
                    if (dt.Rows.Count > 0)
                        return Convert.ToInt32(dt.Rows[0][0]);

                }
                return 1;
            }
            catch
            {
                return 1;
            }
        }

        #endregion

        #region GetValue 获取指定字段和指定条件的第一行第一列的值
        /// <summary>
        /// 获取指定字段和指定条件的第一行第一列的值
        /// </summary>
        /// <param name="tableName">表名.</param>
        /// <param name="fieldName">字段名.</param>
        /// <param name="condition">条件.</param>
        /// <returns>
        /// System.Object
        /// </returns>
        public virtual object GetValue(string tableName, string fieldName, string condition)
        {
            return DbAccess.GetValue(tableName, fieldName, condition, null, null);
        }
        public virtual int GetInt(string sql)
        {
            int a = 0;
            int.TryParse(GetValue(sql), out a);
            return a;
        }
        public virtual string GetValue(string sql)
        {
            DataTable dt = DbAccess.GetDataTable(sql);
            if (dt != null && dt.Rows.Count == 1)
            {
                return dt.Rows[0][dt.Columns[0].ToString()].ToString();
            }
            else
            {
                return "";
            }
        }
        /// <summary>
        /// 获取指定字段和指定条件的第一行第一列的值
        /// </summary>
        /// <param name="tableName">表名.</param>
        /// <param name="fieldName">字段名.</param>
        /// <param name="condition">条件.</param>
        ///  <param name="orderby">排序</param>
        /// <returns>
        /// System.Object
        /// </returns>
        public virtual object GetValue(string tableName, string fieldName, string condition, string orderby)
        {
            return DbAccess.GetValue(tableName, fieldName, condition, orderby, null);
        }

        /// <summary>
        /// 获取指定字段和指定条件的第一行第一列的值
        /// </summary>
        /// <param name="fieldName">字段名.</param>
        /// <param name="condition">条件.</param>
        /// <returns>
        /// System.Object
        /// </returns>
        public virtual object GetValue(string fieldName, string condition)
        {
            return GetValue(TableName, fieldName, condition);
        }

        /// <summary>
        /// 获取指定字段和指定条件的第一行第一列的值
        /// </summary>
        /// <param name="fieldName">字段名.</param>
        /// <param name="condition">条件.</param>
        /// <param name="parameters"></param>
        /// <returns>
        /// System.Object
        /// </returns>
        public object GetValue(string fieldName, string condition, DbParameter[] parameters = null)
        {
            return DbAccess.GetValue(TableName, fieldName, condition, null, parameters);
        }

        /// <summary>
        /// 获取指定字段和指定条件的第一行第一列的值.
        /// </summary>
        /// <param name="fieldName">字段名.</param>
        /// <param name="condParameter">条件参数集.</param>
        /// <returns></returns>
        public object GetValue(string fieldName, ConditionParameter[] condParameter)
        {
            return DbAccess.GetValue(TableName, fieldName, condParameter, null);
        }

        /// <summary>
        /// 获取指定字段和指定条件的第一行第一列的值.
        /// </summary>
        /// <param name="fieldName">字段名.</param>
        /// <param name="condParameter">条件参数集.</param>
        /// <param name="orderBy">排序.</param>
        /// <returns></returns>
        public object GetValue(string fieldName, ConditionParameter[] condParameter, string orderBy)
        {
            return DbAccess.GetValue(TableName, fieldName, condParameter, orderBy);
        }

        #endregion

        #region Count 获取表的数据行数,用于对数据统计
        /// <summary>
        /// 获取表的数据行数,用于对数据统计
        /// </summary>
        /// <param name="condition">条件.</param>
        /// <returns></returns>
        public int Count(string condition, DbParameter[] parameters = null)
        {
            if (parameters != null && parameters.Length > 0)
            {
                return ECF.Utils.ToInt(DbAccess.GetValue(TableName, "Count(0)", condition, null, parameters));
            }
            return ECF.Utils.ToInt(DbAccess.GetValue(TableName, "Count(0)", condition, null, null));
        }
        #endregion


        #region 获取分页数据 +PagingResult GetPageTable(PagingQuery pagingQuery)

        

        #region 解析查询条件 +virtual string ParseCondition(List<Condition> conditions, ref Dictionary<string, object> queryParams)
        /// <summary>
        /// 解析查询条件 by hfs 20170405
        /// </summary>
        /// <param name="conditions"></param>
        /// <param name="queryParams"></param>
        /// <returns></returns>
        public virtual string ParseCondition(List<Condition> conditions, ref Dictionary<string, object> queryParams)
        {
            string condition = "1=1";
            #region 处理查询条件
            foreach (Condition c in conditions)
            {
                if (!string.IsNullOrEmpty(c.Value))
                {
                    condition += c.ToWhereString();
                }
            }
            #endregion
            return condition;
        }
        #endregion

        #region 初始化GetPageTable要返回的字段 +virtual Dictionary<string, Field> InitFields(ref Dictionary<string, object> queryParams)
        /// <summary>
        /// 初始化GetPageTable要返回的字段.需要注意的是:最终返回结果是根据pagingQuery.Fields的值返回的. by hfs 20170417
        /// </summary>
        /// <param name="queryParams"></param>
        /// <returns></returns>
        public virtual Dictionary<string, Field> InitFields(ref Dictionary<string, object> queryParams)
        {
            Dictionary<string, Field> fields = new Dictionary<string, Field>(StringComparer.OrdinalIgnoreCase);
            //fields.Add("*", new ECF.Data.Query.Field("*", "*"));
            //fields.Add("Id", new ECF.Data.Query.Field("Id", "Id"));
            //fields.Add("Status", new ECF.Data.Query.Field("Status", "Status"));
            #region 自定义字段
            //fields.Add("Detail", new ECF.Data.Query.Field("Detail", typeof(DataTable)));
            //fields.Add("Goods", new ECF.Data.Query.Field("Goods", typeof(DataTable)));
            #endregion
            return fields;
        }
        #endregion

        #region 初始化GetPageTable要返回的合计字段（不包含总记录数字段） +virtual Dictionary<string, Field> InitSumFields(ref Dictionary<string, object> queryParams)
        /// <summary>
        /// 初始化GetPageTable要返回的合计字段（不包含总记录数字段）
        /// </summary>
        /// <param name="queryParams"></param>
        /// <returns></returns>
        public virtual Dictionary<string, Field> InitSumFields(ref Dictionary<string, object> queryParams)
        {
            return new Dictionary<string, Field>();
        }
        #endregion

        #region 获取GetPageTable要查询的From字符串 +virtual string GetFromStr(ref Dictionary<string, object> queryParams)
        /// <summary>
        /// 获取GetPageTable要查询的From字符串 by hfs 20170405
        /// </summary>
        /// <param name="queryParams"></param>
        /// <returns></returns>
        public virtual string GetFromStr(ref Dictionary<string, object> queryParams)
        {
            return TableName;
        }
        #endregion

        #region 初始化GetPageTable的分组字段 +virtual IList<Field> InitGroupFields(ref Dictionary<string, object> queryParams)
        /// <summary>
        /// 初始化GetPageTable的分组字段
        /// </summary>
        /// <param name="queryParams"></param>
        /// <returns></returns>
        public virtual Dictionary<string, Field> InitGroupFields(ref Dictionary<string, object> queryParams)
        {
            Dictionary<string, Field> groupFields = new Dictionary<string, Field>(StringComparer.OrdinalIgnoreCase);
            //groupFields.Add("Id", new Field("Id", "Id"));
            return groupFields;
        }
        #endregion

        #region 解析排序条件 +virtual string ParseOrderby(List<Orderby> orderbys, ref Dictionary<string, object> queryParams)
        /// <summary>
        /// 解析排序条件 by hfs 20170405
        /// </summary>
        /// <param name="orderbys"></param>
        /// <param name="queryParams"></param>
        /// <returns></returns>
        public virtual string ParseOrderby(List<Orderby> orderbys, ref Dictionary<string, object> queryParams)
        {
            string orderby;
            #region 处理排序
            List<string> sortby = new List<string>();
            foreach (Orderby ob in orderbys)
            {
                sortby.Add(ob.ToString());
            }
            orderby = sortby.Count > 0 ? "order by " + string.Join(",", sortby) : "";
            #endregion
            return orderby;
        }
        #endregion

        #region 加载自定义字段的数据 +virtual void LoadCustomFieldData(DataTable dt, ref Dictionary<string, object> queryParams)
        /// <summary>
        /// 加载自定义字段的数据 by hfs 20170405
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="queryParams"></param>
        public virtual void LoadCustomFieldData(DataTable dt, ref Dictionary<string, object> queryParams) { }
        #endregion

        #region 解析字段池 -string ParseFields(string fields, Dictionary<string, Field> fieldList, ref Dictionary<string, object> queryParams, ref Dictionary<string, Type> customFields)
        /// <summary>
        /// 解析字段池 by hfs 20170405
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="fieldList"></param>
        /// <param name="queryParams">查询条件参数集合</param>
        /// <param name="customFields">自定义字段容器</param>
        /// <returns></returns>
        private string ParseFields(string fields, Dictionary<string, Field> fieldList, ref Dictionary<string, object> queryParams, ref Dictionary<string, Type> customFields)
        {
            List<string> columns = new List<string>();
            if (!string.IsNullOrWhiteSpace(fields) && fieldList != null && fieldList.Count > 0)
            {
                string[] fieldKeys = fields.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string str in fieldKeys)
                {
                    //去除字符串中的回车，换行符，制表符，首尾空格
                    string key = str.Replace("\r", "").Replace("\n", "").Replace("\t", "").Trim();
                    if (fieldList.ContainsKey(key))
                    {
                        Field field = fieldList[key];
                        if (field == null) continue;
                        if (field.IsCustom)
                        {
                            customFields[key] = (Type)field.Autonym;
                        }
                        else if (key == "*")
                        {
                            columns.Add(field.Autonym + "");
                        }
                        else
                        {
                            columns.Add(field.Autonym + " as " + key);

                            //拼装链表控制器
                            if (field.TableAlias != null && field.TableAlias.Count > 0)
                            {
                                foreach (string tableAlias in field.TableAlias)
                                {
                                    queryParams[tableAlias] = true;
                                }
                            }
                        }
                    }
                    else if (key == "*")
                    {
                        columns.Add("*");
                    }
                }
            }
            return columns.Count > 0 ? string.Join(",", columns) : "";
        }
        #endregion 

        #region 解析GetPageTable要返回的字段 -string ParseFields(string fields, ref Dictionary<string, object> queryParams, ref Dictionary<string, Type> customFields)
        /// <summary>
        /// 解析GetPageTable要返回的字段 by hfs 20170405
        /// </summary>
        /// <param name="fields">要解析的字段集合</param>
        /// <param name="queryParams">查询条件参数集合</param>
        /// <param name="customFields">自定义字段容器</param>
        /// <returns></returns>
        private string ParseFields(string fields, ref Dictionary<string, object> queryParams, ref Dictionary<string, Type> customFields)
        {
            Dictionary<string, Field> fieldList = InitFields(ref queryParams);
            if (fieldList == null || fieldList.Count <= 0)
            {
                return !string.IsNullOrWhiteSpace(fields) ? fields : "*";
            }
            if (string.IsNullOrWhiteSpace(fields) || fields == "*")
            {
                return fieldList.ContainsKey("*") ? fieldList["*"].Autonym.ToString() : "*";
            }
            return ParseFields(fields, fieldList, ref queryParams, ref customFields);
        }
        #endregion

        #region 解析GetPageTable要返回的合计字段 -string ParseSumFields(string sumFields, ref Dictionary<string, object> queryParams, ref Dictionary<string, Type> customFields)
        /// <summary>
        /// 解析GetPageTable要返回的合计字段
        /// </summary>
        /// <param name="sumFields"></param>
        /// <param name="queryParams"></param>
        /// <param name="customFields"></param>
        /// <returns></returns>
        private string ParseSumFields(string sumFields, ref Dictionary<string, object> queryParams, ref Dictionary<string, Type> customFields)
        {
            Dictionary<string, Field> sumFieldList = InitSumFields(ref queryParams);
            if (sumFieldList == null || sumFieldList.Count <= 0)
            {
                return null;
            }
            return ParseFields(sumFields, sumFieldList, ref queryParams, ref customFields);
        }
        #endregion

        #region 解析GetPageTable的分组字段 -string ParseGroupby(string groupFields, ref Dictionary<string, object> queryParams)
        /// <summary>
        /// 解析GetPageTable的分组字段
        /// </summary>
        /// <param name="groupFields"></param>
        /// <param name="queryParams"></param>
        /// <returns></returns>
        private string ParseGroupby(string groupFields, ref Dictionary<string, object> queryParams)
        {
            Dictionary<string, Field> groupbyList = InitGroupFields(ref queryParams);
            string groupby = string.Empty;
            if (!string.IsNullOrWhiteSpace(groupFields) && groupbyList != null && groupbyList.Count > 0)
            {
                List<string> columns = new List<string>();
                string[] fieldKeys = groupFields.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string key in fieldKeys)
                {
                    if (groupbyList.ContainsKey(key))
                    {
                        Field field = groupbyList[key];
                        if (field == null) continue;

                        columns.Add(field.Autonym + "");

                        //拼装链表控制器
                        if (field.TableAlias != null && field.TableAlias.Count > 0)
                        {
                            foreach (string tableAlias in field.TableAlias)
                            {
                                queryParams[tableAlias] = true;
                            }
                        }
                    }
                }
                groupby = columns.Count > 0 ? " group by " + string.Join(",", columns) : "";
            }
            return groupby;
        }
        #endregion

        #endregion

        #region Filter 根据节点获取查询条件
        /// <summary>
        /// 根据节点获取查询条件
        /// </summary>
        /// <param name="filterNode">过滤条件的Xml节点.</param>
        /// <returns>
        /// System.String
        /// </returns>
        public virtual string Filter(XmlNode filterNode, out DbParameter[] parameters)
        {
            return DbAccess.Provider.GetCondition(filterNode, out parameters);
        }
        #endregion

        #region Dispose
        /// <summary>
        /// 强制进行内存回收
        /// </summary>
        public void Dispose()
        {
            GC.Collect();
            GC.WaitForFullGCComplete(500);
        }
        #endregion
    }

}
