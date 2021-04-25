using ContactManager.DAL.Model;
using ContactManager.Entity.Entity;
using ContactManager.Entity.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ContactManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private IUnitOfWork _unitOfWork;
        private readonly ILogger<ContactController> _logger;
        public ContactController(IUnitOfWork unitOfWork, ILogger<ContactController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }        

        // GET: api/<Books>
        [HttpPost]
        [Route("all")]
        public async Task<IActionResult> Get(DataTablesHelper dataTables)
        {
            //return await _unitOfWork.Contacts.GetAll(dataTables);
            try
            {
                var contacts = _unitOfWork.Contacts.GetAll(dataTables);
                ResultData<List<Contact>> result = new ResultData<List<Contact>>();
                result.IsSuccess = true;
                result.Data = contacts.Result.ToList();
                result.HttpCode = HttpStatusCode.OK;
                result.Message = string.Format("{0} total contacts", _unitOfWork.Contacts.Count());
                return Ok(result);
            }
            catch (Exception ex)
            {
                //return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Resource not found.");
                ResultData<string> result = new ResultData<string>();
                result.IsSuccess = false;
                result.Data = ex.InnerException != null ? ex.Message + " Additional Information: " + ex.InnerException.Message : ex.Message;
                result.HttpCode = HttpStatusCode.InternalServerError;
                result.Message = "Error occurred while getting all the contacts. Error Information: " + ex.ToString();
                _logger.LogError(ex, "Error occurred while getting all the contacts.");
                return Ok(result);
                //return BadRequest("Error Occurred: " + ex.Message);
            }
        }

        [HttpGet]
        [Route("active")]
        public async Task<IActionResult> GetActiveContacts()
        {
            try
            {
                var contacts = _unitOfWork.Contacts.GetActiveContacts();
                ResultData<List<Contact>> result = new ResultData<List<Contact>>();
                result.IsSuccess = true;
                result.Data = contacts.ToList();
                result.HttpCode = HttpStatusCode.OK;
                result.Message = string.Format("{0} active contacts found.", contacts.Count());
                return Ok(result);
            }
            catch (Exception ex)
            {
                ResultData<string> result = new ResultData<string>();
                result.IsSuccess = false;
                result.Data = ex.InnerException != null ? ex.Message + " Additional Information: " + ex.InnerException.Message : ex.Message;
                result.HttpCode = HttpStatusCode.InternalServerError;
                result.Message = "Error occurred while getting active contacts. Error Information: " + ex.ToString();
                _logger.LogError(ex, "Error occurred while getting active contacts.");
                return Ok(result);
            }
        }

        [HttpGet]
        [Route("inactive")]
        public async Task<IActionResult> GetInActiveContacts()
        {
            try
            {
                var contacts = _unitOfWork.Contacts.GetInActiveContacts();
                ResultData<List<Contact>> result = new ResultData<List<Contact>>();
                result.IsSuccess = true;
                result.Data = contacts.ToList();
                result.HttpCode = HttpStatusCode.OK;
                result.Message = string.Format("{0} inactive contacts found.", contacts.Count());
                return Ok(result);
            }
            catch (Exception ex)
            {
                ResultData<string> result = new ResultData<string>();
                result.IsSuccess = false;
                result.Data = ex.InnerException != null ? ex.Message + " Additional Information: " + ex.InnerException.Message : ex.Message;
                result.HttpCode = HttpStatusCode.InternalServerError;
                result.Message = "Error occurred while getting inactive contacts. Error Information: " + ex.ToString();
                _logger.LogError(ex, "Error occurred while getting inactive contacts.");
                return Ok(result);
            }
        }

        [HttpGet]
        [Route("Get")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var contact = await _unitOfWork.Contacts.GetById(id);
                ResultData<Contact> result = new ResultData<Contact>();
                result.IsSuccess = true;
                result.Data = contact;
                result.HttpCode = contact != null ? HttpStatusCode.OK : HttpStatusCode.BadRequest;
                result.Message = contact != null ? "Contact found" : "Contact not found";
                return Ok(result);
            }
            catch (Exception ex)
            {
                ResultData<string> result = new ResultData<string>();
                result.IsSuccess = false;
                result.Data = ex.InnerException != null ? ex.Message + " Additional Information: " + ex.InnerException.Message : ex.Message;
                result.HttpCode = HttpStatusCode.InternalServerError;
                result.Message = "Error occurred while getting the contact information. Error Information: " + ex.ToString();
                _logger.LogError(ex,"Error occurred while getting the contact information.");
                return Ok(result);
            }
        }

        [HttpPost]
        [Route("save")]
        public async Task<IActionResult> Save(Contact contact)
        {
            try
            {
                int res = await _unitOfWork.Contacts.Insert(contact);
                _unitOfWork.Save();
                int id = contact.ID;
                ResultData<string> result = new ResultData<string>();
                result.IsSuccess = true;
                result.Data = id.ToString();
                result.HttpCode = HttpStatusCode.OK;
                result.Message = "contact saved";
                return Ok(result);
            }
            catch (Exception ex)
            {
                ResultData<string> result = new ResultData<string>();
                result.IsSuccess = false;
                result.Data = ex.InnerException != null ? ex.Message + " Additional Information: " + ex.InnerException.Message : ex.Message;
                result.HttpCode = HttpStatusCode.InternalServerError;
                result.Message = "Error occurred while saving the contact. Error Information: " + ex.ToString();
                _logger.LogError(ex, "Error occurred while saving the contact.");
                return Ok(result);
            }
        }

        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Update(Contact contact)
        {
            try
            {

                var ocontact = await _unitOfWork.Contacts.GetById(contact.ID);
                if (ocontact != null)
                {
                    ocontact.ID = contact.ID;
                    ocontact.FirstName = contact.FirstName;
                    ocontact.LastName = contact.LastName;
                    ocontact.Email = contact.Email;
                    ocontact.PhoneNumber = contact.PhoneNumber;
                    ocontact.IsActive = contact.IsActive;

                    int res = await _unitOfWork.Contacts.Update(ocontact);
                    _unitOfWork.Save();
                    int id = contact.ID;
                    ResultData<string> result = new ResultData<string>();
                    result.IsSuccess = true;
                    result.Data = "Contact updated successfully.";
                    result.HttpCode = HttpStatusCode.OK;
                    result.Message = "contact updated";
                    return Ok(result);
                }
                else
                {
                    ResultData<string> result = new ResultData<string>();
                    result.IsSuccess = false;
                    result.Data = "Contact not found.";
                    result.HttpCode = HttpStatusCode.InternalServerError;
                    result.Message = "Contact not found";
                    return Ok(result);
                }

            }
            catch (Exception ex)
            {
                ResultData<string> result = new ResultData<string>();
                result.IsSuccess = false;
                result.Data = ex.InnerException != null ? ex.Message + " Additional Information: " + ex.InnerException.Message : ex.Message;
                result.HttpCode = HttpStatusCode.InternalServerError;
                result.Message = "Error occurred while updating the contact. Error Information: " + ex.ToString();
                _logger.LogError(ex, "Error occurred while updating the contact.");
                return Ok(result);
            }
        }

        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var contact = await _unitOfWork.Contacts.GetById(id);
                if (contact != null)
                {
                    _unitOfWork.Contacts.Delete(contact);
                    _unitOfWork.Save();
                    //int id = contact.ID;
                    ResultData<string> result = new ResultData<string>();
                    result.IsSuccess = true;
                    result.Data = "Contact deleted successfully.";
                    result.HttpCode = HttpStatusCode.OK;
                    result.Message = "contact deleted";
                    return Ok(result);
                }
                else
                {
                    ResultData<string> result = new ResultData<string>();
                    result.IsSuccess = false;
                    result.Data = "Contact not found.";
                    result.HttpCode = HttpStatusCode.InternalServerError;
                    result.Message = "Contact not found";
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                ResultData<string> result = new ResultData<string>();
                result.IsSuccess = false;
                result.Data = ex.InnerException != null ? ex.Message + " Additional Information: " + ex.InnerException.Message : ex.Message;
                result.HttpCode = HttpStatusCode.InternalServerError;
                result.Message = "Error occurred while deleting the contact. Error Information: " + ex.ToString();
                _logger.LogError(ex, "Error occurred while deleting the contact.");
                return Ok(result);
            }
        }
    }
}
