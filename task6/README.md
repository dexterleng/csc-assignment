# CSC Task 6

## Set Up Guide

### 1. Create Products and Pricing

On the Products Page (https://dashboard.stripe.com/test/products), create two Products:

- Basic Plan
  - Price of $5/day
- Premium Plan
  - Price of $15/day

End Result:
![](images/products.png)

Hold on to the pricing ID for both products, they will be used to configure the project:

![](images/pricing.png)

### 2. Configure Billing Portal

Billing Settings Page: https://dashboard.stripe.com/test/settings/billing/portal

Enable the following options:

1. Payment methods: Allow customers to update their payment methods
2. Cancel subscriptions
3. Update subscriptions

![](images/billing-portal-cancel-update.png)

Add the two products to the portal:

![](images/billing-portal-add-products.png)

Fill in the links with the appropriate fields. Since we are running it locally it can be a random URL:

![](images/billing-portal-links.png)

Save Changes.

### 3. Configure and Start Project

Open the Project with Visual Studio and open Config.cs:

![](images/config.png)

Fill in all the credentials except for `StripeWebhookSecretKey`.

Take note of the localhost port. In our case it is `49856`:

![](images/localhost.png)

### 4. Forward Webhook Events to localhost

From: https://stripe.com/docs/webhooks/test

Install the Stripe CLI. See https://stripe.com/docs/webhooks/test

Run the following command to forward webhook events to the server:

```
stripe listen --forward-to localhost:49856/api/Stripe/Webhook
```

You should see the webhook secret printed in the terminal:

![](images/webhooks.png)

Add the secret to Config.cs and restart the project.

The server should now receive webhook events.

### 5. Start using the application

Head over to /Home/Login/ to register then login. You should then be redirected to /Home/Dashboard/

Note that you can use `4242 4242 4242 4242` as a test card when checking out or managing subscriptions.