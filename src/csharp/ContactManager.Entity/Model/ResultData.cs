using System;
using System.Collections.Generic;
using System.Text;

namespace ContactManager.Entity.Model
{
    public class ResultData<T>
    {
        public bool IsSuccess { get; set; }
        // private data members 
        private T data;
        // using properties 
        public T Data
        {

            // using accessors 
            get
            {
                return this.data;
            }
            set
            {
                this.data = value;
            }
        }
        public string Message { get; set; }
        public System.Net.HttpStatusCode HttpCode { get; set; }
    }
}
