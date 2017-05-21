using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using L9;

namespace TestProject
{
    [TestClass]
    public class TestClass
    {
        [TestMethod]
        public void TestOfTest()
        {
            Assert.AreNotEqual(5, 6);
           
        }
        [TestMethod]
        public void CanUseContainer()
        {
            SimplyContainer sc = new SimplyContainer();
            
            Assert.IsNotNull(sc);
        }
    }

    [TestClass]
    public class TestResolve
    {
        [TestMethod]
        public void Dependency()
        {
            SimplyContainer sc = new SimplyContainer();
            sc.RegisterType<IFoo, Foo>(false);
            Assert.IsInstanceOfType(sc.Resolve<IFoo>(), typeof(Foo));
        }
        [TestMethod]
        public void NotRegisteredDependency()
        {
            SimplyContainer sc = new SimplyContainer();
            try
            {
                var f = sc.Resolve<IFoo>();
                Assert.Fail("Not registered dependency");
            }
            catch (UnresolveableTypeException)
            {
                // check exception message/trace?
            }
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
        public void DependencySingleton()
        {
            SimplyContainer sc = new SimplyContainer();
            sc.RegisterType<IFoo, Foo>(true);
            Assert.AreEqual(sc.Resolve<IFoo>(), sc.Resolve<IFoo>());
        }
        [TestMethod]
        public void UnregisteredType()
        {
            SimplyContainer sc = new SimplyContainer();

            Foo f1 = sc.Resolve<Foo>();
            Foo f2 = sc.Resolve<Foo>();

            Assert.AreNotSame(f1, f2);
            Assert.IsInstanceOfType(f1, typeof(Foo));
            Assert.IsInstanceOfType(f2, typeof(Foo));

            try
            {
                Bar b = sc.Resolve<Bar>();
                Assert.Fail("Bar does not have any constructor");
            }
            catch (UnresolveableTypeException)
            {
                // check exception message/trace?
            }

            try
            {
                Qux q = sc.Resolve<Qux>();
                Assert.Fail("Qux does not have default constructor");
            }
            catch (UnresolveableTypeException)
            {
                // check exception message/trace?
            }
        }
    }

    interface IFoo { }
    class Foo : IFoo { }  // with default constructor

    class Bar
    {
        private Bar() { }
    }

    class Qux
    {
        public Qux(int xd) { }
    }
}
