using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace ELOR.Razzle.Controllers
{
    /// <summary>
    /// Replaces the standard MVC routing with VK-style routes for every controller
    /// that inherits from <see cref="ApiControllerBase"/>:
    /// <code>/api/{controllerName}.{actionName}</code>
    /// Both controller and action names are converted to camelCase.
    /// <para>
    /// Examples:
    /// <list type="bullet">
    ///   <item><c>AuthController.SignIn</c> → <c>/api/auth.signIn</c></item>
    /// </list>
    /// </para>
    /// The convention accepts both GET and POST on every endpoint.
    /// The "Async" suffix is stripped automatically by ASP.NET Core before this convention runs,
    /// so <c>SignInAsync</c> becomes action name <c>SignIn</c>.
    /// </summary>
    public sealed class VKAPIStyleRouteConvention : IApplicationModelConvention
    {
        public void Apply(ApplicationModel application)
        {
            foreach (var controller in application.Controllers)
            {
                if (!typeof(APIControllerBase).IsAssignableFrom(controller.ControllerType))
                    continue;

                var controllerSegment = ToCamelCase(controller.ControllerName);

                foreach (var action in controller.Actions)
                {
                    var actionSegment = ToCamelCase(action.ActionName);

                    // Replace any selectors the developer may have declared with exactly one
                    // VK-style selector that accepts GET and POST.
                    action.Selectors.Clear();

                    var selector = new SelectorModel
                    {
                        AttributeRouteModel = new AttributeRouteModel
                        {
                            Template = $"/{controllerSegment}.{actionSegment}"
                        }
                    };

                    selector.ActionConstraints.Add(new HttpMethodActionConstraint(["GET", "POST"]));
                    action.Selectors.Add(selector);
                }
            }
        }

        private static string ToCamelCase(string value) =>
            value is { Length: > 0 }
                ? char.ToLowerInvariant(value[0]) + value[1..]
                : value;
    }
}
