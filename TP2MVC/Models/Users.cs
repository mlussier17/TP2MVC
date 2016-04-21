using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TP2MVC.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Nom d'usager")]
        [Required]
        [StringLength(50)]
        public String Username { get; set; }

        [Display(Name = "Prénom")]
        [Required]
        [StringLength(50)]
        public String Firstname { get; set; }

        [Display(Name = "Nom")]
        [Required]
        [StringLength(50)]
        public String Lastname { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "La chaîne {0} doit comporter au moins {2} caractères.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public String Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmer le mot de passe")]
        [Compare("Password", ErrorMessage = "Le mot de passe et celui de confirmation ne correspondent pas.")]
        public string ConfirmPassword { get; set; }

    }

    public class ChangePasswordViewModel
    {
        [Required]
        [StringLength(50)]
        [DataType(DataType.Password)]
        [Display(Name = "Ancien")]
        public String OldPassword { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "La chaîne {0} doit comporter au moins {2} caractères.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nouveau")]
        public String NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmation")]
        [Compare("NewPassword", ErrorMessage = "Le nouveau mot de passe et celui de confirmation ne correspondent pas.")]
        public string ConfirmPassword { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public String Username { get; set; }
        public String Firstname { get; set; }
        public String Lastname { get; set; }
        public String Password { get; set; }
        public int IsAdmin { get; set; }

        public User() { }
        public User(UserViewModel userViewModel)
        {
            Id = userViewModel.Id;
            Username = userViewModel.Username;
            Firstname = userViewModel.Firstname;
            Lastname = userViewModel.Lastname;
            Password = userViewModel.Password;
            IsAdmin = 0;
        }
    }
    public class Users : Sql_Express_Utilities.SqlExpressWrapper<User>
    {
        public Users(Object connectionString)
            : base(connectionString)
        {
            SetCache(true);
        }

        public User GetUserByName(String username)
        {
            List<User> users = GetByFieldName("UserName", username);
            if (users.Count > 0)
                return users[0];
            return null;
        }

        public bool ChangePassword(User user, String newPassword, String oldPassword)
        {
            bool done = false;
            if (user.Password == oldPassword)
            {
                user.Password = newPassword;
                Update(user);
                done = true;
            }
            return done;
        }

        public String GetUserName(int Id)
        {
            User user = Get(Id);
            if (user != null)
                return user.Username;
            return "";
        }
    }
}