using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSC_Task_6
{
    public class Config
    {
        public static string StripePublishableKey = "";
        public static string StripeSecretKey = "";

        // create two products, basic and premium, each with a single price.
        public static string StripeBasicPlanPriceId = "";
        public static string StripePremiumPlanPriceId = "";

        // you can get this from the logs after starting the stripe cli webhook forwarding
        // stripe listen --forward-to localhost:49856/api/Stripe/Webhook
        public static string StripeWebhookSecretKey = "";
    }
}