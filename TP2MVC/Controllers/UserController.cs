using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TP2MVC.Models;

namespace TP2MVC.Controllers
{
    public class UserController : Controller
    {
        private void AddOnLineUser(User user)
        {
            if (HttpRuntime.Cache["OnLineUsers"] == null)
                HttpRuntime.Cache["OnLineUsers"] = new List<User>();

            ((List<User>)HttpRuntime.Cache["OnLineUsers"]).Add(user);
            HttpRuntime.Cache["OnLineUsersLastUpdate"] = DateTime.Now;

            Session["User"] = user;
            Session.Timeout = 3; // minutes

        }
        private void RemoveOnLineUser()
        {
            try
            {
                ((List<User>)HttpRuntime.Cache["OnLineUsers"]).Remove((User)Session["User"]);
                HttpRuntime.Cache["OnLineUsersLastUpdate"] = DateTime.Now;
            }
            catch (Exception)
            {
            }
            Session["User"] = null;
        }
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(UserViewModel userViewModel)
        {
            if (ModelState.IsValid)
            {
                Users users = (Users)HttpRuntime.Cache["Users"];
                User foundUser = users.GetUserByName(userViewModel.Username);
                if (foundUser != null)
                    ModelState.AddModelError("UserName", "Ce nom d'usager existe déjà!");
                else
                {
                    User newUser = new User(userViewModel);
                    users.Add(newUser);
                    AddOnLineUser(newUser);
                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }
        public ActionResult Manage()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(ChangePasswordViewModel changePasswordViewModel)
        {
            if (ModelState.IsValid)
            {
                User loggedUser = (User)Session["user"];
                if (loggedUser.Password == changePasswordViewModel.OldPassword)
                {
                    loggedUser.Password = changePasswordViewModel.NewPassword;
                    ((Users)HttpRuntime.Cache["Users"]).Update(loggedUser);
                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("OldPassword", "Ancien mot de passe incorrect.");
            }
            return View();
        }

        public ActionResult Login()
        {
            RemoveOnLineUser();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserViewModel userViewModel)
        {
            Users users = (Users)HttpRuntime.Cache["Users"];
            User foundUser = users.GetUserByName(userViewModel.Username);
            if (foundUser == null)
                ModelState.AddModelError("Username", "Ce nom d'usager n'existe pas.");
            else
            {
                if (foundUser.Password != userViewModel.Password)
                    ModelState.AddModelError("Password", "Mot de passe incorrect.");
                else
                {
                    AddOnLineUser(foundUser);
                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }

        public ActionResult LogOff()
        {
            RemoveOnLineUser();
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }
	}
}