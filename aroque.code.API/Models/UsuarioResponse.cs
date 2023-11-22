namespace aroque.code.API.Models
{
    public class UsuarioResponse
    {
        public List<Usuario> Usuarios { get; set; } = new List<Usuario>();
        public int Pages { get; set; } 
        public int CurrentPage { get; set; }    
    }
}
