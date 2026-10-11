using Postgrest.Models;
using SQLite;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace InovaLog.Models;

[Supabase.Postgrest.Attributes.Table("usuarios")]
[SQLite.Table("usuarios")]
public class Usuario : BaseModel
{
    // Usamos Guid (UUID) porque no Offline-First o próprio celular pode gerar o ID 
    // sem correr o risco de colidir com IDs gerados na nuvem.
    [Supabase.Postgrest.Attributes.PrimaryKey("id", false)]
    [SQLite.PrimaryKey]
    public Guid Id { get; set; } = Guid.NewGuid();

    // ================= DADOS BÁSICOS (OBRIGATÓRIOS) =================

    [Supabase.Postgrest.Attributes.Column("nome_completo")]
    [SQLite.MaxLength(150), SQLite.NotNull]
    public string NomeCompleto { get; set; } = string.Empty;

    [Supabase.Postgrest.Attributes.Column("email")]
    [SQLite.Unique, SQLite.NotNull]
    public string Email { get; set; } = string.Empty;

    // Valores esperados: "Administrador", "Gestor", "Financeiro", "Motorista"
    [Supabase.Postgrest.Attributes.Column("perfil_acesso")]
    [SQLite.NotNull]
    public string PerfilAcesso { get; set; } = string.Empty;

    // ================= DADOS DE MOTORISTA (OPCIONAIS NA TABELA) =================
    // Usamos '?' (Nullable) pois Administradores e Gestores terão esses campos vazios no banco.

    [Supabase.Postgrest.Attributes.Column("cpf")]
    [SQLite.MaxLength(14)] // Formato: 000.000.000-00
    public string? Cpf { get; set; }

    [Supabase.Postgrest.Attributes.Column("data_nascimento")]
    public DateTime? DataNascimento { get; set; }

    [Supabase.Postgrest.Attributes.Column("numero_cnh")]
    [SQLite.MaxLength(20)]
    public string? NumeroCnh { get; set; }

    // Valores esperados: "C", "D", "E"
    [Supabase.Postgrest.Attributes.Column("categoria_cnh")]
    [SQLite.MaxLength(5)]
    public string? CategoriaCnh { get; set; }

    [Supabase.Postgrest.Attributes.Column("validade_cnh")]
    public DateTime? ValidadeCnh { get; set; }

    // ================= RODAPÉ E CONTROLE =================

    // Já inicializa como 'true' para a chavinha (Switch) vir ligada por padrão na tela
    [Supabase.Postgrest.Attributes.Column("ativo")]
    public bool Ativo { get; set; } = true;

    // Controle essencial para o Offline-First saber se esse registro precisa subir pra nuvem
    [Supabase.Postgrest.Attributes.Ignore] // O Supabase não precisa saber dessa coluna local
    public bool Sincronizado { get; set; } = false;

    // ================= REGRA DE NEGÓCIO EMBUTIDA =================

    /// <summary>
    /// Valida se os campos obrigatórios foram preenchidos, 
    /// exigindo os dados extras caso o perfil seja Motorista.
    /// </summary>
    public bool ValidarCadastro(out string mensagemErro)
    {
        if (string.IsNullOrWhiteSpace(NomeCompleto) ||
            string.IsNullOrWhiteSpace(Email) ||
            string.IsNullOrWhiteSpace(PerfilAcesso))
        {
            mensagemErro = "Preencha todos os dados básicos (Nome, E-mail e Perfil).";
            return false;
        }

        if (PerfilAcesso == "Motorista")
        {
            if (string.IsNullOrWhiteSpace(Cpf) ||
                DataNascimento == null ||
                string.IsNullOrWhiteSpace(NumeroCnh) ||
                string.IsNullOrWhiteSpace(CategoriaCnh) ||
                ValidadeCnh == null)
            {
                mensagemErro = "Para o perfil Motorista, todos os documentos (CPF, Nascimento e dados da CNH) são obrigatórios.";
                return false;
            }
        }

        mensagemErro = string.Empty;
        return true;
    }
}