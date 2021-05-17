using Task_4;

namespace Task_8.AsyncCypher
{
    public class TaskProperties
    {
        public int BlockNumber;
        public int BlocksQuantity;
        public ICypherAlgorithm CypherFramework;
        public byte[] Data;

        public TaskProperties(int blockNumber, int blocksQuantity, ICypherAlgorithm cypherFramework, byte[] data)
        {
            BlockNumber = blockNumber;
            BlocksQuantity = blocksQuantity;
            CypherFramework = cypherFramework;
            Data = data;
        }
    }
}