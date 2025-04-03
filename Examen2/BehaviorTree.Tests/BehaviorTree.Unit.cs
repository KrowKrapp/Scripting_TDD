using NUnit.Framework;
using BehaviorTree;
using System;

namespace BehaviorTree.UnitTests
{
    [TestFixture]
    public class BehaviorTree_Tests
    {
        [SetUp]
        public void SetUp()
        {
            // Configuración previa a cada prueba por si algo
        }

        [Test]
        public void BehaviorTree_HasOnlyOneRoot()
        {
            var root = new Root(new MoveTask(() => true));
            Assert.That(root, Is.Not.Null);
        }

        [Test]
        public void Root_HasOnlyOneChild_AndNotAnotherRoot()
        {
            var task = new MoveTask(() => true);
            var root = new Root(task);
            Assert.That(root, Is.Not.Null);
            Assert.Throws<InvalidOperationException>(() => new Root(new Root(task)));
        }

        [Test]
        public void Composite_CannotBeInstantiated()
        {
            Assert.Throws<TypeInitializationException>(() => Activator.CreateInstance(typeof(Composite)));
        }

        [Test]
        public void Composite_CannotHaveRootAsChild()
        {
            var task = new MoveTask(() => true);
            var root = new Root(task);
            Assert.Throws<InvalidOperationException>(() => new Sequence(root));
        }

        [Test]
        public void Task_CannotBeInstantiatedDirectly()
        {
            Assert.Throws<TypeInitializationException>(() => Activator.CreateInstance(typeof(Task)));
        }

        [Test]
        public void Task_HasNoChildren()
        {
            var task = new MoveTask(() => true);
            Assert.That(task, Is.Not.Null);
        }

        [Test]
        public void BehaviorTree_HierarchyIsCorrect()
        {
            Assert.That(typeof(Node).IsAssignableFrom(typeof(Composite)), Is.True);
            Assert.That(typeof(Node).IsAssignableFrom(typeof(Task)), Is.True);
            Assert.That(typeof(Composite).IsAssignableFrom(typeof(Sequence)), Is.True);
            Assert.That(typeof(Composite).IsAssignableFrom(typeof(Selector)), Is.True);
            Assert.That(typeof(Composite).IsAssignableFrom(typeof(Task)), Is.False);
            Assert.That(typeof(Root).IsAssignableFrom(typeof(Task)), Is.False);
        }

        [Test]
        public void EmptyRoot_ReturnsFalse()
        {
            var root = new Root(null);
            Assert.That(root.Execute(), Is.False);
        }

        [Test]
        public void Root_WithTask_ReturnsTaskExecution()
        {
            var root = new Root(new MoveTask(() => true));
            Assert.That(root.Execute(), Is.True);
        }

        [Test]
        public void Sequence_WithTasks_ReturnsExpectedResults()
        {
            var sequence = new Sequence(new MoveTask(() => true), new MoveTask(() => false));
            Assert.That(sequence.Execute(), Is.False);
        }

        [Test]
        public void Selector_WithTasks_ReturnsExpectedResults()
        {
            var selector = new Selector(null, new MoveTask(() => false), new MoveTask(() => true));
            Assert.That(selector.Execute(), Is.True);
        }
    }
}
