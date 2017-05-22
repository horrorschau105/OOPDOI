using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using L9;

namespace TestProject
{
    [TestClass]
    public class TestResolve
    {
        [TestMethod]
        public void ContainerNotNull()
        {
            SimplyContainer sc = new SimplyContainer();
            Assert.IsNotNull(sc);
        }
        [TestMethod]
        public void SimpleClassResolve()
        {
            SimplyContainer sc = new SimplyContainer();
            sc.RegisterType<Foo>(true);
            Assert.IsInstanceOfType(sc.Resolve<Foo>(), typeof(Foo));
        }
        [TestMethod]
        public void SimpleDependency()
        {
            SimplyContainer sc = new SimplyContainer();
            sc.RegisterType<IFoo, Foo>(false);
            Assert.IsInstanceOfType(sc.Resolve<IFoo>(), typeof(Foo));
        }
        [TestMethod]
        [ExpectedException(typeof(UnresolveableTypeException))]
        public void NotRegisteredDependency()
        {
            SimplyContainer sc = new SimplyContainer();
            var f = sc.Resolve<Foo>();
        }
        [TestMethod]
        public void ConcreteSingleton()
        {
            SimplyContainer sc = new SimplyContainer();
            sc.RegisterType<Foo>(true);
            var f1 = sc.Resolve<Foo>();
            var f2 = sc.Resolve<Foo>();
            Assert.AreEqual(f1, f2);
        }
        [TestMethod]
        public void NotSingleton()
        {
            SimplyContainer sc = new SimplyContainer();
            sc.RegisterType<Foo>(false);
            var f1 = sc.Resolve<Foo>();
            var f2 = sc.Resolve<Foo>();
            Assert.AreNotEqual(f1, f2);
        }
        [TestMethod]
        public void DependencySingleton()
        {
            SimplyContainer sc = new SimplyContainer();
            sc.RegisterType<IFoo, Foo>(true);
            Assert.AreEqual(sc.Resolve<IFoo>(), sc.Resolve<IFoo>());
        }
        [TestMethod]
        public void DoubleDeriving()
        {
            var sc = new SimplyContainer();
            sc.RegisterType<IFoo, IBur>(false);
            sc.RegisterType<IBur, Bur>(false);
            var bur = sc.Resolve<IFoo>();
            Assert.IsInstanceOfType(bur, typeof(Bur));
        }
        [TestMethod]
        public void ManipulatingSingletonPolicy()
        {
            var sc = new SimplyContainer();
            sc.RegisterType<Foo>(true);  
            var o2 = sc.Resolve<Foo>();
            sc.RegisterType<Foo>(false);
            var o3 = sc.Resolve<Foo>();
            Assert.AreNotEqual(o2, o3); 
        }
        [TestMethod]
        public void ManipulatingDependencies()
        {
            var sc = new SimplyContainer();
            sc.RegisterType<IFoo, Foo>(true);
            Foo o1 = (Foo)sc.Resolve<IFoo>();
            sc.RegisterType<IFoo, Fux>(true);
            Fux o2 = (Fux)sc.Resolve<IFoo>();
            Assert.AreNotEqual(o1, o2);  
        }
        [TestMethod]
        public void ManipulateSingletonPolicyAndDependencies()
        {
            var sc = new SimplyContainer();
            sc.RegisterType<IFoo, Foo>(false);
            var o1 = sc.Resolve<IFoo>();
            var o2 = sc.Resolve<IFoo>();
            sc.RegisterType<IFoo, Fux>(true);
            var o3 = sc.Resolve<IFoo>();
            var o4 = sc.Resolve<IFoo>();
            Assert.IsTrue(!o1.Equals(o2) && o3.Equals(o4) && o2.GetType() != o3.GetType());
        }
    }
    interface IFoo { }
    interface IBur : IFoo { }
    class Bur : IBur { }
    class Foo : IFoo { }  // with default constructor
    class Fux : IFoo { }
    class Bar
    {
        private Bar() { }
    }

    class Qux
    {
        public Qux(int xd) { }
    }
}
