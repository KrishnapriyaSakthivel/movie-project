﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Team3_Project.Models;

namespace Team3_Project.Controllers {
	[Authorize]
	public class AccountController : Controller {
		private ApplicationSignInManager _signInManager;
		private ApplicationUserManager _userManager;

		public AccountController() {
		}

		public AccountController(ApplicationUserManager userManager , ApplicationSignInManager signInManager) {
			this.UserManager = userManager;
			this.SignInManager = signInManager;
		}

		public ApplicationSignInManager SignInManager {
			get {
				return this._signInManager ?? this.HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
			}
			private set {
				this._signInManager = value;
			}
		}

		public ApplicationUserManager UserManager {
			get {
				return this._userManager ?? this.HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
			}
			private set {
				this._userManager = value;
			}
		}

		//
		// GET: /Account/Login
		[AllowAnonymous]
		public ActionResult Login(System.String returnUrl) {
			DatabaseAccess db = new DatabaseAccess();
			this.ViewBag.ReturnUrl = returnUrl;
			this.ViewBag.Message = "Your application description page.";
			return this.View();
			// Objects.Database.DatabaseMovies movie = new Objects.Database.DatabaseMovies();
			// System.Data.DataSet results = movie.SELECT("SELECT * FROM title_basics limit 10;");
			// return this.View(results);
		}

		//
		// POST: /Account/Login
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Login(LoginViewModel model , System.String returnUrl) {
			if (!this.ModelState.IsValid) {
				return this.View(model);
			}

			// This doesn't count login failures towards account lockout
			// To enable password failures to trigger account lockout, change to shouldLockout: true
			SignInStatus result = await this.SignInManager.PasswordSignInAsync(model.Email , model.Password , model.RememberMe , shouldLockout: false);
			switch (result) {
				case SignInStatus.Success:
					return this.RedirectToLocal(returnUrl);
				case SignInStatus.LockedOut:
					return this.View("Lockout");
				case SignInStatus.RequiresVerification:
					return this.RedirectToAction("SendCode" , new { ReturnUrl = returnUrl , model.RememberMe });
				case SignInStatus.Failure:
				default:
					this.ModelState.AddModelError("" , "Invalid login attempt.");
					return this.View(model);
			}
		}

		//
		// GET: /Account/VerifyCode
		[AllowAnonymous]
		public async Task<ActionResult> VerifyCode(System.String provider , System.String returnUrl , System.Boolean rememberMe) {
			// Require that the user has already logged in via username/password or external login
			return !await this.SignInManager.HasBeenVerifiedAsync()
				? this.View("Error")
				: this.View(new VerifyCodeViewModel { Provider = provider , ReturnUrl = returnUrl , RememberMe = rememberMe });
		}

		//
		// POST: /Account/VerifyCode
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model) {
			if (!this.ModelState.IsValid) {
				return this.View(model);
			}

			// The following code protects for brute force attacks against the two factor codes. 
			// If a user enters incorrect codes for a specified amount of time then the user account 
			// will be locked out for a specified amount of time. 
			// You can configure the account lockout settings in IdentityConfig
			SignInStatus result = await this.SignInManager.TwoFactorSignInAsync(model.Provider , model.Code , isPersistent: model.RememberMe , rememberBrowser: model.RememberBrowser);
			switch (result) {
				case SignInStatus.Success:
					return this.RedirectToLocal(model.ReturnUrl);
				case SignInStatus.LockedOut:
					return this.View("Lockout");
				case SignInStatus.Failure:
				default:
					this.ModelState.AddModelError("" , "Invalid code.");
					return this.View(model);
			}
		}

		//
		// GET: /Account/Register
		[AllowAnonymous]
		public ActionResult Register() {
			return this.View();
		}

		//
		// POST: /Account/Register
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Register(RegisterViewModel model) {
			if (this.ModelState.IsValid) {
				ApplicationUser user = new ApplicationUser { UserName = model.Email , Email = model.Email };
				IdentityResult result = await this.UserManager.CreateAsync(user , model.Password);
				if (result.Succeeded) {
					await this.SignInManager.SignInAsync(user , isPersistent: false , rememberBrowser: false);

					// For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
					// Send an email with this link
					// string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
					// var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
					// await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

					return this.RedirectToAction("Index" , "Home");
				}
				this.AddErrors(result);
			}

			// If we got this far, something failed, redisplay form
			return this.View(model);
		}

		//
		// GET: /Account/ConfirmEmail
		[AllowAnonymous]
		public async Task<ActionResult> ConfirmEmail(System.String userId , System.String code) {
			if (userId == null || code == null) {
				return this.View("Error");
			}
			IdentityResult result = await this.UserManager.ConfirmEmailAsync(userId , code);
			return this.View(result.Succeeded ? "ConfirmEmail" : "Error");
		}

		//
		// GET: /Account/ForgotPassword
		[AllowAnonymous]
		public ActionResult ForgotPassword() {
			return this.View();
		}

		//
		// POST: /Account/ForgotPassword
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model) {
			if (this.ModelState.IsValid) {
				ApplicationUser user = await this.UserManager.FindByNameAsync(model.Email);
				if (user == null || !await this.UserManager.IsEmailConfirmedAsync(user.Id)) {
					// Don't reveal that the user does not exist or is not confirmed
					return this.View("ForgotPasswordConfirmation");
				}

				// For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
				// Send an email with this link
				// string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
				// var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
				// await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
				// return RedirectToAction("ForgotPasswordConfirmation", "Account");
			}

			// If we got this far, something failed, redisplay form
			return this.View(model);
		}

		//
		// GET: /Account/ForgotPasswordConfirmation
		[AllowAnonymous]
		public ActionResult ForgotPasswordConfirmation() {
			return this.View();
		}

		//
		// GET: /Account/ResetPassword
		[AllowAnonymous]
		public ActionResult ResetPassword(System.String code) {
			return code == null ? this.View("Error") : this.View();
		}

		//
		// POST: /Account/ResetPassword
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model) {
			if (!this.ModelState.IsValid) {
				return this.View(model);
			}
			ApplicationUser user = await this.UserManager.FindByNameAsync(model.Email);
			if (user == null) {
				// Don't reveal that the user does not exist
				return this.RedirectToAction("ResetPasswordConfirmation" , "Account");
			}
			IdentityResult result = await this.UserManager.ResetPasswordAsync(user.Id , model.Code , model.Password);
			if (result.Succeeded) {
				return this.RedirectToAction("ResetPasswordConfirmation" , "Account");
			}
			this.AddErrors(result);
			return this.View();
		}

		//
		// GET: /Account/ResetPasswordConfirmation
		[AllowAnonymous]
		public ActionResult ResetPasswordConfirmation() {
			return this.View();
		}

		//
		// POST: /Account/ExternalLogin
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public ActionResult ExternalLogin(System.String provider , System.String returnUrl) {
			// Request a redirect to the external login provider
			return new ChallengeResult(provider , this.Url.Action("ExternalLoginCallback" , "Account" , new { ReturnUrl = returnUrl }));
		}

		//
		// GET: /Account/SendCode
		[AllowAnonymous]
		public async Task<ActionResult> SendCode(System.String returnUrl , System.Boolean rememberMe) {
			System.String userId = await this.SignInManager.GetVerifiedUserIdAsync();
			if (userId == null) {
				return this.View("Error");
			}
			System.Collections.Generic.IList<System.String> userFactors = await this.UserManager.GetValidTwoFactorProvidersAsync(userId);
			System.Collections.Generic.List<SelectListItem> factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose , Value = purpose }).ToList();
			return this.View(new SendCodeViewModel { Providers = factorOptions , ReturnUrl = returnUrl , RememberMe = rememberMe });
		}

		//
		// POST: /Account/SendCode
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> SendCode(SendCodeViewModel model) {
			if (!this.ModelState.IsValid) {
				return this.View();
			}

			// Generate the token and send it
			return !await this.SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider)
				? this.View("Error")
				: (ActionResult) this.RedirectToAction("VerifyCode" , new { Provider = model.SelectedProvider , model.ReturnUrl , model.RememberMe });
		}

		//
		// GET: /Account/ExternalLoginCallback
		[AllowAnonymous]
		public async Task<ActionResult> ExternalLoginCallback(System.String returnUrl) {
			ExternalLoginInfo loginInfo = await this.AuthenticationManager.GetExternalLoginInfoAsync();
			if (loginInfo == null) {
				return this.RedirectToAction("Login");
			}

			// Sign in the user with this external login provider if the user already has a login
			SignInStatus result = await this.SignInManager.ExternalSignInAsync(loginInfo , isPersistent: false);
			switch (result) {
				case SignInStatus.Success:
					return this.RedirectToLocal(returnUrl);
				case SignInStatus.LockedOut:
					return this.View("Lockout");
				case SignInStatus.RequiresVerification:
					return this.RedirectToAction("SendCode" , new { ReturnUrl = returnUrl , RememberMe = false });
				case SignInStatus.Failure:
				default:
					// If the user does not have an account, then prompt the user to create an account
					this.ViewBag.ReturnUrl = returnUrl;
					this.ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
					return this.View("ExternalLoginConfirmation" , new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
			}
		}

		//
		// POST: /Account/ExternalLoginConfirmation
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model , System.String returnUrl) {
			if (this.User.Identity.IsAuthenticated) {
				return this.RedirectToAction("Index" , "Manage");
			}

			if (this.ModelState.IsValid) {
				// Get the information about the user from the external login provider
				ExternalLoginInfo info = await this.AuthenticationManager.GetExternalLoginInfoAsync();
				if (info == null) {
					return this.View("ExternalLoginFailure");
				}
				ApplicationUser user = new ApplicationUser { UserName = model.Email , Email = model.Email };
				IdentityResult result = await this.UserManager.CreateAsync(user);
				if (result.Succeeded) {
					result = await this.UserManager.AddLoginAsync(user.Id , info.Login);
					if (result.Succeeded) {
						await this.SignInManager.SignInAsync(user , isPersistent: false , rememberBrowser: false);
						return this.RedirectToLocal(returnUrl);
					}
				}
				this.AddErrors(result);
			}

			this.ViewBag.ReturnUrl = returnUrl;
			return this.View(model);
		}

		//
		// POST: /Account/LogOff
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult LogOff() {
			this.AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
			return this.RedirectToAction("Index" , "Home");
		}

		//
		// GET: /Account/ExternalLoginFailure
		[AllowAnonymous]
		public ActionResult ExternalLoginFailure() {
			return this.View();
		}

		protected override void Dispose(System.Boolean disposing) {
			if (disposing) {
				if (this._userManager != null) {
					this._userManager.Dispose();
					this._userManager = null;
				}

				if (this._signInManager != null) {
					this._signInManager.Dispose();
					this._signInManager = null;
				}
			}

			base.Dispose(disposing);
		}

		#region Helpers
		// Used for XSRF protection when adding external logins
		private const System.String XsrfKey = "XsrfId";

		private IAuthenticationManager AuthenticationManager {
			get {
				return this.HttpContext.GetOwinContext().Authentication;
			}
		}

		private void AddErrors(IdentityResult result) {
			foreach (System.String error in result.Errors) {
				this.ModelState.AddModelError("" , error);
			}
		}

		private ActionResult RedirectToLocal(System.String returnUrl) {
			return this.Url.IsLocalUrl(returnUrl) ? this.Redirect(returnUrl) : (ActionResult) this.RedirectToAction("Index" , "Home");
		}

		internal class ChallengeResult : HttpUnauthorizedResult {
			public ChallengeResult(System.String provider , System.String redirectUri)
				: this(provider , redirectUri , null) {
			}

			public ChallengeResult(System.String provider , System.String redirectUri , System.String userId) {
				this.LoginProvider = provider;
				this.RedirectUri = redirectUri;
				this.UserId = userId;
			}

			public System.String LoginProvider { get; set; }
			public System.String RedirectUri { get; set; }
			public System.String UserId { get; set; }

			public override void ExecuteResult(ControllerContext context) {
				AuthenticationProperties properties = new AuthenticationProperties { RedirectUri = RedirectUri };
				if (this.UserId != null) {
					properties.Dictionary[XsrfKey] = this.UserId;
				}
				context.HttpContext.GetOwinContext().Authentication.Challenge(properties , this.LoginProvider);
			}
		}
		#endregion
	}
}
