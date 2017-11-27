using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using GH.DAL.SQLDAL;
using GH.DAL.Model;
using System.Drawing;
using GH.DAL.Helpers;
using System.Drawing.Imaging;

namespace GH.DAL.Controllers
{
    public class UploaderController : Controller
    {
        public ActionResult Index(Guid? id)
        {
            return PartialView();
        }
        
        public string Upload(HttpPostedFileBase fileData, string id)
        {
            string m_return = "ok";
            try
            {
                if (fileData != null && fileData.ContentLength > 0)
                {
                    Guid fileId = Guid.NewGuid();

                    var extension = Path.GetExtension(fileData.FileName);

                    var filePath = string.Format("/Content/uploads/{0}", Path.GetFileName(fileId.ToString() + extension));
                    
                    var filePost = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + filePath);

                    //var filePost = this.Server.MapPath(filePath);
                    
                    string[] words = id.Split(';');
                    var sessionid = words[0];

                    FileUpload model = new FileUpload{
                        kFileUploadId = fileId,
                        kSessionId = new Guid(sessionid),
                        sFileUrl = filePath
                    };
                    fileData.SaveAs(filePost);
                    FileUploadManager.Create(model);
                }
            }
            catch(Exception ex)
            {
                m_return = ex.Message;
            }
            return m_return;
        }

        //pass repair id
        public JsonResult GetFiles(Guid Id)
        {
            try
            {
                var itemFounds = FileUploadManager.GetByRepair(Id);
                return Json(new { Result = "OK", Records = itemFounds });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public void Delete(Guid Id)
        {
            var itemFounds = FileUploadManager.GetById(Id);

            var filePost = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + itemFounds.sFileUrl);

            System.IO.File.Delete(filePost);
        }
    }
}
