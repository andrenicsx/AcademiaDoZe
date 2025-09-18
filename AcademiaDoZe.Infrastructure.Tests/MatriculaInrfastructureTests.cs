// Roberto Antunes Souza
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Repositories;
using AcademiaDoZe.Infrastructure.Tests;

namespace AcademiaDoZe.Infrastructure.Tests;

public class MatriculaInfrastructureTests : TestBase
{
    // Função para gerar CPF válido com 11 dígitos numéricos
    private static string GerarCpfValido()
    {
        var random = new Random();
        return string.Concat(Enumerable.Range(0, 11).Select(_ => random.Next(0, 10).ToString()));
    }

    // Cria um aluno de teste com CPF válido
    private async Task<Aluno> CriarAlunoTeste()
    {
        var repoAluno = new AlunoRepository(ConnectionString, DatabaseType);
        var repoLogradouro = new LogradouroRepository(ConnectionString, DatabaseType);

        var logradouro = await repoLogradouro.ObterPorId(4);
        Assert.NotNull(logradouro);

        var arquivo = Arquivo.Criar(new byte[] { 1, 2, 3 });
        var cpf = GerarCpfValido();

        var aluno = Aluno.Criar(
            0,
            "Aluno Teste",
            cpf,
            new DateOnly(2010, 10, 09),
            "49999999999",
            "aluno@teste.com",
            logradouro!,
            "123",
            "Complemento casa",
            "Senha@123",
            arquivo
        );

        await repoAluno.Adicionar(aluno);
        return aluno;
    }

    // Cria uma matrícula de teste para um aluno
    private async Task<Matricula> CriarMatriculaTeste(Aluno aluno)
    {
        var repoMatricula = new MatriculaRepository(ConnectionString, DatabaseType);
        var arquivo = Arquivo.Criar(new byte[] { 1, 2, 3 });

        var matricula = Matricula.Criar(
            0,
            aluno,
            EMatriculaPlano.Semestral,
            DateOnly.FromDateTime(DateTime.Today),
            DateOnly.FromDateTime(DateTime.Today.AddMonths(6)),
            "Emagrecer",
            EMatriculaRestricoes.Alergias,
            arquivo,
            "Sem observações"
        );

        await repoMatricula.Adicionar(matricula);
        return matricula;
    }

    [Fact]
    public async Task Matricula_Adicionar()
    {
        var aluno = await CriarAlunoTeste();
        var matricula = await CriarMatriculaTeste(aluno);

        Assert.NotNull(matricula);
        Assert.True(matricula.Id > 0);
    }

    [Fact]
    public async Task Matricula_ObterPorAluno_Atualizar()
    {
        var aluno = await CriarAlunoTeste();
        var matricula = await CriarMatriculaTeste(aluno);
        var repoMatricula = new MatriculaRepository(ConnectionString, DatabaseType);

        var arquivo = Arquivo.Criar(new byte[] { 1, 2, 3 });
        var matriculaAtualizada = Matricula.Criar(
            0,
            aluno,
            EMatriculaPlano.Anual,
            new DateOnly(2020, 05, 20),
            new DateOnly(2020, 05, 20).AddMonths(12),
            "Hipertrofia",
            EMatriculaRestricoes.Alergias,
            arquivo,
            "Observação atualizada"
        );

        typeof(Entity).GetProperty("Id")?.SetValue(matriculaAtualizada, matricula.Id);

        var resultado = await repoMatricula.Atualizar(matriculaAtualizada);

        Assert.NotNull(resultado);
        Assert.Equal("Hipertrofia", resultado.Objetivo);
        Assert.Equal("Observação atualizada", resultado.ObservacoesRestricoes);
        Assert.Equal(EMatriculaPlano.Anual, resultado.Plano);
    }

    [Fact]
    public async Task Matricula_ObterPorAluno_Remover_ObterPorId()
    {
        var aluno = await CriarAlunoTeste();
        var matricula = await CriarMatriculaTeste(aluno);
        var repoMatricula = new MatriculaRepository(ConnectionString, DatabaseType);

        // Remover
        var resultadoRemocao = await repoMatricula.Remover(matricula.Id);
        Assert.True(resultadoRemocao);

        // Verificar se foi removida
        var matriculaRemovida = await repoMatricula.ObterPorId(matricula.Id);
        Assert.Null(matriculaRemovida);
    }

    [Fact]
    public async Task Matricula_ObterTodos()
    {
        var aluno = await CriarAlunoTeste();
        await CriarMatriculaTeste(aluno);

        var repoMatricula = new MatriculaRepository(ConnectionString, DatabaseType);
        var resultado = await repoMatricula.ObterTodos();

        Assert.NotNull(resultado);
        Assert.NotEmpty(resultado);
    }

    [Fact]
    public async Task Matricula_ObterPorId()
    {
        var aluno = await CriarAlunoTeste();
        var matricula = await CriarMatriculaTeste(aluno);
        var repoMatricula = new MatriculaRepository(ConnectionString, DatabaseType);

        var matriculaPorId = await repoMatricula.ObterPorId(matricula.Id);

        Assert.NotNull(matriculaPorId);
        Assert.Equal(matricula.Id, matriculaPorId.Id);
    }
}
