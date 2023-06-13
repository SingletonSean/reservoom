﻿using NUnit.Framework;
using Reservoom.Models;

namespace Reservoom.Test.Models
{
    public class RoomIDTest
    {
        [Test]
        public void ToString_ReturnsUniqueRoomId()
        {
            // arrange
            RoomID roomId = new RoomID(2, 15);

            // act
            string roomIdString = roomId.ToString();

            // assert
            Assert.That(roomIdString, Is.EqualTo("2_15"));
        }
    }
}
