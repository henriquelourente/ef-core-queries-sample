using System;

namespace EFCoreSample.Entidades
{
    public class Funcionario
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public DateTime DataAdmissao { get; set; }
        public Guid DepartamentoId { get; set; }
        public bool Excluido { get; set; }
        public virtual Departamento Departamento { get; set; }

        public Funcionario()
        {

        }
    }
}
