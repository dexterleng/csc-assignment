# Task 3

## Set up Guide

### 1. Recaptcha

Create a site on Recaptcha (https://www.google.com/recaptcha/).

Configure the recaptcha settings for the site to:

1. allow domains localhost and 127.0.0.1
2. Verify the origin of recaptcha solutions

https://www.google.com/recaptcha/admin/site/{id}/settings

![](images/recaptcha-settings.png)

Copy the Site key and Secret Key

### 2. Configure credentials in project

Open the project. 

Open Config.cs and fill in the site and secret key:

![](images/config.png)

### 3. Start the project

Click on the CSCTask3 in the Solution Explorer. You should see the SSL Url in the Properties table:

![](images/ssl_url.png)

Use this HTTPS URL to access the application instead of the HTTP one.

You can now start the project and navigate to the SSL URL:

![](images/page.png)

