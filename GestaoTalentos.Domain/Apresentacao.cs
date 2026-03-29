//namespace GestaoTalentos.Domain;

//class representa a apresentação de um talento a um cliente,
//contendo informações sobre o cliente, o talento e a data da apresentação.
//public class Apresentacao
//{
//public int Id { get; set; }
//public int IdCliente { get; set; }
//public Cliente Cliente { get; set; } = null!;
    //public int IdTalento { get; set; }
    //public Talento Talento { get; set; } = null!;
//public DateTime DataApresentacao { get; set; } = DateTime.UtcNow;
    
//modelBuilder.Entity<Apresentacao>()
    //   .HasOne(a => a.Cliente)
    // .WithMany(c => c.Apresentacoes)
    // .HasForeignKey(a => a.IdCliente)
    //  .OnDelete(DeleteBehavior.Cascade);

//}