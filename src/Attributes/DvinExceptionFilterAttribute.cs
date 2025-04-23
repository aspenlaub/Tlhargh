using Aspenlaub.Net.GitHub.CSharp.Dvin.Components;
using Aspenlaub.Net.GitHub.CSharp.Dvin.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Aspenlaub.Net.GitHub.CSharp.Tlhargh.Attributes;

public class DvinExceptionFilterAttribute : ExceptionFilterAttribute {
    private static IFolder? _exceptionLogFolder;

    // ReSharper disable once UnusedMember.Global
    public static void SetExceptionLogFolder(IFolder exceptionLogFolder) {
        _exceptionLogFolder = exceptionLogFolder;
    }

    public override void OnException(ExceptionContext context) {
        ExceptionSaver.SaveUnhandledException(_exceptionLogFolder, context.Exception, nameof(Dvin), _ => { });
        context.Result = new JsonResult(InternalServerError.Create("An exception was logged. We are sorry for the inconvenience."));
    }
}