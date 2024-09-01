namespace Leaf.Models
{
    public class LoginModelView
    {
        public string Username { get; set; }
        public string Senha { get; set; }

        public LoginModelView(string nome, string senha)
        {
            this.Username = nome;
            this.Senha = senha;
        }

    }
}
