using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ZenithCardPerso.Web.Extentions
{
    internal static class Helpers
    {
        public static ControllerBase GetControllerByName(this HtmlHelper htmlHelper, string controllerName)
        {
            IControllerFactory factory = ControllerBuilder.Current.GetControllerFactory();
            IController controller = factory.CreateController(htmlHelper.ViewContext.RequestContext, controllerName);
            if (controller == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, "The IControllerFactory '{0}' did not return a controller for the name '{1}'.", factory.GetType(), controllerName));
            }
            return (ControllerBase)controller;
        }
    }
}