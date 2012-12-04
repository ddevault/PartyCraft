using System;
using System.Collections.Generic;
using System.Text;
using LibNbt.Tags;
using NUnit.Framework;

namespace LibNbt.Test.Tags
{
    [TestFixture]
    public class ListTests
    {
        [Test]
        public void ChangingValidListTagType()
        {
            var list = new NbtList();
            list.Tags.Add(new NbtInt());

            Assert.DoesNotThrow(() => list.SetListType(NbtTagType.TAG_Int));
        }

        [Test]
        public void ChangingInvalidListTagType()
        {
            var list = new NbtList();
            list.Tags.Add(new NbtInt());

            Assert.Throws<Exception>(() => list.SetListType(NbtTagType.TAG_Short));
        }
    }
}
