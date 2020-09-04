using ECF;
using ECF.Json;
using ECF.Web.Http;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdk
{
    public class ApiRequest
    {
        private static string AppId = "096d4009072c927c";

        private static string Secret = "3b56198f096d4009072c927c96fbc8b6";

        /// <summary>
        /// 添加请求签名
        /// </summary>
        /// <param name="apiName">接品名.</param>
        /// <param name="parameters">参数字符串.</param>
        /// <param name="format">返回值格式.</param>
        /// <param name="version">接口版本.</param>
        /// <returns>
        /// String
        /// </returns>
        /// <remarks>
        ///   <list>
        ///    <item><description>添加请求接口签名 added by Shaipe 2018/9/25</description></item>
        ///   </list>
        /// </remarks>
        private string AddSign(string apiName, string parameters, FormatType format, string version)
        {
            Dictionary<string, string> param = parameters.ToDictionary();
            return GetSign(apiName, param, format, version);
        }

        /// <summary>
        /// 添加请求签名
        /// </summary>
        /// <param name="apiName">接品名.</param>
        /// <param name="parameters">参数字符串.</param>
        /// <param name="format">返回值格式.</param>
        /// <param name="version">接口版本.</param>
        /// <returns>
        /// String
        /// </returns>
        /// <remarks>
        ///   <list>
        ///    <item><description>添加请求接口签名 added by Shaipe 2018/9/25</description></item>
        ///   </list>
        /// </remarks>
        private string AddSign(string apiName, QueryParameter[] parameters, FormatType format, string version)
        {
            Dictionary<string, string> param = parameters.ToDictionary();

            return GetSign(apiName, param, format, version);
        }

        

        /// <summary>
        /// 获取接口调用头部
        /// </summary>
        /// <returns>
        /// NameValueCollection
        /// </returns>
        /// <remarks>
        ///   <list>
        ///    <item><description>说明原因 added by Shaipe 2018/9/25</description></item>
        ///   </list>
        /// </remarks>
        private NameValueCollection RequestHeader
        {
            get
            {  //Platform: IOS/Android
               //Version: 533 传入的一个数字
               //Force-Update: true
               //Update-Url: http://xxx.com/ddd
                NameValueCollection nvc = new NameValueCollection();
                //nvc.Add("Platform", "Wap");
                //nvc.Add("Version", Constant.CurrentVersion);

                return nvc;
            }
        }

        /// <summary>
        /// 获取远程字符串.
        ///  Author:   XP-PC/Shaipe
        ///  Created:  09-29-2015
        /// </summary>
        /// <param name="className">类名称.</param>
        /// <param name="actionName">动作名称.</param>
        /// <param name="parameters">参数.</param>
        /// <param name="format">获取数据格式.</param>
        /// <param name="requestMethod">请求方式.</param>
        /// <returns>System.String.</returns>
        public string GetString(string url, string method, string parameters, FormatType format = FormatType.Json, string version = "1.0", string requestMethod = "POST")
        {
            string msg = string.Empty,
             content = string.Empty;

            //string url = ServerGroupRoute.GetServerAPIUrl(ApiUrl, GetMethod(className, actionName));

            string reqParameters = AddSign(method, parameters, format, version);

            if (requestMethod.ToUpper() == "POST")
            {
                content = SyncHttp.HttpPost(url, reqParameters, out msg, RequestHeader);
            }
            else
            {
                content = SyncHttp.HttpGet(url, reqParameters, out msg, RequestHeader);
            }

            if (!msg.IsNullOrEmpty())
            {
                new Exceptions("数据接口请求错误：" + msg + "; 请求Url: " + url);
            }

            return content;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiName"></param>
        /// <param name="param"></param>
        /// <param name="format"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        private static string GetSign(string apiName, Dictionary<string, string> param, FormatType format = FormatType.Json, string version = "3.0")
        {
            param = param ?? new Dictionary<string, string>();
            param = param.ToOrdinalIgnoreCase();
            param.AddItem("appid", AppId);
            if (apiName.IndexOf("vast.") > -1)
            {
                param.AddItem("method", apiName);
            }
            else {
                param.AddItem("method", "linker." + apiName);
            }
            param.AddItem("timestamp", Utils.GetTimstamp().ToString());
            param.AddItem("v", version);
            //if (!param.ContainsKey("fkid"))
            //    param.AddItem("fkid", FKId.ToString());
            //if (!param.ContainsKey("fkflag"))
            //    param.AddItem("fkflag", FKFlag.ToString());
            param.AddItem("format", format.ToString());
            ////默认加上运营主体
            //if (!(apiName.ToLower() == "vast.core.domain.getfkid" || apiName.ToLower() == "vast.core.domain.getproprietordomain"))
            //{
            //    if (!param.ContainsKey("proprietorid"))
            //    {
            //        param.AddItem("proprietorid", DnsRoute.ProprietorId.ToString());
            //    }
            //}
            Dictionary<string, string> sordDic = param.SortFilter();
            string sign = ECF.Security.Encrypt.MD532(sordDic.ToLinkString() + "&secret=" + Secret);
            return param.ToLinkString() + "&sign=" + sign;
        }

        /// <summary>
        /// 获取远程内容
        /// </summary>
        /// <param name="path"></param>
        /// <param name="dic"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static string GetRemoteContent(string url, string apiName, Dictionary<string, string> dic, out string msg)
        {
            try
            {
                NetResponse response = null;
                NetRequest request = new NetRequest();
                request.SetTryTimes(1);

                string data = GetSign(apiName, dic);
                response = request.Post(url, data, string.Empty, out msg);

                if (response != null)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        if (response.Header != null)
                        {
                            string contentType = response.Header.Get("ContentType");
                        }

                        return response.Content;
                    }
                }

                return "{\"code\": " + response.StatusCode + ",\"msg\":\"" + msg + "\"}";
            }
            catch (Exception e)
            {
                msg = e.Message;
                return "";
            }
        }
        /// <summary>
        /// 返回response
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="actionName"></param>
        /// <param name="parameters"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        public static IResultResponse GetResponse(string domain, string actionName, Dictionary<string, string> parameters, Dictionary<string, string> header = null)
        {
            IResultResponse result = null;

            try
            {
                string msg = string.Empty;
                string reqParameters = GetSign(actionName, parameters, FormatType.Json);


                NetResponse response = null;

                NetRequest request = new NetRequest();
                request.SetTimeOut(10000);
                request.SetTryTimes(1);
                if (header != null)
                {
                    foreach (KeyValuePair<string, string> kv in header)
                    {
                        request.AddHeader(kv.Key, kv.Value);
                    }
                }

                if (!domain.StartsWith("http"))
                {
                    domain = "http://" + domain;
                }
                string url = domain + "/Route.axd";

                response = request.Post(url, reqParameters, string.Empty, out msg);

                if (response != null)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        result = ParseResponseData(response.Content);
                    }
                    else
                    {
                        result = ResultResponse.ExceptionResult(response.StatusDescription);
                    }
                }
                else if (!msg.IsNullOrEmpty())
                {
                    result = ResultResponse.ExceptionResult("数据接口请求错误：" + msg + "; 请求接口名: " + actionName);
                    new Exceptions("数据接口请求错误：" + msg + "; 请求接口名: " + actionName);
                }
                else
                {
                    result = ResultResponse.ExceptionResult("数据接口请求错误，请求接口名: " + actionName);
                }
            }
            catch (Exception ex)
            {
                result = ResultResponse.ExceptionResult(ex, ex.Message);
                new Exceptions(ex.Message, ex);
            }

            return result;
        }
        //public static Dictionary<string, object> GetRemoteDictionary(string path, Dictionary<string, object> dic, out string msg)
        //{
        //    string json = ApiRequest.GetRemoteContent(path, dic, out msg);
        //    if (!json.IsNullOrEmpty())
        //    {
        //        object x = JSON.JsonTObject(json);
        //        Dictionary<string, object> dicx = x as Dictionary<string, object>;
        //    }

        //    return null;
        //}

        /// <summary>
        /// 解析返回数据
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static IResultResponse ParseResponseData(string content)
        {
            IResultResponse result = null;

            var dic = JSON.Json2Dictionary(content);
            if (!dic.ContainsKey("Success")) return ResultResponse.ExceptionResult("不是有效对象");
            string message = dic["Message"].ToString();
            int code = Utils.ToInt(dic["Code"]);
            if (Utils.ToBool(dic["Success"]))
            {
                string name = dic["Content"].GetType().Name;

                if (dic["Content"] == null)
                {
                    result = ResultResponse.GetSuccessResult("", message);
                }

                else if (name == "Object[]")
                {

                    result = ResultResponse.GetSuccessResult(ConvertToDataTable(dic["Content"] as object[]), message);
                }

                else if (name == "Dictionary`2")
                {
                    result = ResultResponse.GetSuccessResult(ConvertToDictionary(dic["Content"]), message);
                }
                else
                {
                    result = ResultResponse.GetSuccessResult(dic["Content"].ToString(), message);
                }
            }
            else
            {
                result = ResultResponse.ExceptionResult(message, null, code);
            }
            return result;
        }
        /// <summary>
        /// 转换成dic
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static object ConvertToDictionary(object obj)
        {
            Dictionary<string, object> dic = obj as Dictionary<string, object>;
            //如果有分页就转换分页对象
            if (dic.ContainsKey("PageSize"))
            {
                PagingResult pagingResult = new PagingResult()
                {
                    PageSize = Utils.ToInt(dic["PageSize"]),
                    PageIndex = Utils.ToInt(dic["PageIndex"]),
                    TotalCount = Utils.ToInt(dic["TotalCount"])
                };
                if (dic["Data"].GetType().Name == "Object[]")
                {
                    pagingResult.Data = ConvertToDataTable(dic["Data"] as object[]);
                }
                return pagingResult;
            }
            foreach (var key in dic.Keys)
            {
                if (dic[key].GetType().Name == "Object[]")
                {
                    dic[key] = ConvertToDataTable(dic[key] as object[]);
                    break;
                }
            }
            return dic;
        }
        /// <summary>
        /// 转换成datatable
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        private static DataTable ConvertToDataTable(object[] jsonData)
        {
            DataTable dt = new DataTable();
            for (int i = 0; i < jsonData.Length; i++)
            {
                var item = jsonData[i] as Dictionary<string, object>;
                DataRow dr = dt.NewRow();
                foreach (var key in item.Keys)
                {
                    string name = item[key].GetType().Name;
                    object v = item[key];
                    if (name == "Dictionary`2")
                    {
                        if (!dt.Columns.Contains(key))
                        {
                            dt.Columns.Add(key, typeof(Dictionary<string, object>));
                        }
                    }
                    else if (!dt.Columns.Contains(key))
                    {
                        dt.Columns.Add(key, typeof(string));
                    }
                    dr[key] = v;
                }
                dt.Rows.Add(dr);

            }
            return dt;
        }
    }
}
