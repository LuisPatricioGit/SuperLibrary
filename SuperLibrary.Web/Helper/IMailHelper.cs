namespace SuperLibrary.Web.Helper;

public interface IMailHelper
{
    Response SendEmail(string nameTo,string to, string subject, string body);
}

