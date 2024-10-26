namespace Leaf.Models.ViewModels.Json
{
    public class CompraJsonView
    {
        public int IdPessoa { get; set; }             
        public int IdAdministrativo { get; set; }     
        public List<CompraItemJsonView> ItensCompra { get; set; } 
        public decimal ValorTotal { get; set; }      

    }
}
