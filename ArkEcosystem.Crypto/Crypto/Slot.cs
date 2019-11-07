// Author:
//       Brian Faust <brian@ark.io>
//
// Copyright (c) 2018 Ark Ecosystem <info@ark.io>
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using ArkEcosystem.Crypto.Managers;

namespace ArkEcosystem.Crypto
{
    public static class Slot
    {
        public static UInt32 GetTime()
        {
            var epoch = ConfigManager.GetMilestone(1).Value.epoch;
            return Convert.ToUInt32((DateTime.UtcNow - epoch).TotalMilliseconds / 1000);
        }

        public static UInt32 GetTimeInMsUntilNextSlot() {
            var nextSlotTime = GetSlotTime(GetNextSlot());
            var now = GetTime();

            return (nextSlotTime - now) * 1000;
        }

        public static UInt32 GetSlotNumber(UInt32? epoch = null) {
            if (epoch == null) {
                epoch = GetTime();
            }

            return Math.Floor(epoch / ConfigManager.GetMilestone(1).Value.blocktime);
        }

        public static UInt32 GetSlotTime(UInt32 slot) {
            return slot * ConfigManager.GetMilestone(1).Value.blocktime;
        }

        public static UInt32 GetNextSlot() {
            return GetSlotNumber() + 1;
        }

        public static bool IsForgingAllowed(UInt32? epoch = null) {
            if (epoch == null) {
                epoch = GetTime();
            }

            var blockTime = ConfigManager.GetMilestone(1).Value.blocktime;

            return epoch % blockTime < blockTime / 2;
        }
    }
}
