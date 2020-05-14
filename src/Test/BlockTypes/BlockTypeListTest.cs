using System;
using System.Collections.Generic;

using Bones3Rebuilt;

using NUnit.Framework;

namespace Test
{
    public class BlockTypeListTest
    {
        [Test]
        public void DefaultBlockTypes()
        {
            var list = new BlockTypeList();

            Assert.AreEqual(2, list.Count);

            Assert.AreEqual("Ungenerated", list.GetBlockType(0).Name);
            Assert.AreEqual(0, list.GetBlockType(0).ID);
            Assert.IsFalse(list.GetBlockType(0).IsSolid);
            Assert.IsFalse(list.GetBlockType(0).IsVisible);

            Assert.AreEqual("Air", list.GetBlockType(1).Name);
            Assert.AreEqual(1, list.GetBlockType(1).ID);
            Assert.IsFalse(list.GetBlockType(1).IsSolid);
            Assert.IsFalse(list.GetBlockType(1).IsVisible);
        }

        [Test]
        public void AddBlock()
        {
            var list = new BlockTypeList();
            var block = new BlockBuilder(list.NextBlockID)
                .Name("Coal")
                .Build();
            list.AddBlockType(block);

            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(2, block.ID);
            Assert.AreEqual(block, list.GetBlockType(2));
        }

        [Test]
        public void RemoveBlock()
        {
            var list = new BlockTypeList();
            var block = new BlockBuilder(list.NextBlockID)
                .Build();
            list.AddBlockType(block);

            list.RemoveBlockType(block);

            Assert.AreEqual(2, list.Count);
        }

        [Test]
        public void RemoveBlock_Null()
        {
            var list = new BlockTypeList();
            var block = new BlockBuilder(list.NextBlockID)
                .Name("Iron")
                .Build();
            list.AddBlockType(block);

            list.RemoveBlockType(null);

            Assert.AreEqual(3, list.Count);
        }

        [Test]
        public void RemoveProtectedBlockType_Error()
        {
            var list = new BlockTypeList();

            Assert.Throws<AccessViolationException>(() =>
                list.RemoveBlockType(list.GetBlockType(0)));

            Assert.Throws<AccessViolationException>(() =>
                list.RemoveBlockType(list.GetBlockType(1)));
        }

        [Test]
        public void AddBlock_WrongID_Error()
        {
            var list = new BlockTypeList();

            Assert.Throws<ArgumentException>(() =>
                list.AddBlockType(new BlockBuilder(5).Build()));
        }
    }
}
