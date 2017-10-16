using System.Text;
using Ipfs.Json;
using Xunit;

namespace Ipfs.Test
{
    public class JsonSerializerTests
    {
        private readonly IJsonSerializer _jsonSerializer;

        public JsonSerializerTests()
        {
            _jsonSerializer = new JsonSerializer();
        }

        [Fact]
        public void ShouldSerializeMerkleNodeCorrectly()
        {
            MerkleNode merkleNode = new MerkleNode
            {
                Size = 8,
                Name = "My Merkle Node",
                Hash = new MultiHash("QmT78zSuBmuS4z925WZfrqQ1qHaJ56DQaTfyMUF7F8ff5o"),
                Data= Encoding.UTF8.GetBytes("My string"),
                Links = null,
            };

            string actual = _jsonSerializer.Serialize(merkleNode);
            string expected = "{\"Data\":\"TXkgc3RyaW5n\",\"Hash\":\"QmT78zSuBmuS4z925WZfrqQ1qHaJ56DQaTfyMUF7F8ff5o\",\"Links\":null,\"Name\":\"My Merkle Node\",\"Size\":8}";

            Assert.True(Equals(actual, expected));
        }
    }
}
