using Microsoft.AspNetCore.Mvc;

namespace Erabikata.Backend
{
    public class ControllerRouteAttribute : RouteAttribute
    {
        public ControllerRouteAttribute() : base("api/v{version}/[controller]")
        {
        }
    }
}