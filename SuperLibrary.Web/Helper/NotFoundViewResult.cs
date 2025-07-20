using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace SuperLibrary.Web.Helper;

public class NotFoundViewResult : ViewResult
{
    public NotFoundViewResult(string viewName)
    {
        ViewName = viewName;
        StatusCode = (int)HttpStatusCode.NotFound;
    }
}
