﻿namespace Team3_Project.Controllers {
	public class ProfileController : System.Web.Mvc.Controller {
		// GET: Profile
		new public System.Web.Mvc.ActionResult Profile(int userID=1) {
			return this.View();
		}
	}
}