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
using System.Collections.Generic;
using System.Linq;
using ArkEcosystem.Crypto.Enums;

namespace ArkEcosystem.Crypto.Managers
{
    public class Milestone : IMilestone {
        public UInt32 Index { get; set; }
        public KeyValuePair<string, dynamic> Data { get; set; }
    }

    public static class ConfigManager
    {
        private static INetworkConfig config;
        private static UInt32? height;
        private static IMilestone milestone;
        private static Dictionary<string, dynamic> milestones;

        static ConfigManager() {
            SetConfig(Networks.NetworkConfig.Devnet);
        }

        public static void SetConfig(INetworkConfig config) {
            ConfigManager.config = config;

            validateMilestones();
            buildConstants();
            buildFees();
        }

        public static void SetFromPreset(string network) {
            SetConfig(GetPreset(network));
        }

        public static INetworkConfig GetPreset(string network) {
            return Networks.GetNetwork(network);
        }

        public static INetworkConfig All() { return config; }

        public static void Set<T>(string key, T value) {
            // TODO: uses lodash set to dynamically modify TS object
        }

        public static T Get<T>(string key) {
            // TODO: uses lodash set to dynamically modify TS object
        }

        public static void SetHeight(UInt32 height) {
            ConfigManager.height = height;
        }

        public static UInt32 GetHeight() { return height; }

        public static bool IsNewMilestone() {
            //TODO translate from TS
            //return this.milestones.map(milestone => milestone.height).includes(this.height);
            return false;
        }

        public static KeyValuePair<string, dynamic> GetMilestone() {
            return GetMilestone(null);
        }
        public static KeyValuePair<string, dynamic> GetMilestone(UInt32? height) {
            if (!height.HasValue && ConfigManager.height.HasValue)
            {
                height = ConfigManager.height;
            }
            if (!height.HasValue) {
                height = 1;
            }

            while(
                milestone.Index < milestones.Count - 1 &&
                height >= milestones.ElementAt((int)milestone.Index + 1).Value.height
            ) {
                ++milestone.Index;
                milestone.Data = milestones.ElementAt((int)milestone.Index);
            }
            return milestone.Data;
        }

        public static dynamic GetMilestones() {
            return milestones;
        }

        private static void buildConstants() {
            milestones = config.Milestones.OrderBy(m => m.Value.height) as Dictionary<string, dynamic>;
            milestone = new Milestone {
                Index = 0,
                Data = milestones.ElementAt(0)
            };
            var lastMerged = 0;
            var overwriteMerged = (dest, source, options) => { return source };

            while(lastMerged < milestones.Count - 1) {
                // TODO: Implemented TS deepmerge
                //this.milestones[lastMerged + 1] = deepmerge(this.milestones[lastMerged], this.milestones[lastMerged + 1], {
                //    arrayMerge: overwriteMerge,
                //});
                lastMerged++;
            }
        }

        private static void validateMilestones() {
            var delegateMilestones = config.Milestones.OrderBy(m => m.Value.height).SelectMany<KeyValuePair<string, dynamic>, Dictionary<string, dynamic>>(m => m.Value.activeDelegates) as Dictionary<string, dynamic>;

            for (var i = 0; i < delegateMilestones.Count; ++i) {
                var previous = delegateMilestones.ElementAt(i - 1);
                var current = delegateMilestones.ElementAt(i);

                if (previous.Value.activeDelegates == current.Value.activeDelegates) {
                    continue;
                }
                if ((current.Value.height - previous.Value.height) % previous.Value.activeDelegates != 0) {
                    throw new InvalidMilestoneConfigurationError($"Bad milestone at height: ${current.Value.height}. The number of delegates can only be changed at the beginning of a new round.");
                }
            }
        }
    }
}
