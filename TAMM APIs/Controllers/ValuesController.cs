using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TAMM_APIs.Models;

namespace TAMM_APIs.Controllers
{
    [RoutePrefix("api/TAMM")]
    public class ValuesController : ApiController
    {
        [HttpPost, Route("OTPSMS")]
        public string OTPSMS()
        {

            string destinationUrl = ConfigurationManager.AppSettings["OTPSMSURL"];
            string requestXml = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:ws=\"http://ws.otp.elm.com/\">" +
                                "<soapenv:Header/>" +
                                "<soapenv:Body>" +
                                "<ws:generateOTP>" +
                                "<otpRequest>" +
                                "<username>" + ConfigurationManager.AppSettings["username"] + "</username>" +
                                "<password>" + ConfigurationManager.AppSettings["password"] + "</password>" +
                                "<nationalId>" + ConfigurationManager.AppSettings["nationalId"] + "</nationalId>" +
                                "<serviceCode>" + ConfigurationManager.AppSettings["serviceCode"] + "</serviceCode>" +
                                "<mobileNo>" + ConfigurationManager.AppSettings["mobileNo"] + "</mobileNo>" +
                                "</otpRequest>" +
                                "</ws:generateOTP>" +
                                "</soapenv:Body>" +
                                "</soapenv:Envelope>";

            string Response = postXMLData(destinationUrl, requestXml);

            string CorrelationId = null;

            if (Response.Split(new[] { "<resultMessage>", "</resultMessage>" }, StringSplitOptions.None)[1].ToLower() == "success")
                CorrelationId = Response.Split(new[] { "<correleationID>", "</correleationID>" }, StringSplitOptions.None)[1];

            return CorrelationId;
        }

        [HttpPost, Route("DrivingAuthorization")]
        public string DrivingAuthorization(IssueAuth IssueAuthObj)
        {
            string destinationUrl = ConfigurationManager.AppSettings["DrivingAuthorization"];
            string requestXml = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:ws=\"http://ws.drivingauthorization.carportal.elm.com/\">" +
                                "<soapenv:Header>" +
                                "<ws:lang>En</ws:lang>" +
                                "<ws:password>" + ConfigurationManager.AppSettings["password"] + "</ws:password>" +
                                "<ws:username>" + ConfigurationManager.AppSettings["username"] + "</ws:username>" +
                                "</soapenv:Header>" +
                                "<soapenv:Body>" +
                                "<ws:issueAuthorization>" +
                                "<ownerIdVersion>0</ownerIdVersion>" +
                                "<authorizedId>" + ConfigurationManager.AppSettings["nationalId"] + "</authorizedId>" +
                                "<authorizedIdVersion>10</authorizedIdVersion>" +
                                "<authorizedMobileNo>" + ConfigurationManager.AppSettings["mobileNo"] + "</authorizedMobileNo>" +
                                "<plateInfo>" +
                                "<plateNumber>" + IssueAuthObj.LicenseInfo.plateNumber + "</plateNumber>" +
                                "<plateText1>" + IssueAuthObj.LicenseInfo.plateText1 + "</plateText1>" +
                                "<plateText2>" + IssueAuthObj.LicenseInfo.plateText2 + "</plateText2>" +
                                "<plateText3>" + IssueAuthObj.LicenseInfo.plateText3 + "</plateText3>" +
                                "<plateType>" + IssueAuthObj.LicenseInfo.plateType + "</plateType>" +
                                "</plateInfo>" +
                                "<fromHijri>" + IssueAuthObj.fromHijri + "</fromHijri>" +
                                "<toHijri>" + IssueAuthObj.toHijri + "</toHijri>" +
                                "<external>false</external>" +
                                "<authenticationCode>" + IssueAuthObj.authenticationCode + "</authenticationCode>" +
                                "<correlationId>" + IssueAuthObj.correlationId + "</correlationId>" +
                                "</ws:issueAuthorization>" +
                                "</soapenv:Body>" +
                                "</soapenv:Envelope>";

            string Response = postXMLData(destinationUrl, requestXml);

            string Result = null;

            if (Response.Split(new[] { "<message>", "</message>" }, StringSplitOptions.None)[1].ToLower() == "operation has been done successfully")
                Result = "Success";

            return Result;
        }

        [HttpPost, Route("Cancellation")]
        public string Cancellation(CancelAuth CancelAuthObj)
        {
            string destinationUrl = ConfigurationManager.AppSettings["DrivingAuthorization"];
            string requestXml = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:ws=\"http://ws.drivingauthorization.carportal.elm.com/\">" +
                                "<soapenv:Header>" +
                                "<ws:lang>EN</ws:lang>" +
                                "<ws:password>" + ConfigurationManager.AppSettings["password"] + "</ws:password>" +
                                "<ws:username>" + ConfigurationManager.AppSettings["username"] + "</ws:username>" +
                                "</soapenv:Header>" +
                                "<soapenv:Body>" +
                                "<ws:cancelAuthorization>" +
                                "<authorizationNumber>" + CancelAuthObj.authorizationNumber + "</authorizationNumber>" +
                                "<plateInfo>" +
                                "<plateNumber>" + CancelAuthObj.LicenseInfo.plateNumber + "</plateNumber>" +
                                "<plateText1>" + CancelAuthObj.LicenseInfo.plateText1 + "</plateText1>" +
                                "<plateText2>" + CancelAuthObj.LicenseInfo.plateText2 + "</plateText2>" +
                                "<plateText3>" + CancelAuthObj.LicenseInfo.plateText3 + "</plateText3>" +
                                "<plateType>" + CancelAuthObj.LicenseInfo.plateType + "</plateType>" +
                                "</plateInfo>" +
                                "<external>false</external>" +
                                "</ws:cancelAuthorization>" +
                                "</soapenv:Body>" +
                                "</soapenv:Envelope>";
            string Response = postXMLData(destinationUrl, requestXml);

            return "";
        }

        public string postXMLData(string destinationUrl, string requestXml)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(destinationUrl);
            byte[] bytes;
            bytes = System.Text.Encoding.ASCII.GetBytes(requestXml);
            request.ContentType = "text/xml;charset=utf-8";
            request.ContentLength = bytes.Length;
            request.Method = "POST";
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Close();
            HttpWebResponse response;
            response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream responseStream = response.GetResponseStream();
                string responseStr = new StreamReader(responseStream).ReadToEnd();
                return responseStr;
            }
            return null;
        }
    }
}
