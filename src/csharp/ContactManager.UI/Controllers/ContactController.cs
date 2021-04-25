using ContactManager.Entity.Entity;
using ContactManager.Entity.Model;
using ContactManager.UI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContactManager.UI.Controllers
{
    public class ContactController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISession _session;
        private readonly IConfiguration _config;
        public ContactController(IHttpContextAccessor httpContextAccessor, IConfiguration config)
        {
            _httpContextAccessor = httpContextAccessor;
            _session = _httpContextAccessor.HttpContext.Session;
            _config = config;
        }
        public ActionResult Index()
        {
            try
            {
                DataTablesHelper dataTables = new DataTablesHelper() { Skip = 0, Pagesize = 10, SortColumn = "ID", SortDirection = "asc", Filters = new List<Entity.Model.Filter>() };
                return View(new List<Contact>());
            }
            catch (Exception)
            {

            }
            return View();
        }

        public JsonResult LoadData()
        {
            try
            {
                var draw = Request.Form.ContainsKey("draw") ? Request.Form.Where(a => a.Key.Equals("draw")).FirstOrDefault().Value.ToString() : "";
                var start = Request.Form.ContainsKey("start") ? Request.Form.Where(a => a.Key.Equals("start")).FirstOrDefault().Value.ToString() : "";
                var length = Request.Form.ContainsKey("length") ? Request.Form.Where(a => a.Key.Equals("length")).FirstOrDefault().Value.ToString() : "";
                var oc = Request.Form.Where(a => a.Key.Equals("order[0][column]")).FirstOrDefault().Value;
                var sortColumn = Request.Form.Where(a => a.Key.Equals("columns[" + oc.ToString() + "][name]")).FirstOrDefault().Value;
                var sortColumnDir = Request.Form.Where(a => a.Key.Equals("order[0][dir]")).FirstOrDefault().Value;
                
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                DataTablesHelper dataTables = new DataTablesHelper();
                dataTables.Skip = skip;
                dataTables.Pagesize = pageSize;
                dataTables.SortColumn = sortColumn;
                dataTables.SortDirection = sortColumnDir;
                dataTables.Filters = new List<Entity.Model.Filter>();

                string nameKey = string.Empty;
                var searchKeyValues = Request.Form.Keys.Where(a => a.EndsWith("[search][value]")).ToList();
                foreach (var item in searchKeyValues)
                {

                    if (!string.IsNullOrEmpty(Request.Form[item]))
                    {
                        nameKey = item.Replace("[search][value]", "[name]");
                        dataTables.Filters.Add(new Entity.Model.Filter()
                        {
                            PropertyName = Request.Form[nameKey],
                            Operation = "contains",
                            Value = Request.Form[item].ToString()
                        });
                    }
                }

                var url = string.Empty;
                string methodType = string.Empty;
                RestSharpHandler objRestSharpHandler = new RestSharpHandler(_httpContextAccessor, _config);
                List<NameValuePair> lstParameter = new List<NameValuePair>();
                url = "api/contact/all";
                methodType = "POST";
                string json = JsonConvert.SerializeObject(dataTables);
                ResultData<List<Contact>> result = objRestSharpHandler.RestJSONRequestWithoutAuthentication<ResultData<List<Contact>>>(url, methodType, json);

                if (result != null && result.HttpCode.Equals(System.Net.HttpStatusCode.OK))
                {
                    var data = result.Data;
                    recordsTotal = int.Parse(result.Message.Replace("total contacts", "").Trim());
                    //Returning Json Data    
                    return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
                }
                else
                {
                    return Json(new { ErrorMessage = result != null ? result.Message : "Error in API call" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { ErrorMessage = ex.Message });
            }
        }

        public ActionResult Add()
        {
            return View("Edit", new Contact());
        }
        public ActionResult Edit(int ID)
        {
            string message = string.Empty;
            try
            {
                var url = string.Empty;
                string methodType = string.Empty;
                RestSharpHandler objRestSharpHandler = new RestSharpHandler(_httpContextAccessor, _config);                
                List<NameValuePair> lstParameter = new List<NameValuePair>();
                lstParameter.Add(new NameValuePair() { Name = "ID", Value = ID.ToString(), Type = RestSharp.ParameterType.QueryString });
                url = "api/contact/Get";
                methodType = "GET";
                string json = "";
                ResultData<Contact> result = objRestSharpHandler.RestJSONRequestWithoutAuthentication<ResultData<Contact>>(url, methodType, json, lstParameter);

                if (result != null)
                {
                    if (result.IsSuccess)
                    {
                        return View("Edit", result.Data);
                    }
                    else
                    {
                        //return Json(new { ErrorMessage = "Error occurred in API. " + result.Message, JsonRequestBehavior.AllowGet });
                        message = "Error occurred in API. " + result.Message;
                    }
                }
                else
                {
                    //return Json(new { ErrorMessage = "Error occurred while calling API.", JsonRequestBehavior.AllowGet });
                    message = "Error occurred while calling API.";
                }
            }
            catch (Exception ex)
            {
                //return Json(new { ErrorMessage = ex.Message, JsonRequestBehavior.AllowGet });
                message = "Error occurred: " + ex.Message;
            }
            ViewBag.ErrorMessage = message;
            return View("Edit", new Contact());
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Save(Contact contact)
        {
            string message = string.Empty;
            try
            {
                string methodType = string.Empty;
                RestSharpHandler objRestSharpHandler = new RestSharpHandler(_httpContextAccessor, _config);
                string url = contact.ID > 0 ? "api/contact/update" : "api/contact/save";
                methodType = "POST";
                string json = JsonConvert.SerializeObject(contact);
                ResultData<string> result = objRestSharpHandler.RestJSONRequestWithoutAuthentication<ResultData<string>>(url, methodType, json);
                if (result != null && result.IsSuccess)
                {
                    ViewBag.Message = "Contact saved.";
                }
                else
                {
                    ViewBag.ErrorMessage = result == null ? "Error while saving contact" : result.Data;
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error occurred while saving the contact. Please try again.";
            }
            return View("Edit", contact);
        }

        public ActionResult Delete(int ID)
        {
            try
            {
                var url = string.Empty;
                string methodType = string.Empty;
                RestSharpHandler objRestSharpHandler = new RestSharpHandler(_httpContextAccessor, _config);
                List<NameValuePair> lstParameter = new List<NameValuePair>();
                lstParameter.Add(new NameValuePair() { Name = "ID", Value = ID.ToString(), Type = RestSharp.ParameterType.QueryString });
                url = "api/contact/delete";
                methodType = "DELETE";
                string json = "";
                ResultData<string> result = objRestSharpHandler.RestJSONRequestWithoutAuthentication<ResultData<string>>(url, methodType, json, lstParameter);

                if (result != null)
                {
                    if (result.IsSuccess)
                    {
                        return Json(new { Message = "Contact deleted."});
                    }
                    else
                    {
                        return Json(new { ErrorMessage = "Error occurred in API. " + result.Message });
                    }
                }
                else
                {
                    return Json(new { ErrorMessage = "Error occurred while calling API." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { ErrorMessage = ex.Message });
            }
        }
    }
}
