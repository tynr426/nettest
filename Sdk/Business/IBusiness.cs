using ECF;
using ECF.Data;
using ECF.Data.Query;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Sdk.Business
{
    /// <summary>
    /// 业务逻辑处理接口
    /// </summary>
    public interface IBusiness : IDisposable
    {
        #region Property
        /// <summary>
        /// 数据表名
        /// </summary>
        string TableName { get; }

        /// <summary>
        /// 缓存关键字
        /// </summary>
        string CacheKey { get; }

        /// <summary>
        /// 模块名称
        /// </summary>
        string ModuleName { get; }

        /// <summary>
        /// 业务逻辑处理监控器队列.
        /// </summary>
        List<ECF.Optimize.IMonitor> Monitors { get; }

        /// <summary>
        /// 数据处理性能监控详情.
        /// </summary>
        string MonitorDetail { get; }
        #endregion

        /// <summary>
        /// 重置数据处理监控器对象.
        /// </summary>
        void ResetMonitor();


        #region Insert
        /// <summary>
        /// 向数据表中插入数据.
        /// </summary>
        /// <param name="xmlDoc">XmlDocument文档.</param>
        /// <returns></returns>
        int Insert(XmlDocument xmlDoc);

        /// <summary>
        /// 向数据表中插入数据.
        /// </summary>
        /// <param name="json">Json格式的数据.</param>
        /// <returns></returns>
        int Insert(string json);
        #endregion

        #region Delete
        /// <summary>
        /// 删除数据表中的数据.
        /// </summary>
        /// <param name="id">唯一标识的id.</param>
        /// <returns></returns>
        int Delete(int id);

        /// <summary>
        /// 删除数据表中的数据.
        /// </summary>
        /// <param name="condition">条件.</param>
        /// <returns></returns>
        int Delete(string condition, System.Data.Common.DbParameter[] parameters = null);

        /// <summary>
        /// 删除数据表中的数据.
        /// </summary>
        /// <param name="id">唯一标识的id.</param>
        /// <param name="paras">记录日志的参数组.</param>
        /// <returns></returns>
        int Delete(int id, object[] paras);

        /// <summary>
        /// 批量进行数据物理删除
        /// </summary>
        /// <param name="ids">需要删除的id集.</param>
        /// <param name="paras">记录删除日志参数.</param>
        /// <returns>返回影响删除日志的条数</returns>
        int Delete(string[] ids, object[] paras);

        #endregion

        #region LogicDelete
        /// <summary>
        /// 数据逻辑删除，将表中的Status（状态）值修改为-1
        /// </summary>
        /// <param name="id">唯一标识的id.</param>
        /// <returns>
        /// System.Int32
        /// </returns>
        int LogicDelete(int id);

        /// <summary>
        /// 数据逻辑删除，将表中的Status（状态）值修改为-1
        /// </summary>
        /// <param name="condition">逻辑删除的条件.</param>
        /// <returns>
        /// System.Int32
        /// </returns>
        int LogicDelete(string condition, System.Data.Common.DbParameter[] parameters = null);

        /// <summary>
        /// 数据逻辑删除，将表中的Status（状态）值修改为-1
        /// </summary>
        /// <param name="id">待更新的数据id.</param>
        /// <param name="paras">记录日志参数据,第一个参数为日志标题,第二个参数为模块描述,第三参数为日志内容.</param>
        /// <returns></returns>
        int LogicDelete(int id, object[] paras);

        /// <summary>
        /// 批量进行数据物理删除
        /// </summary>
        /// <param name="ids">需要删除的id集.</param>
        /// <param name="paras">记录删除日志参数.</param>
        /// <returns>返回影响删除日志的条数</returns>
        int LogicDelete(string[] ids, object[] paras);

        #endregion

        #region Update

        /// <summary>
        /// 更新数据库记录,支持Json格式的数据.
        /// </summary>
        /// <param name="json">Json格式的数据.</param>
        /// <param name="id">数据表中唯一标识.</param>
        /// <returns></returns>
        int Update(string json, int id);

        /// <summary>
        /// 更新数据库记录,支持Xmldocument.
        /// </summary>
        /// <param name="xmlDoc">XmlDocument文档.</param>
        /// <param name="id">数据表中唯一标识.</param>
        /// <returns></returns>
        int Update(XmlDocument xmlDoc, int id);

        /// <summary>
        /// 更新数据库记录,支持Json格式的数据.
        /// </summary>
        /// <param name="json">Json格式的数据.</param>
        /// <param name="condition">更新条件.</param>
        /// <returns></returns>
        int Update(string json, string[] fields);

        /// <summary>
        /// 更新数据库记录,支持Xmldocument.
        /// </summary>
        /// <param name="xmlDoc">XmlDocument文档.</param>
        /// <param name="condition">更新条件.</param>
        /// <returns></returns>
        int Update(XmlDocument xmlDoc, string[] fields);

        #endregion

        #region DBAccess

        /// <summary>
        /// 数据库访问对象
        /// </summary>
        IDBAccess DbAccess { get; }

        /// <summary>
        /// 数据库访问服务
        /// </summary>
        IDBService DbService { get; }

        /// <summary>
        /// 数据库提供者.
        /// </summary>
        IDBProvider DbProvider { get; }

        #endregion

        #region 获取记录行的详细信息Xml
        /// <summary>
        /// 获取记录行的详细信息.
        /// </summary>
        /// <param name="id">自动编号的id.</param>
        /// <returns>
        /// 返回Xml模式的数据
        /// </returns>
        string DetailXml(int id);

        /// <summary>
        /// 获取记录行的详细信息.
        /// </summary>
        /// <param name="condition">获取详细的条件.</param>
        /// <returns>
        /// 返回Xml模式的数据
        /// </returns>
        string DetailXml(string condition, System.Data.Common.DbParameter[] parameters = null);

        /// <summary>
        /// 获取记录行的详细信息.
        /// </summary>
        /// <param name="conditions">条件参数.</param>
        /// <returns></returns>
        string DetailXml(ConditionParameter[] conditions);

        #endregion

        #region 获取记录行的详细信息Json

        /// <summary>
        /// 获取记录行的详细信息.
        /// </summary>
        /// <param name="id">自动编号的id.</param>
        /// <returns>返回Json模式的数据</returns>
        string DetailJson(int id);

        /// <summary>
        /// 获取记录行的详细信息.
        /// </summary>
        /// <param name="condition">获取详细的条件.</param>
        /// <returns>
        /// 返回Json模式的数据
        /// </returns>
        string DetailJson(string condition, System.Data.Common.DbParameter[] parameters = null);

        /// <summary>
        /// 获取记录行的详细信息.
        /// </summary>
        /// <param name="conditions">条件参数.</param>
        /// <returns></returns>
        string DetailJson(ConditionParameter[] conditions);

        #endregion

        #region 返回当前业务逻辑用到的主实体
        /// <summary>
        /// 返回当前业务逻辑用到的主实体
        /// </summary>
        /// <value></value>
        IEntity Entity { get; }

        /// <summary>
        /// 获取已附值的实体.
        /// </summary>
        /// <param name="id">数据表标识.</param>
        /// <returns></returns>
        IEntity GetEntity(int id);


        /// <summary>
        /// 根据带参数的条件获取实体.
        /// </summary>
        /// <param name="condition">带参数名的条件字符串.</param>
        /// <param name="parameters">执行Sql的数据库参数.</param>
        /// <returns></returns>
        ECF.IEntity GetEntity(string condition, System.Data.Common.DbParameter[] parameters = null);

        /// <summary>
        /// 根据条件组获取实体.
        /// </summary>
        /// <param name="conditions">条件参数组.</param>
        /// <returns></returns>
        ECF.IEntity GetEntity(ConditionParameter[] conditions);

        /// <summary>
        /// 根据条件组获取实体.
        /// </summary>
        /// <param name="conditions">条件参数组.</param>
        /// <returns></returns>
        ECF.IEntity GetEntity(Dictionary<string, object> conditions);
        #endregion

        #region GetDataRow 获取数据行

        /// <summary>
        ///获取数据行.
        /// </summary>
        /// <param name="id">数据表标识.</param>
        /// <returns></returns>
        System.Data.DataRow GetDataRow(int id);

        /// <summary>
        ///获取数据行.
        /// </summary>
        /// <param name="id">数据查询条件.</param>
        /// <returns></returns>
        System.Data.DataRow GetDataRow(string condition, System.Data.Common.DbParameter[] parameters = null);

        /// <summary>
        /// 根据条件获取数据表中第一行数据.
        /// </summary>
        /// <param name="condParameter">条件参数.</param>
        /// <returns></returns>
        System.Data.DataRow GetDataRow(ConditionParameter[] condParameter);
        #endregion

        #region 获取表里的所有数据 DBTable

        /// <summary>
        /// 根据条件参数组获取数据表.
        /// </summary>
        /// <param name="conditions">条件参数组.</param>
        /// <param name="orderBy">显示顺序.</param>
        /// <param name="rows">需要取数据的行数.</param>
        /// <returns></returns>
        System.Data.DataTable DBTable(ConditionParameter[] conditions, string orderBy = "", int rows = 0);

        /// <summary>
        /// 根据条件获取数据表.
        /// </summary>
        /// <param name="rows">行数.</param>
        /// <param name="condition">条件.</param>
        /// <param name="orderBy">排序.</param>
        /// <param name="parameters">数据参数.</param>
        /// <returns></returns>
        System.Data.DataTable DBTable(int rows, string condition, string orderBy, System.Data.Common.DbParameter[] parameters);

        /// <summary>
        /// 根据条件获取数据表.
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="orderBy">The order by.</param>
        /// <returns></returns>
        System.Data.DataTable DBTable(int rows, string condition, string orderBy);

        /// <summary>
        /// 获取表里的所有数据
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>
        /// System.Data.DataTable
        /// </returns>
        System.Data.DataTable DBTable(string condition, System.Data.Common.DbParameter[] parameters);

        /// <summary>
        /// 根据条件获取数据表.
        /// </summary>
        /// <param name="fields">查询返回字段.</param>
        /// <param name="condition">条件.</param>
        /// <param name="parameters">数据参数.</param>
        /// <returns>System.Data.DataTable</returns>
        System.Data.DataTable DBTable(string fields, string condition, DbParameter[] parameters);

        /// <summary>
        /// Databases the table.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <returns></returns>
        System.Data.DataTable DBTable(string condition);

        /// <summary>
        /// 获取表里的所有数据
        /// </summary>
        /// <param name="fields">The fields.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="ident">if set to <c>true</c> [ident].</param>
        /// <returns>
        /// System.Data.DataTable
        /// </returns>
        System.Data.DataTable DBTable(string condition, string fields, bool ident = false);

        /// <summary>
        /// 获取表里的所有数据
        /// </summary>
        /// <param name="fields">The fields.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="orderby">The orderby.</param>
        /// <returns>
        /// System.Data.DataTable
        /// </returns>
        System.Data.DataTable DBTable(string fields, string condition, string orderby);

        /// <summary>
        /// 获取表里的所有数据
        /// </summary>
        /// <returns>System.Data.DataTable</returns>
        System.Data.DataTable DBTable();


        /// <summary>
        /// 获取数据表.
        /// </summary>
        /// <param name="conditions">条件字典.</param>
        /// <param name="orderby">The orderby.</param>
        /// <param name="fields">The fields.</param>
        /// <param name="rows">The rows.</param>
        /// <returns></returns>
        System.Data.DataTable DBTable(Dictionary<string, object> conditions, string orderby = "", int rows = 0, string[] fields = null);
        #endregion

        #region StatusUpdate

        /// <summary>
        /// 数据库中状态更新
        /// </summary>
        /// <param name="status">要更新的状态值.</param>
        /// <param name="id">The id.</param>
        /// <param name="paras">记录日志的参数集.</param>
        /// <returns></returns>
        int StatusUpdate(int status, int id, object[] paras);
        /// <summary>
        /// 数据库中状态更新
        /// </summary>
        /// <param name="status">要更新的状态值.</param>
        /// <param name="ids">待更新的id集.</param>
        /// <param name="paras">记录日志的参数集.</param>
        /// <returns></returns>
        int StatusUpdate(int status, string[] ids, object[] paras);

        /// <summary>
        /// 数据库中状态更新
        /// </summary>
        /// <param name="status">要更新的状态</param>
        /// <param name="condition">条件组合，用于限定StoreId、Id等</param>
        /// <param name="paras">记录日志的参数集</param>
        /// <param name="param">条件参数</param>
        /// <returns></returns>
        int StatusUpdate(int status, string condition, object[] paras, DbParameter[] param = null);

        /// <summary>
        /// 数据库中状态更新(数据还原)
        /// </summary>
        /// <param name="status">要更新的状态值.</param>
        /// <param name="id">The id.</param>
        /// <param name="paras">记录日志的参数集.</param>
        /// <returns></returns>
        int StatusUpdate(int status, int id, object[] paras, bool revert);
        /// <summary>
        /// 数据库中状态更新(数据还原)
        /// </summary>
        /// <param name="status">要更新的状态值.</param>
        /// <param name="ids">待更新的id集.</param>
        /// <param name="paras">记录日志的参数集.</param>
        /// <returns></returns>
        int StatusUpdate(int status, string[] ids, object[] paras, bool revert);
        #endregion

        #region GetValue 获取指定字段和指定条件的第一行第一列的值

        /// <summary>
        /// 获取指定字段和指定条件的第一行第一列的值
        /// </summary>
        /// <param name="fieldName">字段名.</param>
        /// <param name="condition">条件.</param>
        /// <returns>
        /// System.Object
        /// </returns>
        object GetValue(string fieldName, string condition, System.Data.Common.DbParameter[] parameters = null);

        /// <summary>
        /// 获取指定字段和指定条件的第一行第一列的值.
        /// </summary>
        /// <param name="fieldName">字段名.</param>
        /// <param name="condParameter">条件参数集.</param>
        /// <returns></returns>
        object GetValue(string fieldName, ConditionParameter[] condParameter);

        /// <summary>
        /// 获取指定字段和指定条件的第一行第一列的值.
        /// </summary>
        /// <param name="fieldName">字段名.</param>
        /// <param name="condParameter">条件参数集.</param>
        /// <param name="orderBy">排序.</param>
        /// <returns></returns>
        object GetValue(string fieldName, ConditionParameter[] condParameter, string orderBy);

        #endregion

        /// <summary>
        /// 根据Xml过滤信息获取过滤查询条件
        /// </summary>
        /// <param name="filterXml">条件过滤Xml配置.</param>
        /// <returns>
        /// System.String
        /// </returns>
        string Filter(XmlNode filterNode, out System.Data.Common.DbParameter[] parameters);



        /// <summary>
        /// 获取表的数据行数,用于对数据统计
        /// </summary>
        /// <param name="condition">条件.</param>
        /// <returns></returns>
        int Count(string condition, System.Data.Common.DbParameter[] parameters = null);

        /// <summary>
        /// 清除业务的所有缓存.
        /// </summary>
        void ClearCache();
    }

}
