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

                    UserViewModel usv = new UserViewModel();
                    usv.Password = newUser.Password;
                    usv.ConfirmPassword = newUser.Password;
                    usv.Username = newUser.Username;
                    Login(usv);

                    return RedirectToAction("Index", "Home"); // Atteindra jamais
                }
            }
            return View();
        }

        public ActionResult Sessions()
        {
            User user = (User)Session["User"];

            if (user != null)
            {
                Connections con = (Connections)HttpRuntime.Cache["Connections"];

                if (user.IsAdmin == 1)
                {
                    return View(con.ToList());
                }
                else
                {
                    List<Connection> ConList = new List<Connection>();
                    //con.ToList().Sort((con1,con2) => (con2.EndDate.CompareTo(con1.EndDate))); //Maybe? 

                    foreach(Connection connection in con.ToList())
                        if (connection.UserId == user.Id) ConList.Add(connection);

                    return View(ConList);
                }
            }
            else return RedirectToAction("Index", "Home");
        }

        public ActionResult Stats(int nbjours = 7)
        {
            ViewData["nbjours"] = nbjours;
            return View();
        }

        [HttpGet]
        public ActionResult ConnectionsJson(int id = 0, int jour = 1, DateTime? date = null)
        {
            DateTime Date;
            if (!date.HasValue) Date = DateTime.Now;
            else Date = (DateTime)date;

            ActionExecutingContext filterContext = new ActionExecutingContext();
            User user = (User)Session["User"];

            if (user != null)
            {
                Connections con = (Connections)HttpRuntime.Cache["Connections"];
                if (user.IsAdmin == 1)
                {
                    int userid = user.Id;
                    if (id != 0) userid = id;

                    filterContext.Result = new JsonResult
                    {
                        Data = con.GetJsonConnectionList(userid, jour, Date),
                        ContentEncoding = System.Text.Encoding.UTF8,
                        ContentType = "application/json",
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
                else
                {
                    filterContext.Result = new JsonResult
                    {
                        Data = con.GetJsonConnectionList(user.Id, jour, Date),
                        ContentEncoding = System.Text.Encoding.UTF8,
                        ContentType = "application/json",
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
            }
            else
            {
                filterContext.Result = new JsonResult
                    {
                        Data = new { Success = false, Data = "Unauthorized" },
                        ContentEncoding = System.Text.Encoding.UTF8,
                        ContentType = "application/json",
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
            }
            return filterContext.Result;
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

                    Connection con = new Connection();
                    con.StartDate = DateTime.Now;
                    con.UserId = foundUser.Id;
                    Session["connection"] = con;

                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }

        public ActionResult LogOff()
        {
            if ((User)Session["User"] != null)
            {
                Connections cons = (Connections)HttpRuntime.Cache["Connections"];
                Connection con = (Connection)Session["connection"];
                con.EndDate = DateTime.Now;
                cons.Add(con);

                RemoveOnLineUser();
                Session.Clear();
                Session.Abandon();
            }
            return RedirectToAction("Index", "Home");
        }
	}
}