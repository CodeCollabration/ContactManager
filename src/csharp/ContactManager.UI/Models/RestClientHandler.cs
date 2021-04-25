using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContactManager.UI.Models
{
    public class RestSharpHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISession _session;
        private readonly IConfiguration _config;
        public RestSharpHandler()
        {

        }
        public RestSharpHandler(IHttpContextAccessor httpContextAccessor, IConfiguration config)
        {
            _httpContextAccessor = httpContextAccessor;
            _session = _httpContextAccessor.HttpContext.Session;
            _config = config;
        }
        public T RestJSONRequestWithAuthentication<T>(string url, string methodType, string json, List<NameValuePair> lstParameters = null) where T : new()
        {

            var restUrl = _config.GetValue(typeof(string), "APIURL")  + url;

            var client = new RestClient(restUrl);

            RestRequest request = null;
            switch (methodType.ToUpper())
            {
                case "GET":
                    request = new RestRequest("", Method.GET);
                    break;
                case "POST":
                    request = new RestRequest("", Method.POST);
                    break;
                default:
                    request = new RestRequest("", Method.GET);
                    break;
            }

            //request.AddParameter("Authorization", string.Format("Bearer {0}", HttpContext.Current.Session["access_token"]), ParameterType.HttpHeader);
            request.AddParameter("Authorization", _session.GetString("access_token"), ParameterType.HttpHeader);
            request.AddParameter("UserID", _session.GetInt32("UserID"), ParameterType.HttpHeader);
            //request.AddParameter("UserRole", _session.GetString("UserRole"), ParameterType.HttpHeader);

            request.AddParameter("text/json", json, ParameterType.RequestBody);


            var response = client.Execute<T>(request);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                
            }

            return response.Data;

        }

        public T RestJSONRequestWithoutAuthentication<T>(string url, string methodType, string json, List<NameValuePair> lstParameters = null) where T : new()
        {

            var restUrl = _config.GetValue(typeof(string), "APIURL") + url;

            var client = new RestClient(restUrl);

            RestRequest request = null;
            switch (methodType.ToUpper())
            {
                case "GET":
                    request = new RestRequest("", Method.GET);
                    break;
                case "POST":
                    request = new RestRequest("", Method.POST);
                    break;
                case "DELETE":
                    request = new RestRequest("", Method.DELETE);
                    break;
                default:
                    request = new RestRequest("", Method.GET);
                    break;
            }

            //request.AddParameter("Authorization", string.Format("Bearer {0}", HttpContext.Current.Session["access_token"]), ParameterType.HttpHeader);
            //request.AddParameter("Authorization", HttpContext.Current.Session["access_token"], ParameterType.HttpHeader);
            //request.AddParameter("UserID", HttpContext.Current.Session["UserID"], ParameterType.HttpHeader);
            //request.AddParameter("UserRole", HttpContext.Current.Session["UserRole"], ParameterType.HttpHeader);

            request.AddParameter("text/json", json, ParameterType.RequestBody);
            if (lstParameters != null && lstParameters.Count > 0)
            {
                foreach (NameValuePair param in lstParameters)
                {
                    request.AddParameter(param.Name, param.Value, param.Type);
                }
            }

            var response = client.Execute<T>(request);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                //var context = new HttpContextWrapper(HttpContext.Current);
                //if (context.Request.IsAjaxRequest())
                //{
                //    context.Response.Clear();
                //    context.Response.Write("UnAuthorized");
                //    context.Response.End();
                //}
                //else
                //{
                //    HttpContext.Current.Response.Redirect("~/Admin/Login");
                //}
            }
            return response.Data;
        }
    }
    public class NameValuePair
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public ParameterType Type { get; set; }
    }
}
