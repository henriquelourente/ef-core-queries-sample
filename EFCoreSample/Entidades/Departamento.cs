using System;
using System.Collections.Generic;

namespace EFCoreSample.Entidades
{
    //public class Departamento
    //{
    //    public Guid Id { get; set; }
    //    public string Descricao { get; set; }
    //    public bool Excluido { get; set; }
    //    public virtual IEnumerable<Funcionario> Funcionarios { get; set; }

    //    public Departamento()
    //    {

    //    }
    //}

    public class Departamento
    {
        public Guid Id { get; private set; }
        public string Descricao { get; private set; }
        public bool Excluido { get; private set; }
        public virtual IEnumerable<Funcionario> Funcionarios { get; private set; }

        protected Departamento()
        {

        }

        public Departamento(string descricao)
        {
            this.Descricao = descricao;
        }
    }
}
