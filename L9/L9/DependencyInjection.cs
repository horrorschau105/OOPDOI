using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L9
{
    public class SimplyContainer
    {
        public SimplyContainer()
        {
            
        }
        public void RegisterType<T>(bool Singleton) where T : class
        {

        }
        public void RegisterType<From, To>(bool Singleton) where To : From
        {

        }
        public T Resolve<T>()
        {
            
        }
        
    }
}
