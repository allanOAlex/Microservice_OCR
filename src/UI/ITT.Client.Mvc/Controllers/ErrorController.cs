using ITT.Client.Mvc.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Diagnostics;

namespace ITT.Client.Mvc.Controllers
{
    public class ErrorController : Controller
    {
        private readonly IActionDescriptorCollectionProvider actionDescriptorCollectionProvider;

        public ErrorController(IActionDescriptorCollectionProvider ActionDescriptorCollectionProvider)
        {
            actionDescriptorCollectionProvider = ActionDescriptorCollectionProvider;
        }

        [HttpGet("/Error/HandleError/{statusCode}")]
        public IActionResult HandleError(int? statusCode = null)
        {
            try
            {
                var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
                if (exceptionDetails != null && statusCode.HasValue)
                {
                    ViewData["Message"] = GetErrorMessage(statusCode.Value);

                    ViewBag.StatusCode = statusCode.Value;
                    ViewBag.ExceptionPath = exceptionDetails!.Path;
                    ViewBag.ExecptionMessage = exceptionDetails.Error.Message;

                    return View($"Error{statusCode}");
                }

                if (statusCode.HasValue)
                {
                    ViewData["Message"] = GetErrorMessage(statusCode.Value);

                    if (ViewExists($"Error{statusCode}"))
                    {
                        ViewData["Message"] = GetErrorMessage(statusCode.Value);
                        return View($"Error{statusCode}");
                    }
                    else
                    {
                        return View("Error500");
                    }

                    //return View($"Error{statusCode}");
                }

                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
            catch (Exception)
            {
                throw;
            }

        }

        [HttpGet("/Error/Error/{problemDetails}")]
        public IActionResult Error(ProblemDetails problemDetails)
        {
            try
            {
                var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
                if (problemDetails != null)
                {
                    ViewData["Message"] = GetErrorMessage(problemDetails.Status);

                    ViewBag.StatusCode = problemDetails.Status;
                    ViewBag.ExceptionPath = problemDetails.Instance;
                    ViewBag.ExecptionMessage = problemDetails.Detail;

                    return View($"Error{problemDetails.Status}");
                }

                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
            catch (Exception)
            {
                throw;
            }

        }

        private string GetErrorMessage(int? statusCode)
        {
            return statusCode switch
            {
                401 => "Unauthorized Access",
                403 => "Forbidden Access",
                404 => "Resource Not Found",
                500 => "Internal Server Error",
                _ => "An Error Occurred. \nSomething Went Wrong",
            };
        }

        private bool ViewExists(string viewName)
        {
            var actionDescriptors = actionDescriptorCollectionProvider.ActionDescriptors.Items;

            foreach (var descriptor in actionDescriptors)
            {
                if (descriptor is ControllerActionDescriptor controllerActionDescriptor)
                {
                    if (controllerActionDescriptor.ActionName == viewName &&
                        controllerActionDescriptor.ControllerName == nameof(ErrorController))
                    {
                        return true;
                    }
                }
            }

            return false;
        }


    }

}
