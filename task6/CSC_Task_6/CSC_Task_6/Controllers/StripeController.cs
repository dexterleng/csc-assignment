using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Stripe;
using Stripe.Checkout;
using System.IO;
using System.Threading.Tasks;
using CSC_Task_6.Models;

namespace CSC_Task_6.Controllers
{
    [RoutePrefix("api/Stripe")]
    public class StripeController : ApiController
    {
        [Authorize]
        [HttpPost]
        [Route("CustomerPortal")]
        public IHttpActionResult CustomerPortal()
        {
            // Authenticate your user.
            var userManager = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = userManager.FindById(User.Identity.GetUserId());

            var options = new Stripe.BillingPortal.SessionCreateOptions
            {
                Customer = user.StripeCustomerId,
                ReturnUrl = DashboardUrl(),
            };
            var service = new Stripe.BillingPortal.SessionService();
            var session = service.Create(options);


            return Created(session.Url, session);
        }

        [Authorize]
        [HttpPost]
        [Route("BasicCheckoutSession")]
        public IHttpActionResult CreateBasicCheckoutSession()
        {
            var userManager = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = userManager.FindById(User.Identity.GetUserId());

            var basicPriceId = Config.StripeBasicPlanPriceId;
            var session = CreateCheckoutSession(basicPriceId, user.StripeCustomerId);

            return Created("", session);
        }

        [Authorize]
        [HttpPost]
        [Route("PremiumCheckoutSession")]
        public IHttpActionResult CreatePremiumCheckoutSession()
        {
            var userManager = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = userManager.FindById(User.Identity.GetUserId());

            var premiumPriceId = Config.StripePremiumPlanPriceId;
            var session = CreateCheckoutSession(premiumPriceId, user.StripeCustomerId);

            return Created("", session);
        }

        private Session CreateCheckoutSession(String priceId, String customerId)
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                {
                    "card",
                },
                LineItems = new List<SessionLineItemOptions>
                {
                new SessionLineItemOptions
                {
                    Price = priceId,
                    Quantity = 1
                },
                },
                Mode = "subscription",
                SuccessUrl = DashboardUrl(),
                CancelUrl = DashboardUrl(),
                Customer = customerId,
            };

            var service = new SessionService();
            Session session = service.Create(options);
            return session;
        }

        [Authorize]
        [HttpGet]
        [Route("Subscription")]
        public IHttpActionResult Subscription() {
            var userManager = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = userManager.FindById(User.Identity.GetUserId());

            var service = new SubscriptionService();
            var subscriptions = service.List(new SubscriptionListOptions
            {
                Customer = user.StripeCustomerId
            });

            if (subscriptions.Count() > 1) {
                return InternalServerError();
            }

            var subscription = subscriptions.FirstOrDefault();

            if (subscription == null) {
                return NotFound();
            }

            var items = subscription.Items;
            
            if (items.Count() != 1) {
                return InternalServerError();
            }

            var priceId = items.First().Price.Id;
            var planName = "";
            if (priceId == Config.StripeBasicPlanPriceId)
            {
                planName = "Basic";
            } else if (priceId == Config.StripePremiumPlanPriceId)
            {
                planName = "Premium";
            }

            return Ok(new {
                status = subscription.Status,
                priceId = priceId,
                planName = planName
            });
        }

        [Authorize]
        [HttpGet]
        [Route("StripeEvents")]
        public IHttpActionResult StripeEvents()
        {
            var userManager = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = userManager.FindById(User.Identity.GetUserId());

            var context = new Models.ApplicationDbContext();
            var stripeEvents = context.StripeEvents.Where(e => e.User.Id == user.Id).OrderByDescending(e => e.Date);

            return Ok(stripeEvents);
        }

        [HttpPost]
        [Route("Webhook")]
        public async Task<IHttpActionResult> Webhook(HttpRequestMessage request)
        {
            var json = await request.Content.ReadAsStringAsync();

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json, request.Headers.GetValues("Stripe-Signature").FirstOrDefault(), Config.StripeWebhookSecretKey);

                if (stripeEvent.Type == Events.ChargeSucceeded || stripeEvent.Type == Events.ChargeFailed)
                {
                    var charge = stripeEvent.Data.Object as Charge;
                    InsertChargeEvent(charge, json);
                }
                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest();
            }
        }

        public void InsertChargeEvent(Charge charge, String chargeJsonString)
        {
            var status = charge.Status;
            var customerId = charge.CustomerId;

            if (customerId == null)
            {
                return;
            }

            var context = new Models.ApplicationDbContext();
            var user = context.Users.Where(u => u.StripeCustomerId == customerId).FirstOrDefault();

            if (user == null)
            {
                return;
            }

            
            var stripeEventRecord = new StripeEvent
            {
                Date = DateTime.Now,
                Json = chargeJsonString,
                User = user
            };
            context.StripeEvents.Add(stripeEventRecord);
            context.SaveChanges();
        }

        private string DashboardUrl()
        {
            var uri = Request.RequestUri;
            var host = uri.Scheme + Uri.SchemeDelimiter + uri.Host + ":" + uri.Port;
            var dashboardUrl = host + "/Home/Dashboard";
            return dashboardUrl;
        }
    }
}
