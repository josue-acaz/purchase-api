using System;
using System.Collections;
using System.Collections.Generic;

namespace Voartec.Validators
{
    public class Fluent
    {
        /// <summary>: Verifica se um valor é nulo
        public bool IsNull(dynamic value)
        {
            return value == null;
        }
         /// <summary>: Verifica se um valor é vazio
        public bool IsEmpty(dynamic value)
        {
            return String.IsNullOrEmpty(value.ToString());
        }
         /// <summary>: Valida o tamanho do valor informado
        public bool HasMaxLen(dynamic value, int max_length)
        {
            return String.IsNullOrEmpty(value.ToString()) ? true : ((value.ToString().Length > max_length) ? false : true);
        }
    }
}