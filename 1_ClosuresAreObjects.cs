using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Closures
{
    public class CounterExample
    {

        // 1. What is a closure?
        public Func<int> MakeCounter()
        {
            var count = 0;
            return () => { count = count + 1; return count; };
        }


        [Fact]
        public void DemoCounterClosure()
        {
            var counter1 = MakeCounter();
            var counter2 = MakeCounter();

            counter1().Should().Be(1);
            counter1().Should().Be(2);
            counter1().Should().Be(3);

            counter2().Should().Be(1);
            counter2().Should().Be(2);

            counter1().Should().Be(4);
            counter2().Should().Be(3);
        }

        public class Counter
        {
            private int count = 0;

            public int Increment() { count = count + 1; return count; }
        }

        [Fact]
        public void DemoCounterObject()
        {
            var counter1 = new Counter();
            var counter2 = new Counter();

            counter1.Increment().Should().Be(1);
            counter1.Increment().Should().Be(2);
            counter1.Increment().Should().Be(3);

            counter2.Increment().Should().Be(1);
            counter2.Increment().Should().Be(2);

            counter1.Increment().Should().Be(4);
            counter2.Increment().Should().Be(3);
        }
    }

    public class AppenderExample
    {
        public Func<T, bool> MakeAppender<T>(List<T> appendTo, int capacity)
        {
            return (T item) =>
            {
                if (appendTo.Count >= capacity)
                {
                    return false;
                }
                else
                {
                    appendTo.Add(item);
                    return true;
                }
            };
        }

        [Fact]
        public void DemoAppenderClosure()
        {
            var coll = new List<int>();

            var appender1 = MakeAppender(coll, 3);

            var appender2 = MakeAppender(coll, 5);

            coll.Should().BeEquivalentTo(new List<int> {});

            appender1(1).Should().BeTrue();
            coll.Should().BeEquivalentTo(new List<int> { 1 });

            appender2(2).Should().BeTrue();
            coll.Should().BeEquivalentTo(new List<int> { 1, 2 });

            appender1(3).Should().BeTrue();
            coll.Should().BeEquivalentTo(new List<int> { 1, 2, 3 });

            appender1(4).Should().BeFalse();
            coll.Should().BeEquivalentTo(new List<int> { 1, 2, 3 });

            appender2(4).Should().BeTrue();
            coll.Should().BeEquivalentTo(new List<int> { 1, 2, 3, 4 });

            appender2(5).Should().BeTrue();
            coll.Should().BeEquivalentTo(new List<int> { 1, 2, 3, 4, 5 });

            appender2(6).Should().BeFalse();
            coll.Should().BeEquivalentTo(new List<int> { 1, 2, 3, 4, 5 });
        }

        public class Appender<T>
        {
            private readonly List<T> _items;
            private readonly int _capacity;

            public Appender(List<T> items, int capacity)
            {
                _items = items;
                _capacity = capacity;
            }

            public bool Append(T item)
            {
                if (_items.Count >= _capacity)
                {
                    return false;
                }
                else
                {
                    _items.Add(item);
                    return true;
                }
            }
        }

        [Fact]
        public void DemoAppenderObject()
        {
            var coll = new List<int>();

            var appender1 = new Appender<int>(coll, 3);

            var appender2 = new Appender<int>(coll, 5);

            coll.Should().BeEquivalentTo(new List<int> {});

            appender1.Append(1).Should().BeTrue();
            coll.Should().BeEquivalentTo(new List<int> { 1 });

            appender2.Append(2).Should().BeTrue();
            coll.Should().BeEquivalentTo(new List<int> { 1, 2 });

            appender1.Append(3).Should().BeTrue();
            coll.Should().BeEquivalentTo(new List<int> { 1, 2, 3 });

            appender1.Append(4).Should().BeFalse();
            coll.Should().BeEquivalentTo(new List<int> { 1, 2, 3 });

            appender2.Append(4).Should().BeTrue();
            coll.Should().BeEquivalentTo(new List<int> { 1, 2, 3, 4 });

            appender2.Append(5).Should().BeTrue();
            coll.Should().BeEquivalentTo(new List<int> { 1, 2, 3, 4, 5 });

            appender2.Append(6).Should().BeFalse();
            coll.Should().BeEquivalentTo(new List<int> { 1, 2, 3, 4, 5 });
        }
    }
}
