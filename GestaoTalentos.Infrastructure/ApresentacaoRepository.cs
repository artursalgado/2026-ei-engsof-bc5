//using GestaoTalentos.Domain;
//using Microsoft.EntityFrameworkCore;

//namespace GestaoTalentos.Infrastructure;

/// Repositório para operações de base de dados relacionadas a apresentações.
/// Implementa a interface IApresentacaoRepository.
//public class ApresentacaoRepository : IApresentacaoRepository
//{
//  private readonly GestaoTalentos.Infrastructure.AppDbContext _context;
 
    /// Construtor que injeta o contexto da base de dados.
    /// <param name="context">Instância do AppDbContext.</param>
    //  public ApresentacaoRepository(GestaoTalentos.Infrastructure.AppDbContext context) => _context = context;

    /// Obtém uma Apresentacao pelo Id do Cliente e Id do Talento
    //public async Task<Apresentacao?> GetByIdsAsync(int idCliente, int idTalento)
      //  => await _context.Apresentacoes
        //    .FirstOrDefaultAsync(a => a.IdCliente == idCliente && a.IdTalento == idTalento);
    
//        Task<Apresentacao?> GetByIdClientesync(int idCliente) 
  //      => _context.Apresentacoes
    //        .FirstOrDefault(a => a.IdCliente == idCliente);
        
        
    /// Obtém todas as Apresentações.
    /// <returns>Lista de todas os Apresentacoes.</returns>
    //public async Task<List<Apresentacao>> GetAllAsync()
    //  => await _context.Apresentacoes
    //.AsNoTracking()
            //      .ToListAsync();
    
    /// Adiciona um novo Apresentacao à base de dados.
    /// <param name="Apresentacao">Instância da Apresentacao a ser adicionada.</param>
    //public async Task AddAsync(Apresentacao apresentacao)
    //{
    //  await _context.Apresentacoes.AddAsync(apresentacao);
    //  await _context.SaveChangesAsync();
    //}
    
    /// Atualiza uma Apresentacao existente na base de dados.
    /// <param name="Apresentacao">Instância da apresentacao a ser atualizada.</param>
    //public async Task UpdateAsync(Apresentacao apresentacao)
    //{
    //  _context.Apresentacoes.Update(apresentacao);
    //  await _context.SaveChangesAsync();
    //}
    
    /// Verifica se existe pelo menos uma Apresentacao na base de dados.
    /// <returns>True se existir pelo menos uma Apresentacao, false caso contrário.</returns>
    //public async Task<bool> AnyAsync()
    //  => await _context.Apresentacoes.AnyAsync();
    
    /// Lista todas as apresentações associadas a um cliente específico, identificando-o pelo seu ID.
    /// <param name="idCliente"></param>
    /// <returns></returns>
    //public async Task<List<Apresentacao>> GetByClienteIdAsync(int idCliente)
    //  => await _context.Apresentacoes
    //      .Where(a => a.IdCliente == idCliente)
    //      .ToListAsync();
    
    /// Lista todas as apresentações associadas a um talento específico, identificando-o pelo seu ID.
    /// <param name="idTalento"></param>
    /// <returns></returns>
    //public async Task<List<Apresentacao>> GetByTalentoIdAsync(int idTalento)
      //  => await _context.Apresentacoes
        //    .Where(a => a.IdTalento == idTalento)
          //  .ToListAsync();

//}