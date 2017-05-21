﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L9
{
    public class SimplyContainer // should it be singleton? -> I'd say: doesn't have to
    {
        Dictionary<Type, object> singletons; // for constructed singletons
        Dictionary<Type, bool> registeredTypes; //  foreach class remember if it wants singleton
        Dictionary<Type, Type> registeredDependencies; // foreach class/interface remember deriving class
        public SimplyContainer()
        {
            registeredDependencies = new Dictionary<Type, Type>();
            registeredTypes = new Dictionary<Type, bool>();
            singletons = new Dictionary<Type, object>();
        }
        public void RegisterType<T>(bool Singleton) where T : class
        {
            if (registeredTypes.ContainsKey(typeof(T))) registeredTypes[typeof(T)] = Singleton;
            else registeredTypes[typeof(T)] = Singleton;  // add/update settings of class
            // else registeredTypes.Add(typeof(T), Singleton);  // it adds or throws exeption if key exists, why this over indexer?
        }
        public void RegisterType<From, To>(bool Singleton) where To : class, From
        {
            if (registeredDependencies.ContainsKey(typeof(From))) registeredDependencies[typeof(From)] = typeof(To);
            else registeredDependencies[typeof(From)] = typeof(To); // same as upper
            // else registeredDependencies.Add(typeof(From), typeof(To)); // same as upper

            // register 'To' type too
            RegisterType<To>(Singleton);
        }
        public T Resolve<T>() where T : new() // T has default, nonparameter constructor
        {
            // TODO
            return new T();
           
        }
        
    }
}
