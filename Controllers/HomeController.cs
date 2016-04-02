using EMLtoHTML.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EMLtoHTML.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

       
        public ActionResult Index()
        {
            List<Email> emailList = new List<Email>();
            // string targetDirectory = "F:\\EmailDrop";
            string targetDirectory = ConfigurationSettings.AppSettings["EmailDropLocation"];

            string[] fileNames = GetFileNames(targetDirectory);
            foreach (string emlFilePath in fileNames)
            {

                Email email = new Email();

                CDO.Message msg = new CDO.Message();
                ADODB.Stream stream = new ADODB.Stream();
                stream.Open(Type.Missing, ADODB.ConnectModeEnum.adModeUnknown, ADODB.StreamOpenOptionsEnum.adOpenStreamUnspecified, String.Empty, String.Empty);
                stream.LoadFromFile(emlFilePath);
                stream.Flush();
                msg.DataSource.OpenObject(stream, "_Stream");
                msg.DataSource.Save();

                email.time = GetFileDateModified(emlFilePath);
                email.fileName = Path.GetFileName(emlFilePath);
                email.fromAddress = msg.From;
                email.toAddress = msg.To;
                email.emailSubject = msg.Subject;
                email.content = msg.HTMLBody;
                emailList.Add(email);
                stream.Close();
            }
            var data = emailList;
            return View(data);
        }

        public ActionResult ViewContent(string fileName)
        {
            string targetFile = string.Format(ConfigurationSettings.AppSettings["EmailDropLocation"] + "\\{0}", fileName);

            CDO.Message mail = new CDO.Message();
            ADODB.Stream stream = new ADODB.Stream();
            stream.Open(Type.Missing, ADODB.ConnectModeEnum.adModeUnknown, ADODB.StreamOpenOptionsEnum.adOpenStreamUnspecified, String.Empty, String.Empty);
            stream.LoadFromFile(targetFile);
            stream.Flush();
            mail.DataSource.OpenObject(stream, "_Stream");
            mail.DataSource.Save();

            Email email = new Email();
            email.fileName = Path.GetFileName(targetFile);
            email.content = mail.HTMLBody;

            return View("ViewContent", email);
        }

        private string[] GetFileNames(string targetDrirectory)
        {
            return Directory.GetFiles(targetDrirectory);
        }

        private DateTime GetFileDateModified(string filePath)
        {

            FileInfo fileInfo = new FileInfo(filePath);
            return fileInfo.LastWriteTime;
        }

        public ActionResult ClearFolder(string fileName)
        {
            string targetDirectory = ConfigurationSettings.AppSettings["EmailDropLocation"];
            DirectoryInfo emlFileDirectory = new DirectoryInfo(targetDirectory);

            if (fileName != null)
            {
                FileInfo fileToDelete = new FileInfo(targetDirectory + "//" + fileName);
                fileToDelete.Delete();
                return RedirectToAction("Index");

            }



            foreach (FileInfo file in emlFileDirectory.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in emlFileDirectory.GetDirectories())
            {
                dir.Delete(true);
            }

            return RedirectToAction("Index");
        }

        public ActionResult DownloadFile(string fileName)
        {

            FileInfo document = new FileInfo(fileName);
            string fullPath = ConfigurationSettings.AppSettings["EmailDropLocation"] + "//" + fileName;
            return File(fullPath, System.Net.Mime.MediaTypeNames.Application.Octet);

        }


    }
}
