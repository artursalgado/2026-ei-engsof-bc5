namespace GestaoTalentos.Domain;

/// Estado atual de uma proposta de trabalho.
/// Aberta: proposta visível e à espera de talento.
/// EmNegociacao: contacto iniciado com um ou mais talentos.
/// Fechada: talento selecionado, proposta concluída.
public enum EstadoProposta
{
    Aberta = 0,
    EmNegociacao = 1,
    Fechada = 2
}
