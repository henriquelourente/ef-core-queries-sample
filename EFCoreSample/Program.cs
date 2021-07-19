using EFCoreSample.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.Linq;

namespace EFCoreSample
{
    class Program
    {
        static void Main(string[] args)
        {
            Menu();
        }

        private static void Menu()
        {
            WriteMenu();

            var key = Console.ReadKey();

            while (key.Key != ConsoleKey.Escape)
            {
                switch (key.KeyChar)
                {
                    case '1':
                        new MyContext().SeedData();
                        break;
                    case '2':
                        GerenciarConexoes(false);
                        GerenciarConexoes(true);
                        break;
                    case '3':
                        CarregamentoAdiantado();
                        break;
                    case '4':
                        CarregamentoExplicito();
                        break;
                    case '5':
                        CarregamentoLento();
                        break;
                    case '6':
                        ConsultaProjetada();
                        break;
                    case '7':
                        DivisaoDeConsultas();
                        break;
                    case '8':
                        ConsultandoPropriedadeSombra();
                        break;
                    case '9':
                        ConsultaRastreada();
                        break;
                    case 'a':
                        ConsultaNaoRastreada();
                        break;
                    default: break;
                }

                WriteMenu();
                key = Console.ReadKey();
            }
        }

        private static void ConsultaNaoRastreada()
        {
            using var db = new MyContext();

            //var func = db.Funcionarios.First();
            //func.Nome = "Alterando Entidade";
            //var entidadesModificadas = db.ChangeTracker.Entries().Where(x => x.State == EntityState.Modified);

            var departamentos = db.Funcionarios.AsNoTrackingWithIdentityResolution().Include(x => x.Departamento).ToList();
        }

        private static void ConsultaRastreada()
        {
            using var db = new MyContext();
            var departamento = db.Funcionarios.Include(x => x.Departamento).ToList();
        }

        private static void WriteMenu()
        {
            // Console.Clear();
            Console.WriteLine("Menu: ");
            Console.WriteLine("1: Seed");
            Console.WriteLine("2: Gerenciar Estado das Conexões");
            Console.WriteLine("3: Carregamento Adiantado");
            Console.WriteLine("4: Carregamento Explícito");
            Console.WriteLine("5: Carregamento Lento");
            Console.WriteLine("6: Consulta Projetada");
            Console.WriteLine("7: Divisão de Consultas");
            Console.WriteLine("8: Propriedades de sombra");
            Console.WriteLine("9: Consulta rastreada");
            Console.WriteLine("a: Consulta não rastreada");
        }

        private static void CarregamentoAdiantado()
        {
            using var db = new MyContext();
            var departamentos = db.Departamentos.Include(d => d.Funcionarios);
            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Departamento: {departamento.Descricao}");
                foreach (var funcionario in departamento.Funcionarios)
                {
                    Console.WriteLine($"Funcionário: {funcionario.Nome}");
                }
            }
        }

        private static void CarregamentoExplicito()
        {
            using var db = new MyContext();
            var departamentos = db.Departamentos.Take(4).ToList();
            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Departamento: {departamento.Descricao}");

                db.Entry(departamento).Collection(d => d.Funcionarios).Load();
                foreach (var funcionario in departamento.Funcionarios)
                {
                    Console.WriteLine($"Funcionário: {funcionario.Nome}");
                }
            }
        }

        private static void CarregamentoLento()
        {
            using var db = new MyContext();
            //db.ChangeTracker.LazyLoadingEnabled = false;
            var departamentos = db.Departamentos.Take(4).ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Departamento: {departamento.Descricao}");

                foreach (var funcionario in departamento.Funcionarios)
                {
                    Console.WriteLine($"Funcionário: {funcionario.Nome}");
                }
            }
        }


        private static void GerenciarConexoes(bool gerenciar)
        {
            var conexoes = 0;
            using var db = new MyContext();
            var conexao = db.Database.GetDbConnection();
            db.Funcionarios.Any();
            conexao.StateChange += (_, __) => ++conexoes;

            if (gerenciar)
            {
                conexao.Open();
            }

            var time = Stopwatch.StartNew();

            for (int i = 0; i < 5000; i++)
            {
                db.Funcionarios.AsNoTrackingWithIdentityResolution().First();
            }

            time.Stop();
            Console.WriteLine($"Tempo: {time.Elapsed}. Gerenciar: {gerenciar}. Conexões: {conexoes}");
        }

        private static void ConsultaProjetada()
        {
            using var context = new MyContext();
            var departamentos = context.Departamentos
                                    .Select(d => new
                                    {
                                        d.Descricao,
                                        Funcionarios = d.Funcionarios.Select(f => f.Nome)
                                    }).ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Departamento: {departamento.Descricao}");

                foreach (var funcionario in departamento.Funcionarios)
                {
                    Console.WriteLine($"\nFuncionário: {funcionario}");
                }
            }
        }

        private static void DivisaoDeConsultas()
        {
            using var db = new MyContext();
            var departamentos = db.Departamentos.AsNoTracking().Include(d => d.Funcionarios).AsSplitQuery();
            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Departamento: {departamento.Descricao}");
                foreach (var funcionario in departamento.Funcionarios)
                {
                    Console.WriteLine($"Funcionário: {funcionario.Nome}");
                }
            }
        }

        private static void ConsultandoPropriedadeSombra()
        {
            using var db = new MyContext();
            var departamentos = db.Departamentos.Where(d => EF.Property<DateTime>(d, "UltimaAtualizacao") < DateTime.Today).ToList();
        }
    }
}
