namespace Leaf.Models
{
    public class Departamento
    {
        public int IdDpto { get; set; }

        public string Descricao { get; set; }

        // Relação 1 para N: um departamento pode ter muitos usuários
        public ICollection<Usuario> Usuarios { get; set; }
    }

}
