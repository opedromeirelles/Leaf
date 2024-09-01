namespace Leaf.Entity
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Login { get; set; }
        public string Senha { get; set; }
        public bool Status { get; set; }

        public Departamento departamento = new Departamento();
        private int _idDepartamento;

        public int IdDepartamento
        {
            get{

                if (departamento.IdDpto.Equals(_idDepartamento))
                {
                    return _idDepartamento;
                };

                return 0;
            }
            set
            {
                _idDepartamento = value;
            }

        }

        public Usuario()
        {
            
        }

    }
}
