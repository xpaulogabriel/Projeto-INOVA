using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InovaLog.Models;
using InovaLog.Services;
using InovaLog.Views;
using Microsoft.Maui.Graphics;

namespace InovaLog.ViewModels;

public partial class UsuarioItem : ObservableObject
{
    public string Id { get; init; } = string.Empty;
    public string NomeExibicao { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Documentos { get; init; } = string.Empty;
    public string RotuloPerfil { get; init; } = string.Empty;
    public Usuario ModelOriginal { get; init; } = null!;

    [ObservableProperty] private bool ativo = true;

    public bool TemDocumentos => !string.IsNullOrWhiteSpace(Documentos);

    public string TextoStatus => Ativo ? "Ativo" : "Bloqueado";
    public Color CorFundoStatus => Ativo ? Color.FromArgb("#D1FAE5") : Color.FromArgb("#FEE2E2");
    public Color CorTextoStatus => Ativo ? Color.FromArgb("#10B981") : Color.FromArgb("#EF4444");

    public string TextoAcao => Ativo ? "Bloquear" : "Ativar";
    public Color CorFundoAcao => Ativo ? Color.FromArgb("#FEE2E2") : Color.FromArgb("#D1FAE5");
    public Color CorTextoAcao => Ativo ? Color.FromArgb("#EF4444") : Color.FromArgb("#10B981");

    partial void OnAtivoChanged(bool value)
    {
        OnPropertyChanged(nameof(TextoStatus));
        OnPropertyChanged(nameof(CorFundoStatus));
        OnPropertyChanged(nameof(CorTextoStatus));
        OnPropertyChanged(nameof(TextoAcao));
        OnPropertyChanged(nameof(CorFundoAcao));
        OnPropertyChanged(nameof(CorTextoAcao));
    }
}

public class CategoriaItem : ObservableObject
{
    private bool _ativa = true;

    public string Nome { get; init; } = string.Empty;
    public bool Entrada { get; init; }

    public bool Ativa
    {
        get => _ativa;
        set
        {
            if (SetProperty(ref _ativa, value))
            {
                OnPropertyChanged(nameof(TextoBotao));
                OnPropertyChanged(nameof(CorFundoBotao));
                OnPropertyChanged(nameof(CorTextoBotao));
                OnPropertyChanged(nameof(OpacidadeLinha));
            }
        }
    }

    public string TipoTexto => Entrada ? "ENTRADA" : "SAÍDA";
    public Color CorFundoTipo => Entrada ? Color.FromArgb("#D1FAE5") : Color.FromArgb("#FEE2E2");
    public Color CorTextoTipo => Entrada ? Color.FromArgb("#10B981") : Color.FromArgb("#EF4444");

    public string TextoBotao => Ativa ? "Inativar" : "Reativar";
    public Color CorFundoBotao => Ativa ? Color.FromArgb("#FEE2E2") : Color.FromArgb("#D1FAE5");
    public Color CorTextoBotao => Ativa ? Color.FromArgb("#EF4444") : Color.FromArgb("#10B981");
    public double OpacidadeLinha => Ativa ? 1 : 0.55;
}

public class FormaPagamentoItem : ObservableObject
{
    private bool _ativa = true;

    public string Nome { get; init; } = string.Empty;

    public bool Ativa
    {
        get => _ativa;
        set
        {
            if (SetProperty(ref _ativa, value))
            {
                OnPropertyChanged(nameof(TextoStatus));
                OnPropertyChanged(nameof(CorFundoStatus));
                OnPropertyChanged(nameof(CorTextoStatus));
                OnPropertyChanged(nameof(OpacidadeLinha));
            }
        }
    }

    public string TextoStatus => Ativa ? "Ativo" : "Inativo";
    public Color CorFundoStatus => Ativa ? Color.FromArgb("#D1FAE5") : Color.FromArgb("#F1F5F9");
    public Color CorTextoStatus => Ativa ? Color.FromArgb("#10B981") : Color.FromArgb("#64748B");
    public double OpacidadeLinha => Ativa ? 1 : 0.55;
}

public partial class ConfiguracoesViewModel : ObservableObject
{
    private readonly DatabaseService _db;
    private readonly Supabase.Client _supabaseClient;

    public ObservableCollection<UsuarioItem> Usuarios { get; } = new();

    public ObservableCollection<CategoriaItem> Categorias { get; } = new()
    {
        new CategoriaItem { Nome = "Fretes", Entrada = true },
        new CategoriaItem { Nome = "Combustível" },
        new CategoriaItem { Nome = "Pedágio" },
        new CategoriaItem { Nome = "Salário Motorista" },
        new CategoriaItem { Nome = "Financiamento" },
        new CategoriaItem { Nome = "Manutenção" }
    };

    [ObservableProperty] private string novaCategoria = string.Empty;
    [ObservableProperty] private int tipoCategoriaIndex = 1;

    public ObservableCollection<FormaPagamentoItem> FormasPagamento { get; } = new()
    {
        new FormaPagamentoItem { Nome = "PIX" },
        new FormaPagamentoItem { Nome = "Boleto" },
        new FormaPagamentoItem { Nome = "Cartão Frota" },
        new FormaPagamentoItem { Nome = "Dinheiro" }
    };

    [ObservableProperty] private string novaFormaPagamento = string.Empty;

    [ObservableProperty] private string nomeConta = string.Empty;
    [ObservableProperty] private string emailConta = string.Empty;
    [ObservableProperty] private string rotuloPerfilConta = string.Empty;
    [ObservableProperty] private bool carregando;

    public ConfiguracoesViewModel(DatabaseService db, Supabase.Client supabaseClient)
    {
        _db = db;
        _supabaseClient = supabaseClient;
    }

    public async Task CarregarDadosAsync()
    {
        NomeConta = SessaoPerfil.NomeUsuario;
        EmailConta = SessaoPerfil.EmailUsuario;
        RotuloPerfilConta = SessaoPerfil.RotuloPerfil(SessaoPerfil.PerfilAtual);

        Carregando = true;
        try
        {
            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                var respPerfis = await _supabaseClient.From<Perfil>().Get();
                var respUsuarios = await _supabaseClient.From<Usuario>().Get();
                var respMotoristas = await _supabaseClient.From<Motorista>().Get();

                await _db.ReplaceAllSyncedAsync(respPerfis.Models);
                await _db.ReplaceAllSyncedAsync(respUsuarios.Models);
                await _db.ReplaceAllSyncedAsync(respMotoristas.Models);
            }

            var perfis = (await _db.GetAllAsync<Perfil>()).ToDictionary(p => p.Id, p => p.Nome);
            var motoristas = (await _db.GetAllAsync<Motorista>()).ToDictionary(m => m.UsuarioId, m => m);
            var usuarios = await _db.GetAllAsync<Usuario>();

            Usuarios.Clear();
            foreach (var u in usuarios.OrderBy(x => x.Nome))
            {
                var nomePerfil = u.PerfilId.HasValue && perfis.TryGetValue(u.PerfilId.Value, out var np)
                    ? np.ToUpperInvariant()
                    : "USUÁRIO";

                var docs = string.Empty;
                if (motoristas.TryGetValue(u.Id, out var mot))
                {
                    docs = $"CNH: {mot.CnhNumero} (Cat {mot.CnhCategoria})";
                    if (!string.IsNullOrWhiteSpace(u.CpfCnpj))
                        docs += $" • CPF: {u.CpfCnpj}";
                }
                else if (!string.IsNullOrWhiteSpace(u.CpfCnpj))
                {
                    docs = $"CPF/CNPJ: {u.CpfCnpj}";
                }

                Usuarios.Add(new UsuarioItem
                {
                    Id = u.Id,
                    NomeExibicao = u.Nome,
                    Email = u.Email,
                    RotuloPerfil = nomePerfil,
                    Documentos = docs,
                    Ativo = u.Ativo ?? true,
                    ModelOriginal = u
                });
            }
        }
        catch (Exception ex)
        {
            await ExibirAlerta("Erro", $"Não foi possível carregar os usuários: {ex.Message}");
        }
        finally
        {
            Carregando = false;
        }
    }

    [RelayCommand]
    private async Task NovoUsuarioAsync()
    {
        await Shell.Current.GoToAsync(nameof(CadastroUsuarioPage));
    }

    [RelayCommand]
    private async Task EditarUsuarioAsync(UsuarioItem? item)
    {
        if (item == null) return;
        await Shell.Current.GoToAsync($"{nameof(CadastroUsuarioPage)}?UsuarioId={item.Id}&Modo=editar");
    }

    [RelayCommand]
    private async Task InspecionarUsuarioAsync(UsuarioItem? item)
    {
        if (item == null) return;
        await Shell.Current.GoToAsync($"{nameof(CadastroUsuarioPage)}?UsuarioId={item.Id}&Modo=inspecionar");
    }

    [RelayCommand]
    private async Task AlternarStatusUsuarioAsync(UsuarioItem? item)
    {
        if (item == null) return;

        if (string.Equals(item.Email, SessaoPerfil.EmailUsuario, StringComparison.OrdinalIgnoreCase))
        {
            await ExibirAlerta("Ação bloqueada", "Você não pode bloquear a sua própria conta de administrador.");
            return;
        }

        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            await ExibirAlerta("Sem conexão", "Conecte-se à internet para alterar o status de acesso.");
            return;
        }

        try
        {
            var novoStatus = !item.Ativo;
            item.ModelOriginal.Ativo = novoStatus;
            item.ModelOriginal.AtualizadoEm = DateTime.UtcNow;

            await _supabaseClient.From<Usuario>().Update(item.ModelOriginal);
            await _db.SaveAsync(item.ModelOriginal);

            item.Ativo = novoStatus;
        }
        catch (Exception ex)
        {
            await ExibirAlerta("Erro", $"Falha ao atualizar status: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task ExcluirUsuarioAsync(UsuarioItem? item)
    {
        if (item == null) return;

        if (string.Equals(item.Email, SessaoPerfil.EmailUsuario, StringComparison.OrdinalIgnoreCase))
        {
            await ExibirAlerta("Ação bloqueada", "Você não pode excluir a sua própria conta de administrador.");
            return;
        }

        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            await ExibirAlerta("Sem conexão", "Conecte-se à internet para excluir usuários.");
            return;
        }

        var pagina = Application.Current?.Windows[0].Page;
        if (pagina == null) return;

        bool confirmar = await pagina.DisplayAlertAsync(
            "Confirmar Exclusão",
            $"Deseja excluir permanentemente o usuário {item.NomeExibicao} ({item.Email})?",
            "Excluir",
            "Cancelar");

        if (!confirmar) return;

        try
        {
            // 1. Apaga no Supabase (o ON DELETE CASCADE já remove da tabela 'motoristas')
            await _supabaseClient.From<Usuario>().Delete(item.ModelOriginal);

            // 2. Apaga no SQLite local (usuário e eventual registro de motorista)
            await _db.DeleteAsync(item.ModelOriginal);
            var motoristas = await _db.GetAllAsync<Motorista>();
            var motLocal = motoristas.FirstOrDefault(m => m.UsuarioId == item.Id);
            if (motLocal != null)
                await _db.DeleteAsync(motLocal);

            // 3. Remove da lista na tela
            Usuarios.Remove(item);
        }
        catch (Exception ex)
        {
            // Se o usuário já tiver viagens/movimentações vinculadas, o Postgres bloqueia por FK
            await ExibirAlerta(
                "Não foi possível excluir",
                $"Erro ao excluir usuário (se ele já possui histórico vinculado, use a opção Bloquear): {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task IncluirCategoriaAsync()
    {
        var nome = (NovaCategoria ?? string.Empty).Trim();
        if (nome.Length == 0)
        {
            await ExibirAlerta("Atenção", "Informe o nome da categoria.");
            return;
        }

        if (Categorias.Any(c => string.Equals(c.Nome, nome, StringComparison.OrdinalIgnoreCase)))
        {
            await ExibirAlerta("Atenção", "Essa categoria já existe.");
            return;
        }

        Categorias.Add(new CategoriaItem { Nome = nome, Entrada = TipoCategoriaIndex == 0 });
        NovaCategoria = string.Empty;
    }

    [RelayCommand]
    private void AlternarCategoria(CategoriaItem? categoria)
    {
        if (categoria != null)
            categoria.Ativa = !categoria.Ativa;
    }

    [RelayCommand]
    private async Task IncluirFormaPagamentoAsync()
    {
        var nome = (NovaFormaPagamento ?? string.Empty).Trim();
        if (nome.Length == 0)
        {
            await ExibirAlerta("Atenção", "Informe a forma de pagamento.");
            return;
        }

        if (FormasPagamento.Any(f => string.Equals(f.Nome, nome, StringComparison.OrdinalIgnoreCase)))
        {
            await ExibirAlerta("Atenção", "Essa forma de pagamento já existe.");
            return;
        }

        FormasPagamento.Add(new FormaPagamentoItem { Nome = nome });
        NovaFormaPagamento = string.Empty;
    }

    [RelayCommand]
    private void AlternarFormaPagamento(FormaPagamentoItem? forma)
    {
        if (forma != null)
            forma.Ativa = !forma.Ativa;
    }

    private static async Task ExibirAlerta(string titulo, string mensagem)
    {
        var pagina = Application.Current?.Windows[0].Page;
        if (pagina != null)
            await pagina.DisplayAlertAsync(titulo, mensagem, "OK");
    }
}