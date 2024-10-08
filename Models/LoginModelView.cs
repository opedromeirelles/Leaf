﻿using System.ComponentModel.DataAnnotations;

namespace Leaf.Models
{
    public class LoginModelView
    {
        [Required(ErrorMessage = "Digite o seu login")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Digite a senha")]
        public string Senha { get; set; }

        public LoginModelView()
        {
            this.Username = "";
            this.Senha = "";

        }
    }
}
