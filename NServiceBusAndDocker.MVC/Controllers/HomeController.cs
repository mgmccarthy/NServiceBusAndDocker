using System.Threading.Tasks;
using System.Web.Mvc;
using NServiceBus;
using NServiceBusAndDocker.Messages.Commands;

namespace NServiceBusAndDocker.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEndpointInstance endpoint;

        public HomeController(IEndpointInstance endpoint)
        {
            this.endpoint = endpoint;
        }

        public async Task<ActionResult> Index()
        {
            //var sendOptions = new SendOptions();
            //sendOptions.DelayDeliveryWith(TimeSpan.FromSeconds(10));
            //await endpoint.Send(new TestCommand(), sendOptions).ConfigureAwait(false);
            await endpoint.Send(new TestCommand()).ConfigureAwait(false);

            return View();
        }
    }
}