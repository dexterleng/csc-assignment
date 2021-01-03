function ViewModel() {
    var self = this;

    var tokenKey = 'accessToken';

    self.result = ko.observable();
    self.user = ko.observable();

    self.registerEmail = ko.observable();
    self.registerPassword = ko.observable();
    self.registerPassword2 = ko.observable();

    self.loginEmail = ko.observable();
    self.loginPassword = ko.observable();
    self.errors = ko.observableArray([]);

    function showError(jqXHR) {

        self.result(jqXHR.status + ': ' + jqXHR.statusText);

        var response = jqXHR.responseJSON;
        if (response) {
            if (response.Message) self.errors.push(response.Message);
            if (response.ModelState) {
                var modelState = response.ModelState;
                for (var prop in modelState)
                {
                    if (modelState.hasOwnProperty(prop)) {
                        var msgArr = modelState[prop]; // expect array here
                        if (msgArr.length) {
                            for (var i = 0; i < msgArr.length; ++i) self.errors.push(msgArr[i]);
                        }
                    }
                }
            }
            if (response.error) self.errors.push(response.error);
            if (response.error_description) self.errors.push(response.error_description);
        }
    }

    self.callApi = function () {
        self.result('');
        self.errors.removeAll();

        var token = sessionStorage.getItem(tokenKey);
        var headers = {};
        if (token) {
            headers.Authorization = 'Bearer ' + token;
        }

        self.makeApiRequest({
            type: 'GET',
            url: '/api/values',
            headers: headers
        }).then(function (data) {
            self.result(data);
        }).catch(showError);
    }

    self.register = function () {
        grecaptcha.ready(function () {
            grecaptcha.execute(RECAPTCHA_SITE_KEY, { action: 'submit' }).then(function (token) {
                self.result('');
                self.errors.removeAll();

                var data = {
                    Email: self.registerEmail(),
                    Password: self.registerPassword(),
                    ConfirmPassword: self.registerPassword2(),
                    RecaptchaToken: token
                };

                self.makeApiRequest({
                    type: 'POST',
                    url: '/api/Account/Register',
                    contentType: 'application/json; charset=utf-8',
                    data: JSON.stringify(data)
                }).then(function (data) {
                    self.result("Done!");
                }).catch(showError);
            });
        });
    }

    self.login = function () {
        self.result('');
        self.errors.removeAll();

        var loginData = {
            grant_type: 'password',
            username: self.loginEmail(),
            password: self.loginPassword()
        };

        self.makeApiRequest({
            type: 'POST',
            url: '/Token',
            data: loginData
        }).then(function (data) {
            self.user(data.userName);
            // Cache the access token in session storage.
            sessionStorage.setItem(tokenKey, data.access_token);
        }).catch(showError);
    }

    self.logout = function () {
        // Log out from the cookie based logon.
        var token = sessionStorage.getItem(tokenKey);
        var headers = {};
        if (token) {
            headers.Authorization = 'Bearer ' + token;
        }

        self.makeApiRequest({
            type: 'POST',
            url: '/api/Account/Logout',
            headers: headers
        }).then(function (data) {
            // Successfully logged out. Delete the token.
            self.user('');
            sessionStorage.removeItem(tokenKey);
        }).catch(showError);
    }

    self.ajaxPromise = function(ajaxOptions) {
        return new Promise((resolve, reject) => {
            $.ajax(ajaxOptions)
                .done(function (data) { resolve(data) })
                .fail(function (jqXHR) { reject(jqXHR) });
        });
    }

    self.retryingAjax = function (ajaxOptions, maxRetries, timeoutMs) {
        const thunk = () => self.ajaxPromise(ajaxOptions);
        const retry = (fn, maxRetries, timeoutMs) => new Promise((resolve, reject) => {
            fn()
                .then(resolve)
                .catch((e) => {
                    if (maxRetries === 1) {
                        return reject(e);
                    }
                    console.log('retrying failed promise...');
                    setTimeout(() => {
                        retry(fn, maxRetries - 1, timeoutMs).then(resolve).catch(reject);
                    }, timeoutMs);
                    
                })
        });
        return retry(thunk, maxRetries, timeoutMs);
    }

    self.makeApiRequest = async function (ajaxOptions, maxRetries = 5, timeoutMs = 1000) {
        try {
            self.hideAlert();
            self.showSpinner();
            const res = await self.retryingAjax(ajaxOptions, maxRetries, timeoutMs);
            return res;
        } catch (e) {
            if (e.status) {
                self.showAlert(`Request Failed with status code ${e.status}`);
            } else {
                self.showAlert("Request Failed");
            }
            throw e;
        } finally {
            self.hideSpinner();
        }
    }

    self.showSpinner = function () {
        $("#spinner").show();
    }

    self.showAlert = function (msg) {
        $("#danger-alert-text").text(msg);
        $("#danger-alert").show();
    }

    self.hideSpinner = function () {
        $("#spinner").hide();
    }

    self.hideAlert = function () {
        $("#danger-alert").hide();
    }
}

var app = new ViewModel();
ko.applyBindings(app);